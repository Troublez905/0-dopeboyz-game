using System.Collections.Generic;
using UnityEngine;

namespace Dopeboyz
{
    public class CityController : MonoBehaviour
    {
        public static CityController Instance { get; private set; }

        [Header("District Roads (Unity Units: 1 unit = 100px)")]
        public float[] roadX = new float[] { 3.10f, 6.15f, 8.55f, 12.45f, 15.70f, 18.90f, 23.10f };
        public float[] roadY = new float[] { 2.50f, 5.20f, 8.20f, 11.30f, 14.60f, 18.00f, 21.90f };

        [Header("Landmark Locations")]
        public Vector2 hqPosition = new Vector2(3.10f, 5.20f);
        public Vector2 shopPosition = new Vector2(8.55f, 8.20f);
        public Vector2 lowriderPosition = new Vector2(6.15f, 8.20f);
        public Vector2 vendingPosition = new Vector2(12.45f, 11.30f);
        public Vector2 subwayGratePosition = new Vector2(8.55f, 5.20f);
        public Vector2 poolTablePosition = new Vector2(3.10f, 8.20f);
        public Vector2 boomboxPosition = new Vector2(11.00f, 7.92f);
        public Vector2 marketStallPosition = new Vector2(18.90f, 5.20f);
        public Vector2 radioJammerPosition = new Vector2(15.70f, 18.00f);
        public float radioJammerTimer = 0f;

        [Header("Expanded Landmark Positions")]
        public Vector2 hydrantGeyserPosition = new Vector2(15.70f, 11.30f);
        public Vector2 djBoothPosition = new Vector2(18.90f, 11.30f);
        public Vector2 paintTurretPosition = new Vector2(8.55f, 11.30f);
        public Vector2 heistCratePosition = new Vector2(6.15f, 14.60f);
        public Vector2 tuningBenchPosition = new Vector2(3.10f, 11.30f);
        public Vector2 airCompressorPosition = new Vector2(4.20f, 11.30f);
        public Vector2 motorcyclePosition = new Vector2(4.80f, 8.20f);
        public Vector2 waterTowerPosition = new Vector2(18.90f, 18.00f);
        public Vector2 foodTruckPosition = new Vector2(21.50f, 8.20f);
        public Vector2 barrelFirePosition = new Vector2(9.80f, 3.80f);

        [Header("Landmark Interaction State")]
        public bool heistCrateOpened = false;
        public bool waterTowerTagged = false;

        [Header("Interactive Entities")]
        public Transform lowriderTransform;
        public float lowriderBounce = 0f;
        public Transform blimpTransform;
        public float blimpSpeed = 1.2f;

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
            HandleBlimpPatrol();
            HandleLowriderPhysics();
            if (radioJammerTimer > 0f)
            {
                radioJammerTimer -= Time.deltaTime;
            }
            CheckProximityInteractions();
        }

