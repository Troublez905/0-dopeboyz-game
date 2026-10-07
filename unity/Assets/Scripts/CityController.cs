using System.Collections.Generic;
using UnityEngine;

namespace Dopeboyz
{
    public class CityController : MonoBehaviour
    {
        public static CityController Instance { get; private set; }

        [Header("District Roads (3D World Units)")]
        public float[] roadX = new float[] { 3.10f, 6.15f, 8.55f, 12.45f, 15.70f, 18.90f, 23.10f };
        public float[] roadZ = new float[] { 2.50f, 5.20f, 8.20f, 11.30f, 14.60f, 18.00f, 21.90f };

        [Header("3D Landmark Locations")]
        public Vector3 hqPosition = new Vector3(3.10f, 0.5f, 5.20f);
        public Vector3 shopPosition = new Vector3(8.55f, 0.5f, 8.20f);
        public Vector3 lowriderPosition = new Vector3(6.15f, 0.5f, 8.20f);
        public Vector3 vendingPosition = new Vector3(12.45f, 0.5f, 11.30f);
        public Vector3 subwayGratePosition = new Vector3(8.55f, 0.05f, 5.20f);
        public Vector3 poolTablePosition = new Vector3(3.10f, 0.5f, 8.20f);
        public Vector3 boomboxPosition = new Vector3(11.00f, 0.5f, 7.92f);
        public Vector3 marketStallPosition = new Vector3(18.90f, 0.5f, 5.20f);
        public Vector3 radioJammerPosition = new Vector3(15.70f, 0.5f, 18.00f);
        public float radioJammerTimer = 0f;

        [Header("Expanded 3D Landmark Positions")]
        public Vector3 hydrantGeyserPosition = new Vector3(15.70f, 0.5f, 11.30f);
        public Vector3 djBoothPosition = new Vector3(18.90f, 0.5f, 11.30f);
        public Vector3 paintTurretPosition = new Vector3(8.55f, 0.5f, 11.30f);
        public Vector3 heistCratePosition = new Vector3(6.15f, 0.5f, 14.60f);
        public Vector3 tuningBenchPosition = new Vector3(3.10f, 0.5f, 11.30f);
        public Vector3 airCompressorPosition = new Vector3(4.20f, 0.5f, 11.30f);
        public Vector3 motorcyclePosition = new Vector3(4.80f, 0.5f, 8.20f);
        public Vector3 waterTowerPosition = new Vector3(18.90f, 8.5f, 18.00f);
        public Vector3 foodTruckPosition = new Vector3(21.50f, 0.5f, 8.20f);
        public Vector3 barrelFirePosition = new Vector3(9.80f, 0.5f, 3.80f);
        public Vector3 manholePosition = new Vector3(7.45f, 0.05f, 14.60f);
        public Vector3 manholeExitPosition = new Vector3(20.70f, 0.05f, 4.92f);
        public Vector3 cityHallMonumentPosition = new Vector3(12.45f, 0.5f, 14.60f);
        public Vector3 newsChopperPosition = new Vector3(12.45f, 18.0f, 14.60f);
        public Vector3 electrifiedPuddlePosition = new Vector3(15.70f, 0.02f, 8.20f);
        public Vector3 storefrontPosition = new Vector3(18.90f, 0.5f, 14.60f);

        [Header("Landmark Interaction State")]
        public bool heistCrateOpened = false;
        public bool waterTowerTagged = false;
        public bool storefrontShattered = false;
        public bool blockPartyActive = false;

        [Header("Interactive 3D Entities")]
        public Transform lowriderTransform;
        public float lowriderBounce = 0f;
        public Transform blimpTransform;
        public float blimpSpeed = 1.4f;
        public Transform newsChopperTransform;
        public Transform reconDroneTransform;

        [Header("Walls & Territory")]
        public List<WallTag> districtWalls = new List<WallTag>();

