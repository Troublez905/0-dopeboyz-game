using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dopeboyz
{
    // Scene gameplay stays scene-owned. Only the session and its interface survive loads.
    public sealed class GameSession : MonoBehaviour
    {
        [Serializable] public sealed class SaveData
        {
            public int version = 1, xp, cash = 150, score, district, cleared, credits, contracts;
            public bool completed;
            public bool crateOpened, storefrontShattered, towerTagged, deliveryCompleted;
            public float stash = 50;
            public Vector3 checkpoint;
            public List<string> rewarded = new List<string>();
            public List<string> walls = new List<string>();
            public List<string> upgradeIds = new List<string>();
            public List<int> ranks = new List<int>();
        }
        public static GameSession Instance { get; private set; }
        public event Action ProgressionChanged;
        public int Level => Mathf.Min(10, 1 + progress.xp / 150);
        public static bool SmokeTest => Array.IndexOf(Environment.GetCommandLineArgs(), "--canal-smoke") >= 0;
        public string SavePath => Path.Combine(Application.persistentDataPath, SmokeTest ? "canal-smoke-test.json" : "canal-ink-yard-v1.json");
        public bool IsLoading { get; private set; }
        SaveData progress = new SaveData();
        GameObject menu;
        Transform menuContent;
        Text hud, message, objective, heading, prompt;
        Slider loading;
        Font font;
        string notice = "";
        float noticeUntil;
        bool ready, settings, confirmNew;
        float master = 0.8f, music = 0.35f, effects = 0.8f;
        WallTag[] walls = Array.Empty<WallTag>();
        float autosave;
        float challengeDeadline;
        HashSet<string> challengeWalls = new HashSet<string>();
        float deliveryDeadline;
        public bool HasDelivery => deliveryDeadline > Time.time;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            Application.runInBackground = true;
            if (Instance == null) new GameObject("Canal Session").AddComponent<GameSession>();
        }
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            master = PlayerPrefs.GetFloat("canal.master", .8f);
            music = PlayerPrefs.GetFloat("canal.music", .35f);
            effects = PlayerPrefs.GetFloat("canal.effects", .8f);
            BuildInterface();
        }
        IEnumerator Start()
        {
            yield return null;
            BindScene();
            SetState(GameState.MainMenu);
            ShowMenu();
            if (SmokeTest) gameObject.AddComponent<CanalSmokeTest>();
        }
        void BindScene()
        {
            ready = GameManager.Instance != null && PlayerController.Instance != null && CampaignManager.Instance != null && Camera.main != null;
            if (!ready) { Notify("Scene is missing its player, camera or managers. Open MainScene."); return; }
            CanalSceneRepair.Apply();
            walls = FindObjectsByType<WallTag>(FindObjectsSortMode.None);
            var oldUi = FindFirstObjectByType<UIController>();
            if (oldUi != null) oldUi.gameObject.SetActive(false);
            foreach (var events in FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
                if (!events.transform.IsChildOf(transform)) events.gameObject.SetActive(false);
            foreach (var listener in FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
                listener.enabled = listener.gameObject == Camera.main.gameObject;
            if (Camera.main.GetComponent<AudioListener>() == null) Camera.main.gameObject.AddComponent<AudioListener>();
            GameManager.Instance.OnAnnouncement += Notify;
            ApplyAudio();
            OctoberDistrictAssets.Install();
            RecountWalls();
        }
        void Update()
        {
            if (!ready || IsLoading) return;
            var gm = GameManager.Instance;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gm.currentState == GameState.Playing) { SetState(GameState.Paused); ShowMenu(); }
                else if (gm.currentState != GameState.MainMenu) { SetState(GameState.Playing); }
            }
            if (gm.currentState != GameState.Playing) return;
            if (Input.GetKeyDown(KeyCode.Tab)) { SetState(GameState.Shop); ShowUpgrades(); return; }
            RecountWalls();
            var p = PlayerController.Instance;
            if (challengeDeadline > 0 && Time.time > challengeDeadline) { challengeDeadline = 0; Notify("Tag sprint expired. Try again at the workbench."); }
            if (deliveryDeadline > 0 && Time.time > deliveryDeadline) { deliveryDeadline = 0; Notify("Delivery expired. Pick up a new shipment at the cargo crate."); }
            hud.text = $"HP {p.currentHealth:0}/{p.maxHealth:0}    PAINT {p.currentPaint:0}    AMMO {p.currentAmmo}\n$ {gm.cash}    REP {gm.score}    LEVEL {Level}    HEAT {gm.cityHeat:0}\n{(p.paintballSelected ? "PAINTBALL" : "SPRAY")}    XP {progress.xp}    CREDITS {CampaignManager.Instance.levelCredits}";
            objective.text = gm.contractActive ? "CONTRACT / " + gm.currentContractWall : gm.districtName + " / " + CampaignManager.Instance.districts[CampaignManager.Instance.currentDistrictIndex].taskDescription;
            if (gm.contractActive)
            {
                foreach (var wall in walls)
                    if (wall.spotName == gm.currentContractWall) objective.text += $" / {Vector3.Distance(p.transform.position, wall.transform.position):0}m";
            }
            message.text = Time.unscaledTime < noticeUntil ? notice : "";
            if (!p.isSpraying) gm.CoolHeat(Time.deltaTime * 1.5f);
            autosave += Time.deltaTime;
            if (autosave > 45f) { autosave = 0; Save(); }
        }
        public void Notify(string text)
        {
            notice = text; noticeUntil = Time.unscaledTime + 7;
            if (message != null) message.text = text;
        }
        public void SetPrompt(string text) { if (prompt != null) prompt.text = text; }
        void SetState(GameState state)
        {
            if (GameManager.Instance != null) GameManager.Instance.currentState = state;
            bool playing = state == GameState.Playing;
            Time.timeScale = playing ? 1 : 0;
            Cursor.lockState = playing ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !playing;
            menu.SetActive(!playing);
            hud.gameObject.SetActive(playing);
            objective.gameObject.SetActive(playing);
            prompt.gameObject.SetActive(playing);
        }
        public void RecordClaim(string owner, string name)
        {
            RecountWalls();
            if (owner != "crew") return;
            SoundManager.Instance?.PlayAt(SoundType.WallClaim, PlayerController.Instance.transform.position);
            if (challengeDeadline > Time.time && challengeWalls.Add(name) && challengeWalls.Count >= 3)
            {
                challengeDeadline = 0;
                Award("challenge:tag-sprint",150);
                Notify("Tag sprint complete!");
            }
            Award("wall:" + CampaignManager.Instance.currentDistrictIndex + ":" + name, 75);
            var gm = GameManager.Instance;
            if (gm.contractActive && gm.currentContractWall == name)
            {
                gm.contractActive = false;
                gm.AddCash(gm.contractReward);
                gm.AddScore(100);
                CampaignManager.Instance.contractsCompletedThisDistrict++;
                Award("contract:" + CampaignManager.Instance.currentDistrictIndex + ":" + name, 100);
                Notify("Contract complete. Return to HQ to checkpoint and upgrade.");
            }
        }
        public void Award(string id, int xp)
        {
            if (progress.rewarded.Contains(id)) return;
            int previous = Level;
            progress.rewarded.Add(id);
            progress.xp = Mathf.Min(1350, progress.xp + xp);
            CampaignManager.Instance.levelCredits += Level - previous;
            if (Level > previous) { Notify("LEVEL " + Level + " / Skill credit earned"); SoundManager.Instance?.PlaySound(SoundType.LevelUp); }
            ProgressionChanged?.Invoke();
        }
        void RecountWalls()
        {
            var gm = GameManager.Instance;
            gm.crewWalls = 0; gm.rivalWalls = 0; gm.totalWalls = walls.Length;
            foreach (var wall in walls)
            {
                if (wall.owner == WallOwner.Crew) gm.crewWalls++;
                if (wall.owner == WallOwner.Rival) gm.rivalWalls++;
            }
        }
        public void Checkpoint()
        {
            progress.checkpoint = PlayerController.Instance.transform.position;
            Save();
        }
        public void Recover()
        {
            var p = PlayerController.Instance;
            p.transform.position = progress.checkpoint;
            p.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            p.currentHealth = p.maxHealth; p.currentPaint = Mathf.Max(30,p.currentPaint);
            GameManager.Instance.CoolHeat(100);
            Notify("Recovered at HQ checkpoint.");
        }
        public void StartDelivery()
        {
            if (progress.rewarded.Contains("challenge:delivery:" + CampaignManager.Instance.currentDistrictIndex)) { Notify("This district's shipment has already been delivered."); return; }
            deliveryDeadline = Time.time + 90;
            Notify("Shipment collected / Reach the lowrider within 90 seconds.");
        }
        public void FinishDelivery()
        {
            if (!HasDelivery) return;
            deliveryDeadline = 0; Award("challenge:delivery:" + CampaignManager.Instance.currentDistrictIndex,150);
            CampaignManager.Instance.deliveryCompleted = true;
            GameManager.Instance.AddCash(100); GameManager.Instance.AddScore(100);
            Save(); Notify("Shipment delivered / $100 earned.");
        }
        public bool Save()
        {
            if (!ready || IsLoading) return false;
            var gm = GameManager.Instance; var cm = CampaignManager.Instance;
            progress.cash = gm.cash; progress.score = gm.score;
            progress.district = cm.currentDistrictIndex; progress.cleared = cm.clearedDistricts;
            progress.credits = cm.levelCredits; progress.contracts = cm.contractsCompletedThisDistrict;
            progress.completed = cm.campaignCompleted;
            progress.deliveryCompleted = cm.deliveryCompleted; progress.stash = cm.hqStashPaint;
            if (CityController.Instance != null)
            {
                progress.crateOpened = CityController.Instance.heistCrateOpened;
                progress.storefrontShattered = CityController.Instance.storefrontShattered;
                progress.towerTagged = CityController.Instance.waterTowerTagged;
            }
            progress.walls.Clear();
            foreach (var wall in walls) if (wall.owner == WallOwner.Crew) progress.walls.Add(wall.spotName);
            progress.upgradeIds.Clear(); progress.ranks.Clear();
            foreach (var pair in cm.upgrades) { progress.upgradeIds.Add(pair.Key); progress.ranks.Add(pair.Value.currentRank); }
            try
            {
                File.WriteAllText(SavePath + ".tmp", JsonUtility.ToJson(progress, true));
                if (File.Exists(SavePath)) File.Replace(SavePath + ".tmp", SavePath, SavePath + ".bak");
                else File.Move(SavePath + ".tmp", SavePath);
                return true;
            }
            catch (IOException ex) { Notify("Save failed: " + ex.Message); Debug.LogException(ex); return false; }
            catch (UnauthorizedAccessException ex) { Notify("Save access denied: " + ex.Message); Debug.LogException(ex); return false; }
        }
        public void Begin(bool resume)
        {
            if (IsLoading) return;
            SaveData data = new SaveData();
            if (resume)
            {
                try
                {
                    data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
                    if (data == null || data.version != 1 || data.district < 0 || data.district > 7 || data.upgradeIds == null || data.ranks == null || data.upgradeIds.Count != data.ranks.Count || data.walls == null || data.rewarded == null)
                        throw new InvalidDataException("Unsupported or damaged save. Original file preserved.");
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException)
                { Notify("Cannot continue: " + ex.Message); return; }
            }
            else if (File.Exists(SavePath))
            {
                try { File.Copy(SavePath, SavePath + ".before-new-game", true); }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                { Notify("Cannot preserve previous save: " + ex.Message); return; }
            }
            StartCoroutine(LoadGame(data, resume));
        }
        IEnumerator LoadGame(SaveData data, bool resume)
        {
            IsLoading = true; ready = false; SetState(GameState.Loading); ClearMenu();
            heading.text = "CANAL INK YARD";
            Label(menuContent, "Loading district", 22);
            loading = MakeSlider(menuContent, 0, null);
            yield return null;
            var operation = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            while (!operation.isDone) { loading.value = operation.progress; yield return null; }
            yield return null;
            BindScene();
            if (!ready) { IsLoading = false; ShowMenu(); yield break; }
            progress = data;
            var gm = GameManager.Instance; var cm = CampaignManager.Instance; var p = PlayerController.Instance;
            gm.cash = data.cash; gm.score = data.score;
            cm.currentDistrictIndex = data.district; cm.clearedDistricts = data.cleared;
            cm.levelCredits = data.credits; cm.contractsCompletedThisDistrict = data.contracts; cm.campaignCompleted = data.completed;
            cm.deliveryCompleted = data.deliveryCompleted; cm.hqStashPaint = data.stash;
            if (CityController.Instance != null)
            {
                CityController.Instance.heistCrateOpened = data.crateOpened;
                CityController.Instance.storefrontShattered = data.storefrontShattered;
                CityController.Instance.waterTowerTagged = data.towerTagged;
            }
            gm.districtLevel = data.district; gm.districtName = cm.districts[data.district].name;
            for (int i = 0; i < data.upgradeIds.Count; i++)
                if (cm.upgrades.ContainsKey(data.upgradeIds[i])) cm.upgrades[data.upgradeIds[i]].currentRank = Mathf.Clamp(data.ranks[i], 0, 5);
            cm.RefreshStats();
            foreach (var wall in walls)
            {
                wall.owner = data.walls.Contains(wall.spotName) ? WallOwner.Crew : WallOwner.None;
                wall.crewProgress = wall.owner == WallOwner.Crew ? 1 : 0; wall.rivalProgress = 0; wall.UpdateVisuals();
            }
            if (!resume) progress.checkpoint = p.transform.position;
            p.transform.position = progress.checkpoint;
            p.currentHealth = p.maxHealth; p.currentPaint = p.maxPaint;
            loading.value = 1;
            IsLoading = false; SetState(GameState.Playing); Save();
        }
        void BuildInterface()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var root = new GameObject("Session Interface", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform);
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.GetComponent<Canvas>().sortingOrder = 100;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;
            var events = new GameObject("Session Events", typeof(EventSystem), typeof(StandaloneInputModule)); events.transform.SetParent(transform);
            menu = new GameObject("Menu", typeof(RectTransform), typeof(Image)); menu.transform.SetParent(root.transform, false);
            var rect = (RectTransform)menu.transform; rect.anchorMin = new Vector2(0,0); rect.anchorMax = new Vector2(.4f,1); rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            menu.GetComponent<Image>().color = new Color(.025f,.035f,.04f,.94f);
            var content = new GameObject("Menu Content", typeof(RectTransform), typeof(VerticalLayoutGroup)); content.transform.SetParent(menu.transform,false);
            var cr = (RectTransform)content.transform; cr.anchorMin = Vector2.zero; cr.anchorMax = Vector2.one; cr.offsetMin = new Vector2(32,24); cr.offsetMax = new Vector2(-32,-32);
            var layout = content.GetComponent<VerticalLayoutGroup>(); layout.spacing = 10; layout.childForceExpandHeight = false; layout.childControlHeight = true;
            menuContent = content.transform;
            heading = Label(menuContent, "STREET SHOOTERS", 34);
            hud = ScreenLabel(root.transform, new Vector2(24,-24), new Vector2(580,100), TextAnchor.UpperLeft);
            objective = ScreenLabel(root.transform, new Vector2(24,-130), new Vector2(650,64), TextAnchor.UpperLeft);
            message = ScreenLabel(root.transform, new Vector2(530,-575), new Vector2(700,110), TextAnchor.UpperLeft);
            prompt = ScreenLabel(root.transform, new Vector2(400,-505), new Vector2(700,64), TextAnchor.UpperLeft);
            var reticle = ScreenLabel(root.transform,new Vector2(625,-345),new Vector2(30,30),TextAnchor.MiddleCenter);
            reticle.text = "+";
        }
        Text ScreenLabel(Transform parent, Vector2 position, Vector2 size, TextAnchor align)
        {
            Text text = Label(parent, "", 20); var r = text.rectTransform;
            r.anchorMin = r.anchorMax = new Vector2(0,1); r.pivot = new Vector2(0,1); r.anchoredPosition = position; r.sizeDelta = size; text.alignment = align; return text;
        }
        Text Label(Transform parent, string value, int size)
        {
            var go = new GameObject("Label", typeof(RectTransform), typeof(Text), typeof(LayoutElement)); go.transform.SetParent(parent,false);
            var text = go.GetComponent<Text>(); text.font = font; text.fontSize = size; text.text = value; text.color = new Color(.88f,.96f,.94f); text.raycastTarget = false;
            go.GetComponent<LayoutElement>().preferredHeight = size > 28 ? 92 : 44; return text;
        }
        void Button(string value, Action action)
        {
            var go = new GameObject(value, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement)); go.transform.SetParent(menuContent,false);
            go.GetComponent<LayoutElement>().preferredHeight = 44; go.GetComponent<Image>().color = new Color(.08f,.22f,.21f);
            var text = Label(go.transform,value,20); text.alignment = TextAnchor.MiddleCenter;
            text.rectTransform.anchorMin = Vector2.zero; text.rectTransform.anchorMax = Vector2.one; text.rectTransform.offsetMin = text.rectTransform.offsetMax = Vector2.zero;
            go.GetComponent<Button>().onClick.AddListener(() => action());
        }
        Slider MakeSlider(Transform parent, float value, Action<float> changed)
        {
            var go = new GameObject("Volume", typeof(RectTransform), typeof(Image), typeof(Slider), typeof(LayoutElement)); go.transform.SetParent(parent,false);
            go.GetComponent<LayoutElement>().preferredHeight = 22; go.GetComponent<Image>().color = Color.gray;
            var handle = new GameObject("Handle",typeof(RectTransform),typeof(Image)); handle.transform.SetParent(go.transform,false); handle.GetComponent<Image>().color = Color.cyan;
            var slider = go.GetComponent<Slider>(); slider.handleRect = (RectTransform)handle.transform; slider.handleRect.sizeDelta = new Vector2(18,22); slider.targetGraphic = handle.GetComponent<Image>(); slider.value = value;
            if (changed != null) slider.onValueChanged.AddListener(v => changed(v)); else slider.interactable = false;
            return slider;
        }
        void ClearMenu()
        {
            foreach (Transform child in menuContent) if (child.gameObject != heading.gameObject) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
        }
        void ShowMenu()
        {
            ClearMenu(); heading.text = "STREET\nSHOOTERS";
            if (settings)
            {
                Label(menuContent,"MASTER",18); MakeSlider(menuContent,master,v => {master=v; ApplyAudio();});
                Label(menuContent,"MUSIC",18); MakeSlider(menuContent,music,v => {music=v; ApplyAudio();});
                Label(menuContent,"EFFECTS",18); MakeSlider(menuContent,effects,v => {effects=v; ApplyAudio();});
                Button("Back",()=>{settings=false; PlayerPrefs.Save(); ShowMenu();}); return;
            }
            if (confirmNew)
            {
                Label(menuContent,"Replace current progress? A backup will be kept.",20);
                Button("Start new game",()=>{confirmNew=false; Begin(false);}); Button("Cancel",()=>{confirmNew=false;ShowMenu();}); return;
            }
            bool paused = ready && GameManager.Instance.currentState != GameState.MainMenu;
            if (paused)
            {
                Button("Resume",()=>SetState(GameState.Playing));
                Button("Restart checkpoint",()=>Begin(true));
                Button("Save and main menu",()=>{if(Save()){SetState(GameState.MainMenu);ShowMenu();}});
            }
            else
            {
                if (File.Exists(SavePath)) Button("Continue",()=>Begin(true));
                Button("New game",()=>{if(File.Exists(SavePath)){confirmNew=true;ShowMenu();}else Begin(false);});
            }
            Button("Settings",()=>{settings=true;ShowMenu();});
            Button("Quit",()=>Application.Quit());
        }
        public void ShowUpgrades()
        {
            SetState(GameState.Shop); ClearMenu(); heading.text = "WORKBENCH";
            var cm = CampaignManager.Instance;
            Label(menuContent,$"LEVEL {Level} / {cm.levelCredits} CREDITS",20);
            foreach (string id in new[]{"speed","paint","health","gear","crew"})
            {
                string key = id; var u = cm.upgrades[key];
                Button($"{u.name}  {u.currentRank}/5",()=>{if(cm.BuyUpgrade(key)){Save();ShowUpgrades();}else Notify("Requires a skill credit and the district unlock.");});
            }
            Button("Back",()=>SetState(GameState.Playing));
            Button("Tag sprint / 3 walls in 90s",()=>{challengeWalls.Clear();challengeDeadline=Time.time+90;SetState(GameState.Playing);});
        }
        void ApplyAudio()
        {
            AudioListener.volume = master;
            SoundManager.Instance?.SetVolumes(music,effects);
            PlayerPrefs.SetFloat("canal.master",master); PlayerPrefs.SetFloat("canal.music",music); PlayerPrefs.SetFloat("canal.effects",effects);
        }
        void OnDestroy() { if (Instance == this) {Instance = null; Time.timeScale = 1; Cursor.lockState = CursorLockMode.None; Cursor.visible = true;} }
    }
}