        public void SetupDistrictWalls()
        {
            // If walls are not pre-assigned in scene, find them
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
                blimpTransform.Translate(Vector3.right * (blimpSpeed * Time.deltaTime));
                if (blimpTransform.position.x > 25.0f)
                {
                    blimpTransform.position = new Vector3(1.0f, blimpTransform.position.y, blimpTransform.position.z);
                }
            }
        }

        private void HandleLowriderPhysics()
        {
            if (lowriderBounce > 0f)
            {
                lowriderBounce = Mathf.Max(0f, lowriderBounce - Time.deltaTime * 3.5f);
                if (lowriderTransform != null)
                {
                    float hop = Mathf.Sin(lowriderBounce * Mathf.PI) * 0.35f;
                    lowriderTransform.localPosition = new Vector3(lowriderTransform.localPosition.x, lowriderPosition.y + hop, lowriderTransform.localPosition.z);
                }
            }
        }

        private void CheckProximityInteractions()
        {
            if (PlayerController.Instance == null) return;
            Vector2 playerPos = PlayerController.Instance.transform.position;

            string promptKey = null;
            string promptAction = null;

            // 1. Lowrider Hydraulic Hop
            if (Vector2.Distance(playerPos, lowriderPosition) < 3.0f)
            {
                promptKey = "[ H ]";
                promptAction = "Lowrider Hydraulic Shockwave";
                if (Input.GetKeyDown(KeyCode.H)) TriggerLowriderHop();
            }
            // 2. Boombox Soundquake
            else if (Vector2.Distance(playerPos, boomboxPosition) < 3.0f)
            {
                promptKey = "[ B ]";
                promptAction = "Boombox Bass Soundquake (AoE Stun)";
                if (Input.GetKeyDown(KeyCode.B)) TriggerBoomboxSoundquake();
            }
            // 3. Contraband Heist Crate
            else if (Vector2.Distance(playerPos, heistCratePosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = heistCrateOpened ? "Emptied Heist Crate" : "Crack Heist Crate (Loot Cash & Paint)";
                if (Input.GetKeyDown(KeyCode.E)) CrackHeistCrate();
            }
            // 4. DJ Turntable Booth
            else if (Vector2.Distance(playerPos, djBoothPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "DJ Turntable (Hype Overdrive 1.8x Speed)";
                if (Input.GetKeyDown(KeyCode.E)) TriggerDJBooth();
            }
            // 5. Street Hydrant Geyser
            else if (Vector2.Distance(playerPos, hydrantGeyserPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "Fire Hydrant Geyser (Wash Walls & Launch Foes)";
                if (Input.GetKeyDown(KeyCode.E)) TriggerHydrantGeyser();
            }
            // 6. Nozzle Tuning Bench
            else if (Vector2.Distance(playerPos, tuningBenchPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = (PlayerController.Instance != null && PlayerController.Instance.calibratedNozzle) ? "Tuned Nozzles Active (+35% Reach)" : "Calibrate Spray Nozzles (+35% Reach)";
                if (Input.GetKeyDown(KeyCode.E)) UseTuningBench();
            }
            // 7. Air Compressor Station
            else if (Vector2.Distance(playerPos, airCompressorPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "Supercharge Paint Canister (+Velocity & Dmg)";
                if (Input.GetKeyDown(KeyCode.E)) UseAirCompressor();
            }
            // 8. Scrambler Motorcycle
            else if (Vector2.Distance(playerPos, motorcyclePosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "Rev Motorcycle Burnout (-1 Star Heat)";
                if (Input.GetKeyDown(KeyCode.E)) RevMotorcycle();
            }
            // 9. Water Tower Skyline Mural
            else if (Vector2.Distance(playerPos, waterTowerPosition) < 3.0f)
            {
                promptKey = "[ E ]";
                promptAction = waterTowerTagged ? "Skyline Mural (Claimed)" : "Tag Skyline Master Mural (+500 Rep)";
                if (Input.GetKeyDown(KeyCode.E)) TagWaterTower();
            }
            // 10. Vending Machine
            else if (Vector2.Distance(playerPos, vendingPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "Neon Soda Kiosk ($15 for Speed & Stamina)";
                if (Input.GetKeyDown(KeyCode.E)) UseVendingMachine();
            }
            // 11. Contraband Market Stall
            else if (Vector2.Distance(playerPos, marketStallPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "Contraband Black Market ($25 Full Restock)";
                if (Input.GetKeyDown(KeyCode.E)) UseMarketStall();
            }
            // 12. Police Radio Jammer
            else if (Vector2.Distance(playerPos, radioJammerPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = radioJammerTimer > 0f ? $"Radio Jammer Active ({radioJammerTimer:F0}s)" : "Hack Police Radio Jammer (25s Lockout)";
                if (Input.GetKeyDown(KeyCode.E)) ActivateRadioJammer();
            }
            // 13. Subway Grate Updraft Vent
            else if (Vector2.Distance(playerPos, subwayGratePosition) < 1.8f)
            {
                TriggerSubwayUpdraft();
            }
            // 14. Rooftop Pool Table Sanctuary
            else if (Vector2.Distance(playerPos, poolTablePosition) < 2.5f)
            {
                GameManager.Instance?.CoolHeat(4.0f * Time.deltaTime);
                promptKey = "[ REST ]";
                promptAction = "Rooftop Chill Sanctuary (Cooling Heat)";
            }
            // 15. Headquarters
            else if (Vector2.Distance(playerPos, hqPosition) < 2.5f)
            {
                promptKey = "[ E ]";
                promptAction = "Crew HQ (Stash Cash & Deliver Contraband)";
            }
            // 16. Street Food Truck
            else if (Vector2.Distance(playerPos, foodTruckPosition) < 2.8f)
            {
                promptKey = "[ E ]";
                promptAction = "Street Tacos ($10 Full Heal & Speed Boost)";
                if (Input.GetKeyDown(KeyCode.E)) UseFoodTruck();
            }
            // 17. Street Barrel Fire
            else if (Vector2.Distance(playerPos, barrelFirePosition) < 2.5f)
            {
                PlayerController.Instance.currentStamina = PlayerController.Instance.maxStamina;
                GameManager.Instance?.CoolHeat(6.0f * Time.deltaTime);
                promptKey = "[ REST ]";
                promptAction = "Barrel Fire Warmup (Refueling Stamina & Cooling Heat)";
            }

            if (promptKey != null)
            {
                UIController.Instance?.SetInteractionPrompt(promptKey, promptAction);
            }
            else
            {
                UIController.Instance?.ClearInteractionPrompt();
            }
        }

        public void TriggerLowriderHop()
        {
            lowriderBounce = 1.0f;
            CameraController.Instance?.TriggerScreenShake(0.3f, 0.35f);
            SoundManager.Instance?.PlaySound(SoundType.LowriderHop);
            GameManager.Instance?.Announce("🚗 Lowrider Hydraulic Hop Shockwave!");

            // Push away nearby enemies
            Collider2D[] colliders = Physics2D.OverlapCircleAll(lowriderPosition, 4.5f);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy") || col.CompareTag("Rival") || col.CompareTag("Cop"))
                {
                    Rigidbody2D erb = col.GetComponent<Rigidbody2D>();
                    if (erb != null)
                    {
                        Vector2 pushDir = ((Vector2)col.transform.position - lowriderPosition).normalized;
                        erb.AddForce(pushDir * 250f, ForceMode2D.Impulse);
                    }
                }
            }
        }

        public void TriggerBoomboxSoundquake()
        {
            CameraController.Instance?.TriggerScreenShake(0.4f, 0.45f);
            SoundManager.Instance?.PlaySound(SoundType.BoomboxBass, 1.2f);
            GameManager.Instance?.Announce("🔊 BOOMBOX SOUNDQUAKE! Bass shockwave clears the block!");

            Collider2D[] colliders = Physics2D.OverlapCircleAll(boomboxPosition, 5.5f);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy") || col.CompareTag("Rival") || col.CompareTag("Cop"))
                {
                    Rigidbody2D erb = col.GetComponent<Rigidbody2D>();
                    if (erb != null)
                    {
                        Vector2 pushDir = ((Vector2)col.transform.position - boomboxPosition).normalized;
                        erb.AddForce(pushDir * 320f, ForceMode2D.Impulse);
                    }
                    ActorController actor = col.GetComponent<ActorController>();
                    if (actor != null)
                    {
                        actor.TakeDamage(25f, ((Vector2)col.transform.position - boomboxPosition).normalized);
                    }
                }
            }
        }

        public void UseMarketStall()
        {
            if (GameManager.Instance != null && GameManager.Instance.SpendCash(25))
            {
                PlayerController.Instance.currentPaint = PlayerController.Instance.maxPaint;
                PlayerController.Instance.currentAmmo = PlayerController.Instance.maxAmmo;
                PlayerController.Instance.currentHealth = PlayerController.Instance.maxHealth;
                SoundManager.Instance?.PlaySound(SoundType.CashChime);
                GameManager.Instance.Announce("🛍️ Contraband Alley Bazaar: Full Paint & Ammo Refill!");
            }
            else
            {
                GameManager.Instance?.Announce("Need $25 for Contraband Black Market Supplies!");
            }
        }

        public void ActivateRadioJammer()
        {
            radioJammerTimer = 25.0f;
            SoundManager.Instance?.PlaySound(SoundType.HitDeflect);
            GameManager.Instance?.CoolHeat(50f);
            GameManager.Instance?.Announce("📡 Police Radio Jammer Active! Police Dispatch Blinded for 25s!");
        }

        public void UseVendingMachine()
        {
            if (GameManager.Instance != null && GameManager.Instance.SpendCash(15))
            {
                PlayerController.Instance.currentStamina = PlayerController.Instance.maxStamina;
                PlayerController.Instance.currentPaint = Mathf.Min(PlayerController.Instance.maxPaint, PlayerController.Instance.currentPaint + 30f);
                PlayerController.Instance.neonSodaTimer = 12.0f;
                SoundManager.Instance?.PlaySound(SoundType.CashChime);
                GameManager.Instance.Announce("🥤 Neon Soda Rush! +Sprint Speed & +Paint Ammo");
            }
            else
            {
                GameManager.Instance?.Announce("Need $15 for Neon Soda!");
            }
        }

        public void TriggerSubwayUpdraft()
        {
            Rigidbody2D prb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (prb != null)
            {
                prb.linearVelocity = new Vector2(prb.linearVelocity.x, 14f);
                SoundManager.Instance?.PlaySound(SoundType.UpdraftSteam, 0.9f);
                GameManager.Instance?.Announce("💨 Steam Updraft Vent Launch!");
            }
        }

        public void UseFoodTruck()
        {
            if (GameManager.Instance != null && GameManager.Instance.SpendCash(10))
            {
                PlayerController.Instance.currentHealth = PlayerController.Instance.maxHealth;
                PlayerController.Instance.currentStamina = PlayerController.Instance.maxStamina;
                PlayerController.Instance.neonSodaTimer = 15.0f;
                SoundManager.Instance?.PlaySound(SoundType.CashChime);
                GameManager.Instance.Announce("🌮 404 Street Tacos! Full Health Restored & Speed Surge!");
            }
            else
            {
                GameManager.Instance?.Announce("Need $10 for Street Tacos!");
            }
        }

        public void TriggerHydrantGeyser()
        {
            CameraController.Instance?.TriggerScreenShake(0.35f, 0.4f);
            SoundManager.Instance?.PlaySound(SoundType.GeyserBlast, 1.1f);
            GameManager.Instance?.Announce("🌊 HYDRANT GEYSER BURST! Water blast washes rival paint and clears the block!");

            // Wash rival paint within 6.5 units of hydrant
            foreach (var wall in districtWalls)
            {
                if (wall != null && Vector2.Distance(wall.transform.position, hydrantGeyserPosition) < 6.5f)
                {
                    if (wall.owner == "rival")
                    {
                        wall.ApplySpray("neutral", 100f);
                    }
                }
            }

            // Launch nearby enemies
            Collider2D[] colliders = Physics2D.OverlapCircleAll(hydrantGeyserPosition, 5.0f);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy") || col.CompareTag("Rival") || col.CompareTag("Cop"))
                {
                    Rigidbody2D erb = col.GetComponent<Rigidbody2D>();
                    if (erb != null)
                    {
                        Vector2 pushDir = ((Vector2)col.transform.position - hydrantGeyserPosition).normalized;
                        erb.AddForce((pushDir + Vector2.up * 1.5f).normalized * 350f, ForceMode2D.Impulse);
                    }
                }
            }
        }

        public void TriggerDJBooth()
        {
            CameraController.Instance?.TriggerScreenShake(0.2f, 0.25f);
            SoundManager.Instance?.PlaySound(SoundType.TurntableScratch, 1.2f);
            PlayerController.Instance?.ActivateHypeOverdrive(22.0f);
        }

        public void CrackHeistCrate()
        {
            if (heistCrateOpened)
            {
                GameManager.Instance?.Announce("Contraband heist crate has already been emptied!");
                return;
            }

            heistCrateOpened = true;
            CameraController.Instance?.TriggerScreenShake(0.3f, 0.35f);
            SoundManager.Instance?.PlaySound(SoundType.CrateCrack, 1.2f);
            SoundManager.Instance?.PlaySound(SoundType.CashChime, 1.0f);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCash(85);
                GameManager.Instance.AddScore(120);
            }
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.currentAmmo = PlayerController.Instance.maxAmmo;
                PlayerController.Instance.currentPaint = PlayerController.Instance.maxPaint;
            }
            GameManager.Instance?.Announce("📦 CONTRABAND HEIST CRACKED! +$85 Cash, +120 Rep, Full Ammo & Paint!");
        }

        public void UseTuningBench()
        {
            if (PlayerController.Instance != null && PlayerController.Instance.calibratedNozzle)
            {
                GameManager.Instance?.Announce("Spray nozzles already precision calibrated!");
                return;
            }

            SoundManager.Instance?.PlaySound(SoundType.HitDeflect, 1.0f);
            PlayerController.Instance?.CalibrateSprayNozzles();
        }

        public void UseAirCompressor()
        {
            SoundManager.Instance?.PlaySound(SoundType.CompressorHiss, 1.1f);
            PlayerController.Instance?.ActivateSuperchargedPaint(40.0f);
        }

        public void RevMotorcycle()
        {
            CameraController.Instance?.TriggerScreenShake(0.25f, 0.3f);
            SoundManager.Instance?.PlaySound(SoundType.EngineRev, 1.2f);
            GameManager.Instance?.CoolHeat(35f);
            GameManager.Instance?.Announce("🏍️ SCRAMBLER BURNOUT! Neon smoke screen blinds pursuers (-1 Star Heat)!");
        }

        public void TagWaterTower()
        {
            if (waterTowerTagged)
            {
                GameManager.Instance?.Announce("Water Tower skyline mural already claimed by your crew!");
                return;
            }

            waterTowerTagged = true;
            CameraController.Instance?.TriggerScreenShake(0.45f, 0.5f);
            SoundManager.Instance?.PlaySound(SoundType.WallClaim, 1.3f);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(500);
                GameManager.Instance.AddCash(150);
            }
            GameManager.Instance?.Announce("👑 SKYLINE MASTER PIECE! Tagged Water Tower Mural! (+500 Rep & +$150)!");
        }

        public int GetActiveCrewCount()
        {
            ActorController[] actors = FindObjectsByType<ActorController>(FindObjectsSortMode.None);
            int count = 0;
            foreach (var a in actors)
            {
                if (a.kind == ActorKind.Crew) count++;
            }
            return count;
        }

        public bool SpawnCrewRecruit(Vector2 position)
        {
            if (crewPrefab == null)
            {
                // Instantiate generic actor if prefab not assigned
                GameObject crewObj = new GameObject("CrewMember");
                crewObj.transform.position = position;
                crewObj.tag = "Crew";
                ActorController actor = crewObj.AddComponent<ActorController>();
                actor.kind = ActorKind.Crew;
                actor.health = 120f;
                actor.moveSpeed = 6.0f;
                CircleCollider2D col = crewObj.AddComponent<CircleCollider2D>();
                col.radius = 0.35f;
                Rigidbody2D rb = crewObj.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                return true;
            }

            GameObject c = Instantiate(crewPrefab, position, Quaternion.identity);
            c.tag = "Crew";
            return true;
        }
    }
}