        [Header("Prefabs for Dynamic Spawning")]
        public GameObject crewPrefab;
        public GameObject rivalPrefab;
        public GameObject copPrefab;
        public GameObject swatPrefab;
        public GameObject captainPrefab;
        public GameObject pickupPrefab;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetupDistrictWalls();
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) return;
            HandleBlimpPatrol();
            HandleNewsChopper();
            HandleReconDrone();
            HandleLowriderPhysics();
            HandleAIDirector();

            if (radioJammerTimer > 0f)
            {
                radioJammerTimer -= Time.deltaTime;
            }

            CheckProximityInteractions();
        }

        public void SetupDistrictWalls()
        {
            if (districtWalls == null || districtWalls.Count == 0)
            {
                districtWalls = new List<WallTag>(FindObjectsByType<WallTag>(FindObjectsSortMode.None));
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.totalWalls = Mathf.Max(12, districtWalls.Count);
            }
        }

        private void HandleBlimpPatrol()
        {
            if (blimpTransform != null)
            {
                blimpTransform.Translate(Vector3.right * (blimpSpeed * Time.deltaTime), Space.World);
                if (blimpTransform.position.x > 35.0f)
                {
                    blimpTransform.position = new Vector3(0.0f, blimpTransform.position.y, blimpTransform.position.z);
                }
            }
        }

        private void HandleNewsChopper()
        {
            if (newsChopperTransform != null)
            {
                // Circle overhead around district center
                float angle = Time.time * 0.45f;
                float radius = 9.0f;
                newsChopperTransform.position = newsChopperPosition + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(Time.time * 1.2f) * 0.5f, Mathf.Sin(angle) * radius);
                newsChopperTransform.rotation = Quaternion.Euler(5f, -angle * Mathf.Rad2Deg + 90f, 0f);
            }
        }

        private void HandleReconDrone()
        {
            if (reconDroneTransform != null)
            {
                // Circle overhead along outer street perimeter
                float angle = -Time.time * 0.65f;
                float radius = 13.5f;
                Vector3 center = new Vector3(25f, 5.5f, 25f);
                reconDroneTransform.position = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(Time.time * 2.2f) * 0.35f, Mathf.Sin(angle) * radius);
                reconDroneTransform.rotation = Quaternion.Euler(4f, -angle * Mathf.Rad2Deg - 90f, 0f);
            }
        }

        private void HandleLowriderPhysics()
        {
            if (lowriderTransform != null)
            {
                if (lowriderBounce > 0f)
                {
                    lowriderBounce -= Time.deltaTime * 3.5f;
                    float offsetY = Mathf.Abs(Mathf.Sin(Time.time * 24f)) * 0.6f * lowriderBounce;
                    lowriderTransform.position = new Vector3(lowriderPosition.x, lowriderPosition.y + offsetY, lowriderPosition.z);
                }
            }
        }

        private void CheckProximityInteractions()
        {
            if (PlayerController.Instance == null) return;
            Vector3 pPos = PlayerController.Instance.transform.position;
            bool interacting = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2);
            if (GameSession.Instance != null && Vector3.Distance(pPos, hqPosition) < 2.5f)
            {
                ShowPrompt("E", "HQ checkpoint and workbench");
                if (interacting) { GameSession.Instance.Checkpoint(); GameSession.Instance.ShowUpgrades(); }
                return;
            }

            // 1. Hydraulic Lowrider
            if (Vector3.Distance(pPos, lowriderPosition) < 2.5f)
            {
                ShowPrompt("[ E ]", "Trigger Hydraulic Bounce Shockwave");
                if (interacting)
                {
                    GameSession.Instance?.FinishDelivery();
                    TriggerLowriderBounce();
                }
                return;
            }

            // 2. Vending Machine
            if (Vector3.Distance(pPos, vendingPosition) < 2.0f)
            {
                ShowPrompt("[ E ]", "Buy Neon Energy Soda ($10)");
                if (interacting)
                {
                    BuyNeonSoda();
                }
                return;
            }

            // 3. Subway Updraft Grate
            if (Vector3.Distance(pPos, subwayGratePosition) < 2.0f)
            {
                ShowPrompt("[ AIR ]", "Rooftop Steam Updraft Vent");
                // Launch upward
                var rb = PlayerController.Instance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 14.5f, rb.linearVelocity.z);
                    SoundManager.Instance?.PlaySound(SoundType.Whoosh, 0.8f);
                }
                return;
            }

            // 4. Secret Sewer Manhole (Shift+E or E at manhole)
            if (Vector3.Distance(pPos, manholePosition) < 2.2f)
            {
                ShowPrompt("[ E ]", "Drop Underground to Secret Subway Passage");
                if (interacting)
                {
                    // Escape through sewers, clear police heat
                    PlayerController.Instance.transform.position = manholeExitPosition + Vector3.up * 0.5f;
                    GameManager.Instance?.AddHeat(-50f);
                    SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.0f);
                    GameManager.Instance?.Announce("🚇 Underground Sewer Escape! Police pursuit evaded!");
                }
                return;
            }

            // 5. Alley Barrel Fire Warmup
            if (Vector3.Distance(pPos, barrelFirePosition) < 2.8f)
            {
                ShowPrompt("[ WARM ]", "Alley Trash Can Fire Warmup Zone");
                // Heal & Stamina regen
                PlayerController.Instance.currentHealth = Mathf.Min(PlayerController.Instance.maxHealth, PlayerController.Instance.currentHealth + 12f * Time.deltaTime);
                PlayerController.Instance.currentStamina = Mathf.Min(PlayerController.Instance.maxStamina, PlayerController.Instance.currentStamina + 20f * Time.deltaTime);
                return;
            }

            // 6. Air Compressor Station
            if (Vector3.Distance(pPos, airCompressorPosition) < 2.2f)
            {
                ShowPrompt("[ E ]", "Recharge High-Pressure Air Compressor (Free)");
                if (interacting)
                {
                    PlayerController.Instance.currentPaint = PlayerController.Instance.maxPaint;
                    PlayerController.Instance.superchargedPaintTimer = 25f;
                    SoundManager.Instance?.PlaySound(SoundType.Whoosh, 0.9f);
                    GameManager.Instance?.Announce("💨 Air Compressor Recharged! 25s High-Pressure Paint Velocity!");
                }
                return;
            }

            // 7. Nozzle Tuning Bench
            if (Vector3.Distance(pPos, tuningBenchPosition) < 2.2f)
            {
                ShowPrompt("[ E ]", "Tune Spray Nozzle Calibration");
                if (interacting)
                {
                    if (GameSession.Instance != null) GameSession.Instance.ShowUpgrades();
                    else PlayerController.Instance.CycleNozzleCap();
                }
                return;
            }

            // 8. Block Party Boombox
            if (Vector3.Distance(pPos, boomboxPosition) < 2.5f)
            {
                ShowPrompt("[ E ]", "Start Block Party & Rep Fever Mode");
                if (interacting)
                {
                    blockPartyActive = true;
                    GameManager.Instance?.AddScore(150);
                    GameManager.Instance?.Announce("🎵 BLOCK PARTY STARTED! Rep Fever Multiplier Activated!");
                    SoundManager.Instance?.PlaySound(SoundType.LevelUp);
                }
                return;
            }

            // 9. Neon Storefront Glass
            if (Vector3.Distance(pPos, storefrontPosition) < 2.5f && !storefrontShattered)
            {
                ShowPrompt("[ E ]", "Shatter Neon Storefront Glass & Loot");
                if (interacting)
                {
                    storefrontShattered = true;
                    GameManager.Instance?.AddCash(85);
                    PlayerController.Instance.currentPaint = PlayerController.Instance.maxPaint;
                    GameManager.Instance?.AddHeat(20f);
                    SoundManager.Instance?.PlaySound(SoundType.Explosion, 0.7f);
                    GameManager.Instance?.Announce("💎 Storefront Shattered! Plundered $85 + Full Paint Refill!");
                }
                return;
            }

            // 10. Rooftop Water Tower Mega-Mural
            if (Vector3.Distance(pPos, waterTowerPosition) < 3.0f && !waterTowerTagged)
            {
                ShowPrompt("[ E ]", "Paint Rooftop Water Tower Mega-Mural");
                if (interacting)
                {
                    waterTowerTagged = true;
                    GameManager.Instance?.AddScore(600);
                    GameManager.Instance?.AddCash(250);
                    SoundManager.Instance?.PlaySound(SoundType.WallComplete);
                    GameManager.Instance?.Announce("🎨 WATER TOWER MEGA-MURAL COMPLETED! Earned $250 + 600 Rep!");
                }
                return;
            }

            // 11. Heist Cargo Crate
            if (Vector3.Distance(pPos, heistCratePosition) < 2.2f)
            {
                ShowPrompt("[ E ]", "Open Heist Cargo Crate");
                if (interacting)
                {
                    GameSession.Instance?.StartDelivery();
                    if (heistCrateOpened) return;
                    heistCrateOpened = true;
                    GameManager.Instance?.AddCash(120);
                    PlayerController.Instance.currentAmmo = PlayerController.Instance.maxAmmo;
                    SoundManager.Instance?.PlaySound(SoundType.LevelUp);
                    GameManager.Instance?.Announce("📦 Heist Crate Unlocked! Found $120 and Max Ammo!");
                }
                return;
            }

            // 12. Rooftop Zipline Hook
            Vector3 zipCableStart = new Vector3(8.55f, 6.0f, 8.20f);
            Vector3 zipCableEnd = new Vector3(18.90f, 6.0f, 18.00f);
            if (Vector3.Distance(pPos, zipCableStart) < 2.5f)
            {
                ShowPrompt("[ SPACE ]", "Grind Overhead Rooftop Cable Zipline");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerController.Instance.StartZipline(zipCableStart, zipCableEnd);
                }
                return;
            }

            // 13. Gourmet Food Truck (Taco Wagon)
            if (Vector3.Distance(pPos, foodTruckPosition) < 3.2f)
            {
                ShowPrompt("[ E ]", "Buy Gourmet Street Tacos ($15) [Restore Health & Stamina]");
                if (interacting)
                {
                    if (GameManager.Instance != null && GameManager.Instance.cash >= 15)
                    {
                        GameManager.Instance.AddCash(-15);
                        PlayerController.Instance.currentHealth = PlayerController.Instance.maxHealth;
                        PlayerController.Instance.currentStamina = PlayerController.Instance.maxStamina;
                        SoundManager.Instance?.PlaySound(SoundType.LevelUp);
                        GameManager.Instance.Announce("🌮 Delicious Street Tacos! Health & Stamina Fully Restored!");
                    }
                    else
                    {
                        GameManager.Instance?.Announce("❌ Not enough cash for street tacos ($15 required)");
                    }
                }
                return;
            }

            // Hide prompt if not near any interactive landmark
            HidePrompt();
        }

        private void TriggerLowriderBounce()
        {
            lowriderBounce = 1.0f;
            SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.0f);
            CameraController.Instance?.TriggerScreenShake(0.3f, 0.4f);

            // Repel nearby enemies with shockwave
            Collider[] hits = Physics.OverlapSphere(lowriderPosition, 6.5f);
            foreach (var h in hits)
            {
                if (h.CompareTag("Enemy") || h.CompareTag("Cop") || h.CompareTag("Rival"))
                {
                    var a = h.GetComponent<ActorController>();
                    if (a != null)
                    {
                        Vector3 push = (h.transform.position - lowriderPosition).normalized;
                        a.TakeDamage(15f, push * 12f);
                    }
                }
            }
            GameManager.Instance?.Announce("🚗 Hydraulic Bounce Shockwave blasted nearby rivals!");
        }

        private void BuyNeonSoda()
        {
            if (GameManager.Instance != null && GameManager.Instance.SpendCash(10))
            {
                PlayerController.Instance.neonSodaTimer = 35f;
                PlayerController.Instance.currentStamina = PlayerController.Instance.maxStamina;
                SoundManager.Instance?.PlaySound(SoundType.Pickup);
                GameManager.Instance?.Announce("⚡ Chugged Neon Energy Soda! +35% Speed Boost for 35s!");
            }
            else
            {
                GameManager.Instance?.Announce("⚠️ Need $10 for Neon Soda!");
            }
        }

        public bool SpawnCrewRecruit(Vector3 pos)
        {
            GameObject recruit = null;
            if (crewPrefab != null)
            {
                recruit = Instantiate(crewPrefab, pos, Quaternion.identity);
            }
            else
            {
                recruit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                recruit.transform.position = pos;
                recruit.transform.localScale = new Vector3(0.8f, 1.0f, 0.8f);
                var ren = recruit.GetComponent<Renderer>();
                if (ren != null)
                {
                    Material m = new Material(Shader.Find("Standard"));
                    m.color = Color.cyan;
                    ren.material = m;
                }
            }

            recruit.tag = "Crew";
            var act = recruit.GetComponent<ActorController>();
            if (act == null) act = recruit.AddComponent<ActorController>();
            act.kind = ActorKind.Crew;
            act.crewOrder = CrewOrder.Follow;

            return true;
        }

        public int GetActiveCrewCount()
        {
            int count = 0;
            var actors = Object.FindObjectsByType<ActorController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var a in actors)
            {
                if (a.kind == ActorKind.Crew) count++;
            }
            return count;
        }

        [Header("Dynamic AI Director")]
        public float aiDirectorTimer = 4.0f;
        public bool rivalBossSummoned = false;

        private void HandleAIDirector()
        {
            aiDirectorTimer -= Time.deltaTime;
            if (aiDirectorTimer > 0f) return;
            aiDirectorTimer = 7.0f;

            float heat = GameManager.Instance != null ? GameManager.Instance.cityHeat : 0f;
            int crewWalls = GameManager.Instance != null ? GameManager.Instance.crewWalls : 0;

            int activeCops = CountActors(ActorKind.Cop);
            int activeSwat = CountActors(ActorKind.SwatRiot);
            int activeK9 = CountActors(ActorKind.PoliceK9);
            int activeRivals = CountActors(ActorKind.Rival);

            // 1. Police pursuit scaling with Heat
            int desiredCops = Mathf.Clamp(Mathf.FloorToInt(heat / 16f), 0, 4);
            if (activeCops < desiredCops)
            {
                SpawnPoliceOfficer(GetRandomRoadPoint());
            }

            if (heat >= 35f && activeSwat < 2)
            {
                SpawnSwatOfficer(GetRandomRoadPoint());
            }

            if (heat >= 60f && activeK9 < 2)
            {
                SpawnPoliceK9(GetRandomRoadPoint());
            }

            // 2. Rival Gang Grunts contesting walls
            int desiredRivals = Mathf.Clamp(crewWalls + 1, 1, 4);
            if (activeRivals < desiredRivals)
            {
                SpawnRivalGrunt(GetRandomRoadPoint());
            }

            // 3. Boss Captain Showdown trigger at 4+ walls claimed
            if (crewWalls >= 4 && !rivalBossSummoned)
            {
                rivalBossSummoned = true;
                SpawnRivalBossCaptain(new Vector3(30f, 0.5f, 30f));
                GameManager.Instance?.Announce("🚨 WARNING: INK DRIP RIVAL BOSS CAPTAIN HAS ENTERED THE DISTRICT!");
                SoundManager.Instance?.PlaySound(SoundType.Siren);
            }
        }

        public GameObject SpawnPoliceOfficer(Vector3 pos)
        {
            GameObject cop = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            cop.name = "Patrol_Police_Officer";
            cop.transform.position = pos;
            cop.tag = "Cop";
            var ren = cop.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = new Color(0.12f, 0.25f, 0.65f);
                ren.material = m;
            }
            var act = cop.AddComponent<ActorController>();
            act.kind = ActorKind.Cop;
            return cop;
        }

        public GameObject SpawnSwatOfficer(Vector3 pos)
        {
            GameObject swat = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            swat.name = "SWAT_Riot_Enforcer";
            swat.transform.position = pos;
            swat.tag = "Cop";
            var ren = swat.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = new Color(0.2f, 0.22f, 0.28f);
                ren.material = m;
            }
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Quad);
            shield.name = "RiotShieldVisual";
            shield.transform.SetParent(swat.transform, false);
            shield.transform.localPosition = new Vector3(0f, 0f, 0.55f);
            shield.transform.localScale = new Vector3(1.1f, 1.6f, 1f);
            var col = shield.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var sRen = shield.GetComponent<Renderer>();
            if (sRen != null)
            {
                Material sm = new Material(Shader.Find("Standard"));
                sm.color = new Color(0.1f, 0.1f, 0.15f);
                sRen.material = sm;
            }
            var act = swat.AddComponent<ActorController>();
            act.kind = ActorKind.SwatRiot;
            act.hasRiotShield = true;
            return swat;
        }

        public GameObject SpawnPoliceK9(Vector3 pos)
        {
            GameObject k9 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            k9.name = "Police_K9_Tracker";
            k9.transform.position = pos;
            k9.transform.localScale = new Vector3(0.6f, 0.6f, 1.1f);
            k9.tag = "Cop";
            var ren = k9.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = new Color(0.45f, 0.3f, 0.15f);
                ren.material = m;
            }
            var act = k9.AddComponent<ActorController>();
            act.kind = ActorKind.PoliceK9;
            return k9;
        }

        public GameObject SpawnRivalGrunt(Vector3 pos)
        {
            GameObject rival = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rival.name = "Rival_Gang_Grunt";
            rival.transform.position = pos;
            rival.tag = "Rival";
            var ren = rival.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = new Color(0.95f, 0.15f, 0.55f);
                ren.material = m;
            }
            var act = rival.AddComponent<ActorController>();
            act.kind = ActorKind.Rival;
            return rival;
        }

        public GameObject SpawnRivalBossCaptain(Vector3 pos)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Rival_Boss_Captain";
            boss.transform.position = pos;
            boss.transform.localScale = new Vector3(1.1f, 1.15f, 1.1f);
            boss.tag = "Enemy";
            var ren = boss.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = new Color(0.85f, 0.05f, 0.25f);
                ren.material = m;
            }
            var act = boss.AddComponent<ActorController>();
            act.kind = ActorKind.RivalCaptain;
            return boss;
        }

        public int CountActors(ActorKind kind)
        {
            int count = 0;
            var actors = Object.FindObjectsByType<ActorController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var a in actors)
            {
                if (a.kind == kind) count++;
            }
            return count;
        }

        public Vector3 GetRandomRoadPoint()
        {
            if (roadX.Length == 0 || roadZ.Length == 0) return new Vector3(25f, 0.5f, 25f);
            float rx = roadX[Random.Range(0, roadX.Length)];
            float rz = roadZ[Random.Range(0, roadZ.Length)];
            return new Vector3(rx, 0.5f, rz);
        }

        private void ShowPrompt(string key, string action)
        {
            GameSession.Instance?.SetPrompt(key + " / " + action);
            if (UIController.Instance != null)
            {
                UIController.Instance.ShowInteractionPrompt(key, action);
            }
        }

        private void HidePrompt()
        {
            GameSession.Instance?.SetPrompt("");
            if (UIController.Instance != null)
            {
                UIController.Instance.HideInteractionPrompt();
            }
        }
    }
}
