using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dopeboyz
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        [Header("HUD Sliders & Gauges")]
        public Slider healthSlider;
        public Slider paintSlider;
        public Slider staminaSlider;
        public Text ammoText;
        public Text cashText;
        public Text scoreText;
        public Text comboText;
        public Text wantedStarsText;
        public Text cameraViewText;

        [Header("Campaign & District Tracker")]
        public Text districtTitleText;
        public Text objectiveText;
        public Text holdTimerText;

        [Header("Toast Notification Banner")]
        public GameObject toastPanel;
        public Text toastText;
        private Coroutine toastCoroutine;

        [Header("Upgrades Shop Panel")]
        public GameObject upgradesPanel;
        public Text creditsAvailableText;
        public Text upgradesShopStatusText;
        public bool IsUpgradesOpen => upgradesPanel != null && upgradesPanel.activeSelf;

        [Header("Cyber Radar / Minimap")]
        public GameObject radarPanel;
        public RectTransform radarBlipContainer;
        private List<Image> radarBlips = new List<Image>();

        [Header("Contextual Interaction Prompt")]
        public GameObject interactionPromptPanel;
        public Text interactionPromptKeyText;
        public Text interactionPromptActionText;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            EnsureRadarPanel();
            EnsureUpgradesShopPanel();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAnnouncement += ShowToast;
                GameManager.Instance.OnCashChanged += UpdateCash;
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnWantedLevelChanged += UpdateWantedStars;
            }

            if (CampaignManager.Instance != null)
            {
                CampaignManager.Instance.OnDistrictChanged += (idx) => UpdateDistrictInfo();
                CampaignManager.Instance.OnCreditsChanged += (credits) => UpdateCredits(credits);
            }

            UpdateDistrictInfo();

            // Refresh bottom controls hint bar to include new mechanics
            Text hintsLabel = GameObject.Find("HintsLabel")?.GetComponent<Text>();
            if (hintsLabel != null)
            {
                hintsLabel.text = "[WASD] Move/Skate | [SPACE] Jump/Trick/Grind | [LMB] Spray | [RMB] Shoot | [TAB] Black Market | [V] Drone | [T] Nozzle | [1-6] Stencil | [N] Turret | [K] Mine | [U] Bubble | [L] Slow-Mo | [F1] Camera";
            }
        }

        private void Update()
        {
            if (GameSession.Instance != null) return;
            UpdatePlayerGauges();
            UpdateComboDisplay();
            UpdateCameraViewDisplay();
            UpdateDistrictHoldTimer();
            UpdateRadar();

            // Toggle Upgrades Menu with Tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleUpgradesPanel();
            }

            if (IsUpgradesOpen)
            {
                HandleShopHotkeys();
            }
        }

        private void HandleShopHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleUpgradesPanel();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) BuyShopItem(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) BuyShopItem(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) BuyShopItem(3);
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) BuyShopItem(4);
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) BuyShopItem(5);
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) BuyShopItem(6);
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) BuyShopItem(7);
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) BuyShopItem(8);
        }

        public void BuyShopItem(int index)
        {
            if (index >= 6 && index <= 8)
            {
                string id = index == 6 ? "speed" : index == 7 ? "paint" : "health";
                if (CampaignManager.Instance != null && CampaignManager.Instance.BuyUpgrade(id))
                    GameSession.Instance?.Save();
                else ShowToast("Upgrade requires a skill credit and an available rank.");
                return;
            }
            if (PlayerController.Instance == null || GameManager.Instance == null) return;
            var p = PlayerController.Instance;
            var gm = GameManager.Instance;
            var cm = CampaignManager.Instance;

            switch (index)
            {
                case 1: // Med Stimpack ($25)
                    if (gm.cash >= 25 && p.currentHealth < p.maxHealth)
                    {
                        gm.AddCash(-25);
                        p.currentHealth = Mathf.Min(p.maxHealth, p.currentHealth + 50f);
                        SoundManager.Instance?.PlaySound(SoundType.HealthPickup);
                        ShowToast("💊 Med Stimpack Used! Health +50 HP");
                    }
                    else if (gm.cash < 25) ShowToast("❌ Not enough cash for Med Stimpack ($25 needed)");
                    else ShowToast("⚠️ Health is already full!");
                    break;

                case 2: // Paint Refill Drum ($30)
                    if (gm.cash >= 30 && p.currentPaint < p.maxPaint)
                    {
                        gm.AddCash(-30);
                        p.currentPaint = Mathf.Min(p.maxPaint, p.currentPaint + 60f);
                        SoundManager.Instance?.PlaySound(SoundType.PaintPickup);
                        ShowToast("🎨 High-Pressure Paint Refilled (+60 Paint)");
                    }
                    else if (gm.cash < 30) ShowToast("❌ Not enough cash for Paint Drum ($30 needed)");
                    else ShowToast("⚠️ Paint tank is already full!");
                    break;

                case 3: // Ammo Drum ($20)
                    if (gm.cash >= 20 && p.currentAmmo < p.maxAmmo)
                    {
                        gm.AddCash(-20);
                        p.currentAmmo = Mathf.Min(p.maxAmmo, p.currentAmmo + 36);
                        SoundManager.Instance?.PlaySound(SoundType.AmmoPickup);
                        ShowToast("📦 Paintball Ammo Box Loaded (+36 Paintballs)");
                    }
                    else if (gm.cash < 20) ShowToast("❌ Not enough cash for Ammo Box ($20 needed)");
                    else ShowToast("⚠️ Ammo is already full!");
                    break;

                case 4: // Sentry Paint Turret Kit ($65)
                    if (gm.cash >= 65)
                    {
                        gm.AddCash(-65);
                        p.DeployTurret();
                        ShowToast("🤖 Tactical Sentry Turret Deployed!");
                    }
                    else ShowToast("❌ Not enough cash for Turret Kit ($65 needed)");
                    break;

                case 5: // Police Radio Scrambler / Bribe ($75)
                    if (gm.cash >= 75 && gm.cityHeat > 0)
                    {
                        gm.AddCash(-75);
                        gm.AddHeat(-45f);
                        SoundManager.Instance?.PlaySound(SoundType.Whoosh);
                        ShowToast("📻 Police Dispatch Scrambled! -45 Heat Cleared");
                    }
                    else if (gm.cash < 75) ShowToast("❌ Not enough cash for Police Radio Scrambler ($75 needed)");
                    else ShowToast("⚠️ Police Heat is already zero!");
                    break;

            }

            UpdateShopStatus();
        }

        private void UpdateShopStatus()
        {
            if (upgradesShopStatusText != null && GameManager.Instance != null && PlayerController.Instance != null)
            {
                int creds = CampaignManager.Instance != null ? CampaignManager.Instance.levelCredits : 0;
                upgradesShopStatusText.text = $"CASH: ${GameManager.Instance.cash}  |  CREDITS: {creds}  |  HEAT: {GameManager.Instance.cityHeat:F0}%  |  HP: {PlayerController.Instance.currentHealth:F0}/{PlayerController.Instance.maxHealth:F0}";
            }
        }

        private void EnsureUpgradesShopPanel()
        {
            if (upgradesPanel != null) return;

            // Dynamically construct sleek Cyberpunk Black Market Shop
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null) parentCanvas = FindFirstObjectByType<Canvas>();
            if (parentCanvas == null) return;

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            GameObject panelObj = new GameObject("UpgradesShopPanel");
            panelObj.transform.SetParent(parentCanvas.transform, false);
            RectTransform rt = panelObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(620f, 440f);

            var bgImg = panelObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.06f, 0.11f, 0.95f);

            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panelObj.transform, false);
            RectTransform trt = titleObj.AddComponent<RectTransform>();
            trt.anchoredPosition = new Vector2(0f, 185f);
            trt.sizeDelta = new Vector2(580f, 40f);
            Text titleTxt = titleObj.AddComponent<Text>();
            titleTxt.text = "⚡ BLACK MARKET & TURF UPGRADES ⚡";
            titleTxt.font = defaultFont;
            titleTxt.fontSize = 20;
            titleTxt.alignment = TextAnchor.MiddleCenter;
            titleTxt.color = new Color(0.3f, 0.96f, 0.84f);

            // Subtitle
            GameObject statusObj = new GameObject("StatusText");
            statusObj.transform.SetParent(panelObj.transform, false);
            RectTransform srt = statusObj.AddComponent<RectTransform>();
            srt.anchoredPosition = new Vector2(0f, 150f);
            srt.sizeDelta = new Vector2(580f, 30f);
            upgradesShopStatusText = statusObj.AddComponent<Text>();
            upgradesShopStatusText.font = defaultFont;
            upgradesShopStatusText.fontSize = 14;
            upgradesShopStatusText.alignment = TextAnchor.MiddleCenter;
            upgradesShopStatusText.color = new Color(1.0f, 0.88f, 0.43f);

            // Item descriptions
            string[] items = new string[]
            {
                "[ 1 ]  Med Stimpack (+$50 HP) ----------------- $25",
                "[ 2 ]  Hi-Pressure Paint Drum (+60 Paint) -------- $30",
                "[ 3 ]  Paintball Ammo Box (+36 Ammo) ---------- $20",
                "[ 4 ]  Tactical Sentry Turret Kit --------------- $65",
                "[ 5 ]  Police Dispatch Scrambler (-45 Heat) ---- $75",
                "[ 6 ]  Runner Speed Upgrade (+15%) --------- 1 Credit / $100",
                "[ 7 ]  Paint Reservoir Rig (+30 Cap) ------- 1 Credit / $100",
                "[ 8 ]  Kevlar Body Armor (+25 HP) ---------- 1 Credit / $100"
            };

            for (int i = 0; i < items.Length; i++)
            {
                GameObject rowObj = new GameObject($"ItemRow_{i + 1}");
                rowObj.transform.SetParent(panelObj.transform, false);
                RectTransform rrt = rowObj.AddComponent<RectTransform>();
                rrt.anchoredPosition = new Vector2(0f, 110f - i * 32f);
                rrt.sizeDelta = new Vector2(560f, 28f);

                var rowImg = rowObj.AddComponent<Image>();
                rowImg.color = (i % 2 == 0) ? new Color(0.08f, 0.12f, 0.20f, 0.7f) : new Color(0.05f, 0.08f, 0.14f, 0.7f);

                GameObject rowTxtObj = new GameObject("Label");
                rowTxtObj.transform.SetParent(rowObj.transform, false);
                RectTransform lrrt = rowTxtObj.AddComponent<RectTransform>();
                lrrt.anchorMin = Vector2.zero;
                lrrt.anchorMax = Vector2.one;
                lrrt.offsetMin = new Vector2(15f, 0f);
                lrrt.offsetMax = new Vector2(-15f, 0f);

                Text rTxt = rowTxtObj.AddComponent<Text>();
                rTxt.text = items[i];
                rTxt.font = defaultFont;
                rTxt.fontSize = 13;
                rTxt.alignment = TextAnchor.MiddleLeft;
                rTxt.color = (i >= 5) ? new Color(1.0f, 0.75f, 0.3f) : Color.white;
            }

            // Footer instructions
            GameObject footerObj = new GameObject("Footer");
            footerObj.transform.SetParent(panelObj.transform, false);
            RectTransform frt = footerObj.AddComponent<RectTransform>();
            frt.anchoredPosition = new Vector2(0f, -180f);
            frt.sizeDelta = new Vector2(580f, 30f);
            Text footerTxt = footerObj.AddComponent<Text>();
            footerTxt.text = "Press [ 1 - 8 ] to Buy Instantly  |  Press [ TAB / ESC ] to Return to Streets";
            footerTxt.font = defaultFont;
            footerTxt.fontSize = 12;
            footerTxt.alignment = TextAnchor.MiddleCenter;
            footerTxt.color = new Color(0.7f, 0.85f, 1f, 0.85f);

            upgradesPanel = panelObj;
            upgradesPanel.SetActive(false);
        }

        private void EnsureRadarPanel()
        {
            if (radarPanel != null) return;

            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null) parentCanvas = FindFirstObjectByType<Canvas>();
            if (parentCanvas == null) return;

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Container in Top-Right corner
            radarPanel = new GameObject("RadarHUDWidget");
            radarPanel.transform.SetParent(parentCanvas.transform, false);
            RectTransform rt = radarPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-20f, -65f);
            rt.sizeDelta = new Vector2(140f, 140f);

            var bgImg = radarPanel.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.07f, 0.12f, 0.88f);

            // Title Header
            GameObject rTitle = new GameObject("RadarTitle");
            rTitle.transform.SetParent(radarPanel.transform, false);
            RectTransform trt = rTitle.AddComponent<RectTransform>();
            trt.anchoredPosition = new Vector2(0f, 12f);
            trt.sizeDelta = new Vector2(140f, 20f);
            Text rTxt = rTitle.AddComponent<Text>();
            rTxt.text = "RADAR 45M";
            rTxt.font = defaultFont;
            rTxt.fontSize = 10;
            rTxt.alignment = TextAnchor.MiddleCenter;
            rTxt.color = new Color(0.3f, 0.96f, 0.84f);

            // Center Player Marker (Cyan Dot)
            GameObject playerDot = new GameObject("PlayerCenterDot");
            playerDot.transform.SetParent(radarPanel.transform, false);
            RectTransform prt = playerDot.AddComponent<RectTransform>();
            prt.anchoredPosition = Vector2.zero;
            prt.sizeDelta = new Vector2(8f, 8f);
            var pImg = playerDot.AddComponent<Image>();
            pImg.color = new Color(0.3f, 0.96f, 0.84f);

            // Blip Container
            GameObject blipContainerObj = new GameObject("BlipsContainer");
            blipContainerObj.transform.SetParent(radarPanel.transform, false);
            radarBlipContainer = blipContainerObj.AddComponent<RectTransform>();
            radarBlipContainer.anchorMin = Vector2.zero;
            radarBlipContainer.anchorMax = Vector2.one;
            radarBlipContainer.offsetMin = Vector2.zero;
            radarBlipContainer.offsetMax = Vector2.zero;

            // Pre-instantiate 30 blips
            for (int i = 0; i < 30; i++)
            {
                GameObject bObj = new GameObject($"Blip_{i}");
                bObj.transform.SetParent(radarBlipContainer, false);
                RectTransform brt = bObj.AddComponent<RectTransform>();
                brt.sizeDelta = new Vector2(5f, 5f);
                var img = bObj.AddComponent<Image>();
                img.color = Color.white;
                bObj.SetActive(false);
                radarBlips.Add(img);
            }
        }

        private void UpdateRadar()
        {
            if (radarPanel == null || PlayerController.Instance == null) return;

            Vector3 playerPos = PlayerController.Instance.transform.position;
            float radarRadius = 60f; // pixel radius in UI
            float maxScanWorldDist = 45f;
            int blipIndex = 0;

            // 1. Walls (Green = Crew, Red = Rival, Yellow = Neutral)
            var walls = CityController.Instance != null ? CityController.Instance.districtWalls : null;
            if (walls != null)
            {
                foreach (var wall in walls)
                {
                    if (wall == null || blipIndex >= radarBlips.Count) continue;
                    Vector3 diff = wall.transform.position - playerPos;
                    float dist = diff.magnitude;
                    if (dist > maxScanWorldDist) continue;

                    float nx = (diff.x / maxScanWorldDist) * radarRadius;
                    float ny = (diff.z / maxScanWorldDist) * radarRadius;

                    Color c = Color.yellow;
                    if (wall.owner == WallOwner.Crew) c = new Color(0.2f, 1.0f, 0.4f);
                    else if (wall.owner == WallOwner.Rival) c = new Color(1.0f, 0.2f, 0.3f);

                    SetBlip(blipIndex++, nx, ny, c, 5f);
                }
            }

            // 2. Actors (Police, Swat, Rivals, Crew)
            ActorController[] actors = Object.FindObjectsByType<ActorController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var act in actors)
            {
                if (act == null || blipIndex >= radarBlips.Count) continue;
                Vector3 diff = act.transform.position - playerPos;
                float dist = diff.magnitude;
                if (dist > maxScanWorldDist || dist < 0.2f) continue;

                float nx = (diff.x / maxScanWorldDist) * radarRadius;
                float ny = (diff.z / maxScanWorldDist) * radarRadius;

                Color c = Color.white;
                float size = 6f;
                if (act.kind == ActorKind.Cop || act.kind == ActorKind.SwatRiot || act.kind == ActorKind.PoliceK9)
                {
                    // Flash red/blue
                    c = (Time.time % 0.4f > 0.2f) ? Color.red : new Color(0.2f, 0.6f, 1.0f);
                }
                else if (act.kind == ActorKind.Rival || act.kind == ActorKind.RivalCaptain)
                {
                    c = new Color(1.0f, 0.1f, 0.7f);
                    if (act.kind == ActorKind.RivalCaptain) size = 9f;
                }
                else if (act.kind == ActorKind.Crew)
                {
                    c = new Color(0.3f, 0.96f, 0.84f);
                }

                SetBlip(blipIndex++, nx, ny, c, size);
            }

            // Hide unused blips
            for (int i = blipIndex; i < radarBlips.Count; i++)
            {
                if (radarBlips[i].gameObject.activeSelf) radarBlips[i].gameObject.SetActive(false);
            }
        }

        private void SetBlip(int idx, float x, float y, Color col, float size)
        {
            var img = radarBlips[idx];
            img.gameObject.SetActive(true);
            img.color = col;
            RectTransform brt = img.rectTransform;
            brt.anchoredPosition = new Vector2(x, y);
            brt.sizeDelta = new Vector2(size, size);
        }

        private void UpdatePlayerGauges()
        {
            if (PlayerController.Instance == null) return;

            if (healthSlider != null)
            {
                healthSlider.value = PlayerController.Instance.currentHealth / PlayerController.Instance.maxHealth;
            }

            if (paintSlider != null)
            {
                paintSlider.value = PlayerController.Instance.currentPaint / PlayerController.Instance.maxPaint;
            }

            if (staminaSlider != null)
            {
                staminaSlider.value = PlayerController.Instance.currentStamina / PlayerController.Instance.maxStamina;
            }

            if (ammoText != null)
            {
                ammoText.text = $"AMMO: {PlayerController.Instance.currentAmmo} / {PlayerController.Instance.maxAmmo}";
            }
        }

        private void UpdateComboDisplay()
        {
            if (GameManager.Instance == null || comboText == null) return;

            if (GameManager.Instance.combo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"x{GameManager.Instance.combo} COMBO ({GameManager.Instance.comboTimer:F0}s)";
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }

        private void UpdateCameraViewDisplay()
        {
            if (CameraController.Instance != null && cameraViewText != null)
            {
                cameraViewText.text = CameraController.Instance.zoomTierNames[CameraController.Instance.currentTierIndex];
            }
        }

        private void UpdateDistrictInfo()
        {
            if (CampaignManager.Instance == null || CampaignManager.Instance.districts.Count == 0) return;
            int idx = CampaignManager.Instance.currentDistrictIndex;
            var dist = CampaignManager.Instance.districts[idx];

            if (districtTitleText != null)
            {
                districtTitleText.text = $"DISTRICT {idx + 1}: {dist.name}";
            }

            if (objectiveText != null)
            {
                objectiveText.text = $"OBJECTIVE: {dist.taskDescription} (Walls: {GameManager.Instance?.crewWalls}/{dist.requiredWalls})";
            }
        }

        private void UpdateDistrictHoldTimer()
        {
            if (CampaignManager.Instance == null || holdTimerText == null) return;
            int idx = CampaignManager.Instance.currentDistrictIndex;
            var dist = CampaignManager.Instance.districts[idx];

            if (dist.holdTimeRequired > 0f)
            {
                holdTimerText.gameObject.SetActive(true);
                holdTimerText.text = $"HOLD TIME: {CampaignManager.Instance.currentHoldTimer:F1}s / {dist.holdTimeRequired:F0}s";
            }
            else
            {
                holdTimerText.gameObject.SetActive(false);
            }
        }

        public void UpdateCash(int newCash)
        {
            if (cashText != null) cashText.text = $"$ {newCash}";
            UpdateShopStatus();
        }

        public void UpdateScore(int newScore)
        {
            if (scoreText != null) scoreText.text = $"REP: {newScore}";
        }

        public void UpdateCredits(int credits)
        {
            if (creditsAvailableText != null) creditsAvailableText.text = $"UPGRADE CREDITS: {credits}";
            UpdateShopStatus();
        }

        public void UpdateWantedStars(int stars)
        {
            if (wantedStarsText != null)
            {
                string starString = "";
                for (int i = 0; i < stars; i++) starString += "★ ";
                wantedStarsText.text = starString;
                wantedStarsText.color = stars > 2 ? new Color(1f, 0.25f, 0.35f) : new Color(1f, 0.88f, 0.43f);
            }
            UpdateShopStatus();
        }

        public void ShowToast(string message)
        {
            if (GameSession.Instance != null) { GameSession.Instance.Notify(message); return; }
            if (toastPanel == null || toastText == null) return;
            if (toastCoroutine != null) StopCoroutine(toastCoroutine);
            toastCoroutine = StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            toastText.text = message;
            toastPanel.SetActive(true);
            yield return new WaitForSeconds(3.5f);
            toastPanel.SetActive(false);
        }

        public void ToggleUpgradesPanel()
        {
            EnsureUpgradesShopPanel();
            if (upgradesPanel == null) return;
            bool active = !upgradesPanel.activeSelf;
            upgradesPanel.SetActive(active);
            if (active)
            {
                UpdateShopStatus();
                SoundManager.Instance?.PlaySound(SoundType.SprayCapSelect);
            }
        }

        public void SetInteractionPrompt(string key, string actionText)
        {
            if (interactionPromptPanel == null) return;
            if (interactionPromptKeyText != null) interactionPromptKeyText.text = key;
            if (interactionPromptActionText != null) interactionPromptActionText.text = actionText;
            if (!interactionPromptPanel.activeSelf) interactionPromptPanel.SetActive(true);
        }

        public void ClearInteractionPrompt()
        {
            if (interactionPromptPanel != null && interactionPromptPanel.activeSelf)
            {
                interactionPromptPanel.SetActive(false);
            }
        }

        public void ShowInteractionPrompt(string key, string actionText)
        {
            SetInteractionPrompt(key, actionText);
        }

        public void HideInteractionPrompt()
        {
            ClearInteractionPrompt();
        }
    }
}

