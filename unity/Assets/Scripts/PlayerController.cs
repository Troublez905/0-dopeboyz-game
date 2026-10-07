using System.Collections;
using UnityEngine;

namespace Dopeboyz
{
    public enum NozzleType
    {
        MonsterFatCap,
        NeedleCap,
        ChiselTip,
        SoftCap,
        SplatterNozzle,
        FlareNozzle
    }

    public enum SkateboardDeck
    {
        CyberpunkNeon,
        WildstyleGraffiti,
        MidnightSkull,
        GoldenFlame
    }

    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Movement Physics")]
        public float moveSpeed = 8.5f;
        public float sprintMultiplier = 1.45f;
        public float jumpForce = 9.5f;
        public float jetpackForce = 13.5f;
        public float gravity = -22f;
        private Rigidbody rb;
        private Vector3 moveInput;
        private bool isGrounded = false;
        private bool canDoubleJump = true;

        [Header("Evasive Dash")]
        public float dashSpeed = 22f;
        public float dashDuration = 0.22f;
        public float dashCooldown = 0.85f;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector3 dashDirection;

        [Header("Vital Stats")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float maxPaint = 100f;
        public float currentPaint = 100f;
        public float maxStamina = 100f;
        public float currentStamina = 100f;
        public float paintConsumptionRate = 18f;

        [Header("Combat & Weapons")]
        public bool hasWeapon = true;
        public bool paintballSelected;
        public int maxAmmo = 72;
        public int currentAmmo = 48;
        public float fireRate = 0.2f;
        private float fireCooldown = 0f;
        public GameObject projectilePrefab;

        [Header("Aiming & Spraying")]
        public Transform nozzleTransform;
        public ParticleSystem sprayParticles;
        public float sprayRange = 4.2f;
        public Vector3 aimDirection = Vector3.forward;
        public bool isSpraying = false;
        private float spraySfxTimer = 0f;

        [Header("Nozzle Cap System (6 Types)")]
        public NozzleType currentNozzle = NozzleType.MonsterFatCap;
        public string[] nozzleNames = new string[] {
            "Monster Fat Cap (Ultra-Wide)",
            "Needle Cap (Long-Range Laser)",
            "Chisel Tip (High-Impact Heavy)",
            "Soft Cap (Fanning Eco)",
            "Splatter Nozzle (Shotgun Cone)",
            "Flare Nozzle (Incendiary Glow)"
        };

        [Header("Custom Skateboard Decks & Tricks")]
        public SkateboardDeck currentDeck = SkateboardDeck.CyberpunkNeon;
        public int trickComboMultiplier = 1;
        public float trickTimer = 0f;
        public bool isGrinding = false;
        public float grindTimer = 0f;
        private string[] trickNames = new string[] {
            "KICKFLIP",
            "HEELFLIP",
            "360 POP SHUVIT",
            "HARDFLIP",
            "INWARD HEELFLIP",
            "VARIAL KICKFLIP"
        };

        [Header("Aerial Recon Scout Drone (V Key)")]
        public bool reconDroneActive = false;
        public float reconDroneTimer = 0f;
        public GameObject playerReconDroneObj;
        public Light reconDroneSpotlight;

        [Header("Special Abilities & Hardware")]
        public bool bubbleShieldActive = false;
        public float bubbleShieldTimer = 0f;
        public GameObject bubbleShieldVisual;

        public bool slowmoActive = false;
        public float slowmoTimer = 0f;

        public bool neonUnderglowActive = false;
        public GameObject neonUnderglowVisual;

        public bool thermalFoilActive = false;
        public float thermalFoilTimer = 0f;

        public float neonSodaTimer = 0f;
        public float hypeOverdriveTimer = 0f;
        public float superchargedPaintTimer = 0f;

        [Header("Deployable Gadgets")]
        public GameObject turretPrefab;
        public GameObject minePrefab;
        public GameObject flarePrefab;

        [Header("Zipline & Mobility")]
        public bool isZiplining = false;
        public Vector3 ziplineStart;
        public Vector3 ziplineEnd;
        public float ziplineProgress = 0f;

        [Header("Penalties & State")]
        public bool inJail = false;
        public float jailTimer = 0f;
        public bool inHospital = false;
        public float hospitalTimer = 0f;
        public bool carryingDelivery = false;

        [Header("Crew Management")]
        public CrewOrder currentCrewOrder = CrewOrder.Follow;

        private void Awake()
        {
            Instance = this;
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) return;
            if (HandlePenalties()) return;

            HandleInputs();
            HandleTimers();
            HandleZiplineMovement();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) return;
            if (inJail || inHospital || isZiplining)
            {
                if (rb != null) rb.linearVelocity = Vector3.zero;
                return;
            }

            // Ground Check
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.45f);
            if (isGrounded)
            {
                canDoubleJump = true;
                if (isGrinding)
                {
                    isGrinding = false;
                    if (grindTimer > 0.35f)
                    {
                        int grindRep = (int)(grindTimer * 90);
                        GameManager.Instance?.Announce($"🔥 50-50 RAIL GRIND COMPLETED! +{grindRep} REP");
                        GameManager.Instance?.AddScore(grindRep);
                        SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.1f);
                    }
                    grindTimer = 0f;
                }
            }
            else
            {
                // Check Rail Grinding along edges / curbs / rails
                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift))
                {
                    bool nearRail = Physics.CheckSphere(transform.position + Vector3.down * 0.15f, 0.95f, ~0, QueryTriggerInteraction.Ignore);
                    if (nearRail)
                    {
                        isGrinding = true;
                        grindTimer += Time.fixedDeltaTime;
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x * 1.03f, 0f, rb.linearVelocity.z * 1.03f);
                        GameManager.Instance?.AddScore(2);
                    }
                }
            }

            // Handle Movement
            if (dashTimer > 0f)
            {
                dashTimer -= Time.fixedDeltaTime;
                rb.linearVelocity = new Vector3(dashDirection.x * dashSpeed, rb.linearVelocity.y, dashDirection.z * dashSpeed);
            }
            else
            {
                float targetSpeed = moveSpeed;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton8)) targetSpeed *= sprintMultiplier;
                if (neonSodaTimer > 0f) targetSpeed *= 1.35f;

                Vector3 targetVel = moveInput * targetSpeed;
                rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);
            }

            // Rotate character towards aim direction or movement
            Vector3 faceDir = (aimDirection.sqrMagnitude > 0.01f) ? aimDirection : moveInput;
            faceDir.y = 0f;
            if (faceDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(faceDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 14f);
            }
        }

        private void HandleInputs()
        {
            // 1. Movement Inputs (WASD & Gamepad Left Stick)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            var view = Camera.main;
            Vector3 forward = view != null ? Vector3.ProjectOnPlane(view.transform.forward, Vector3.up).normalized : Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            moveInput = Vector3.ClampMagnitude(forward * v + right * h, 1f);
            if (Input.GetKeyDown(KeyCode.R)) paintballSelected = !paintballSelected;

            // 2. Aiming Inputs
            HandleAiming();

            // 3. Jump & Jetpack Double-Jump Thrusters (Space / Gamepad A)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                if (isGrounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                    SoundManager.Instance?.PlaySound(SoundType.Footstep, 0.6f);
                }
                else if (canDoubleJump && currentPaint >= 10f)
                {
                    // Jetpack Thruster Burst!
                    canDoubleJump = false;
                    currentPaint -= 10f;
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jetpackForce, rb.linearVelocity.z);
                    SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.0f);
                    GameManager.Instance?.Announce("🚀 Aerosol Jetpack Thruster Fired!");

                    // Visual burst effect
                    GameObject thrusterFx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    thrusterFx.transform.position = transform.position + Vector3.down * 0.4f;
                    thrusterFx.transform.localScale = Vector3.one * 0.8f;
                    var c = thrusterFx.GetComponent<Collider>();
                    if (c != null) Destroy(c);
                    Destroy(thrusterFx, 0.5f);
                }
                else if (!isGrounded)
                {
                    // Skateboard aerial trick combo!
                    PerformSkateboardTrick();
                }
            }

            // 4. Evasive Dash (Left Alt / Gamepad Left Trigger / Double-Tap)
            if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
            float trigLeft = GetSafeAxis("TriggerAxisLeft");
            if ((Input.GetKeyDown(KeyCode.LeftControl) || trigLeft > 0.6f) && dashCooldownTimer <= 0f && currentStamina >= 15f)
            {
                TriggerDash();
            }

            // 5. Spray Painting (Left Click / Gamepad Right Trigger)
            float trigRight = GetSafeAxis("TriggerAxisRight");
            bool sprayKey = !paintballSelected && (Input.GetMouseButton(0) || trigRight > 0.5f || Input.GetKey(KeyCode.JoystickButton5));
            if (sprayKey && currentPaint > 0f)
            {
                PerformSpraying();
            }
            else
            {
                isSpraying = false;
                if (sprayParticles != null && sprayParticles.isPlaying) sprayParticles.Stop();
            }

            // 6. Paintball Shooting (Right Click / F key / Gamepad RT tap)
            if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;
            if (paintballSelected && Input.GetMouseButton(0) && hasWeapon && fireCooldown <= 0f && currentAmmo > 0)
            {
                ShootPaintball();
            }

            // 7. Nozzle Cap Switching (T key / Gamepad Y)
            if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                CycleNozzleCap();
            }

            // 8. Custom Stencil Stamps (1 to 6 keys / Gamepad X) - Suppressed if Shop open
            bool shopOpen = UIController.Instance != null && UIController.Instance.IsUpgradesOpen;
            if (!shopOpen)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        PlaceStencilStamp(i + 1);
                    }
                }
                if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))
                {
                    PlaceStencilStamp(1);
                }
            }

            // 9. Deployable Paint Turret (N key)
            if (Input.GetKeyDown(KeyCode.N))
            {
                DeployTurret();
            }

            // 9b. Aerial Recon Scout Drone (V key)
            if (Input.GetKeyDown(KeyCode.V))
            {
                DeployOrTriggerReconDrone();
            }

            // 10. Tactical Paint Mine (K or Alt+X)
            if (Input.GetKeyDown(KeyCode.K))
            {
                DeployPaintMine();
            }

            // 11. Pressurized Bubble Shield (U key / Gamepad B)
            if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                ActivateBubbleShield();
            }

            // 12. Adrenaline Slow-Mo Surge (L key / Gamepad LB)
            if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.JoystickButton4))
            {
                ActivateSlowmo();
            }

            // 13. Neon Ground Underglow (J or O key)
            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.O))
            {
                ToggleNeonUnderglow();
            }

            // 14. Thermal Stealth Foil Blanket (B key)
            if (Input.GetKeyDown(KeyCode.B))
            {
                ActivateThermalFoil();
            }

            // 15. Skateboard Deck Switch (G key)
            if (Input.GetKeyDown(KeyCode.G))
            {
                CycleSkateboardDeck();
            }

            // 16. Turf War Frontline Flare (Y key)
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ThrowFrontlineFlare();
            }

            // 17. Squad Orders (Q key)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CycleCrewOrders();
            }

            // 18. Contracts & Upgrades (C key & TAB key)
            if (Input.GetKeyDown(KeyCode.C))
            {
                RequestStreetContract();
            }
        }

        private void HandleAiming()
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f,.5f,0));
                Vector3 point = ray.GetPoint(50); float nearest = 50;
                foreach (var hit in Physics.RaycastAll(ray,50,~0,QueryTriggerInteraction.Ignore))
                    if (!hit.transform.IsChildOf(transform) && hit.distance < nearest) { nearest = hit.distance; point = hit.point; }
                Vector3 origin = nozzleTransform != null ? nozzleTransform.position : transform.position + Vector3.up;
                aimDirection = (point-origin).normalized;
                return;
            }
            // Gamepad Right Thumbstick
            float rsX = GetSafeAxis("RightStickHorizontal");
            float rsY = GetSafeAxis("RightStickVertical");
            if (Mathf.Abs(rsX) > 0.2f || Mathf.Abs(rsY) > 0.2f)
            {
                aimDirection = new Vector3(rsX, 0f, rsY).normalized;
                return;
            }

            // Mouse Raycast onto Ground Plane
            Camera mainCam = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
            if (mainCam != null)
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, transform.position);
                float enter;
                if (groundPlane.Raycast(ray, out enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    Vector3 dir = (hitPoint - transform.position);
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.01f)
                    {
                        aimDirection = dir.normalized;
                    }
                }
            }
        }

        private float GetSafeAxis(string axisName)
        {
            var pad = UnityEngine.InputSystem.Gamepad.current;
            if (pad == null) return 0f;
            switch (axisName)
            {
                case "TriggerAxisLeft": return pad.leftTrigger.ReadValue();
                case "TriggerAxisRight": return pad.rightTrigger.ReadValue();
                case "RightStickHorizontal": return pad.rightStick.x.ReadValue();
                case "RightStickVertical": return pad.rightStick.y.ReadValue();
                default: return 0f;
            }
        }

        private void TriggerDash()
        {
            currentStamina -= 15f;
            dashCooldownTimer = dashCooldown;
            dashTimer = dashDuration;
            dashDirection = (moveInput.sqrMagnitude > 0.01f) ? moveInput : aimDirection;

            // Skateboard trick combo bonus!
            trickTimer = 2.5f;
            trickComboMultiplier = Mathf.Min(trickComboMultiplier + 1, 5);
            GameManager.Instance?.AddScore(15 * trickComboMultiplier);

            SoundManager.Instance?.PlaySound(SoundType.Whoosh, 0.9f);
        }

        private void PerformSpraying()
        {
            isSpraying = true;
            if (sprayParticles != null && !sprayParticles.isPlaying) sprayParticles.Play();

            float sprayCost = paintConsumptionRate * Time.deltaTime;
            if (currentNozzle == NozzleType.SoftCap) sprayCost *= 0.6f;
            else if (currentNozzle == NozzleType.MonsterFatCap) sprayCost *= 1.4f;

            currentPaint = Mathf.Max(0f, currentPaint - sprayCost);

            // Spray SFX
            spraySfxTimer -= Time.deltaTime;
            if (spraySfxTimer <= 0f)
            {
                spraySfxTimer = 0.22f;
                SoundManager.Instance?.PlaySound(SoundType.SprayCan, 0.5f);
            }

            // Check if aiming at any WallTag in 3D range
            RaycastHit hit;
            Vector3 origin = (nozzleTransform != null) ? nozzleTransform.position : transform.position + Vector3.up * 1.0f;
            float effRange = sprayRange;
            if (currentNozzle == NozzleType.NeedleCap) effRange *= 1.8f;

            if (Physics.Raycast(origin, aimDirection, out hit, effRange))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    var tag = hit.collider.GetComponent<WallTag>();
                    if (tag != null)
                    {
                        float rate = tag.sprayRate * Time.deltaTime * (1f + .12f * (CampaignManager.Instance?.GetUpgradeRank("paint") ?? 0));
                        if (currentNozzle == NozzleType.MonsterFatCap) rate *= 1.85f;
                        if (superchargedPaintTimer > 0f) rate *= 2.0f;

                        tag.ApplySpray("crew", rate);
                    }
                }
            }
        }

        private void ShootPaintball()
        {
            fireCooldown = fireRate;
            currentAmmo--;

            Vector3 spawnPos = (nozzleTransform != null) ? nozzleTransform.position : transform.position + Vector3.up * 1.0f + aimDirection * 0.6f;

            GameObject proj = null;
            if (projectilePrefab != null)
            {
                proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(aimDirection));
            }
            else
            {
                proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                proj.transform.position = spawnPos;
                proj.transform.localScale = Vector3.one * 0.35f;
                var col = proj.GetComponent<SphereCollider>();
                if (col != null) col.isTrigger = true;
                var pRb = proj.AddComponent<Rigidbody>();
                pRb.useGravity = false;
            }

            var pComp = proj.GetComponent<Projectile>();
            if (pComp == null) pComp = proj.AddComponent<Projectile>();

            float dmg = 25f;
            if (currentNozzle == NozzleType.ChiselTip) dmg = 42f;

            dmg += 6f * (CampaignManager.Instance?.GetUpgradeRank("gear") ?? 0);
            pComp.Initialize("Player", aimDirection, dmg, 26f);
            SoundManager.Instance?.PlaySound(SoundType.ShootPaint, 0.7f);
        }

        public void CycleNozzleCap()
        {
            int next = ((int)currentNozzle + 1) % 6;
            currentNozzle = (NozzleType)next;
            SoundManager.Instance?.PlaySound(SoundType.SprayCapSelect);
            GameManager.Instance?.Announce($"🎨 Equipped Nozzle: {nozzleNames[next]}");
        }

        public void CycleSkateboardDeck()
        {
            int next = ((int)currentDeck + 1) % 4;
            currentDeck = (SkateboardDeck)next;
            SoundManager.Instance?.PlaySound(SoundType.Whoosh, 0.7f);
            GameManager.Instance?.Announce($"🛹 Switched Deck: {currentDeck}");
        }

        public void PlaceStencilStamp(int stencilIndex)
        {
            if (currentPaint < 8f) return;
            currentPaint -= 8f;

            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 1.0f;
            if (Physics.Raycast(origin, aimDirection, out hit, 4.0f))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    var tag = hit.collider.GetComponent<WallTag>();
                    if (tag != null)
                    {
                        tag.ApplyInstantStencil("crew");
                        SoundManager.Instance?.PlaySound(SoundType.WallComplete);
                        GameManager.Instance?.Announce($"👑 Applied Stencil #{stencilIndex} to {tag.spotName}!");
                    }
                }
            }
        }

        public void DeployTurret()
        {
            if (currentPaint < 25f)
            {
                GameManager.Instance?.Announce("⚠️ Need at least 25 Paint to deploy Turret!");
                return;
            }
            currentPaint -= 25f;

            Vector3 spawnPos = transform.position + transform.forward * 1.5f;
            spawnPos.y = 0.5f;

            GameObject turret = null;
            if (turretPrefab != null)
            {
                turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                turret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                turret.transform.position = spawnPos;
                turret.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                turret.tag = "Crew";
                var tComp = turret.AddComponent<DeployableTurret>();
                tComp.projectilePrefab = projectilePrefab;
            }

            SoundManager.Instance?.PlaySound(SoundType.LevelUp);
            GameManager.Instance?.Announce("🤖 Sentry Paint Turret Deployed!");
        }

        public void DeployPaintMine()
        {
            if (currentPaint < 15f) return;
            currentPaint -= 15f;

            Vector3 spawnPos = transform.position;
            spawnPos.y = 0.1f;

            GameObject mine = null;
            if (minePrefab != null)
            {
                mine = Instantiate(minePrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                mine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                mine.transform.position = spawnPos;
                mine.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
                var col = mine.GetComponent<Collider>();
                if (col != null) col.isTrigger = true;
                mine.AddComponent<PaintMine>();
            }

            SoundManager.Instance?.PlaySound(SoundType.Footstep);
            GameManager.Instance?.Announce("💣 Tactical Paint Splatter Mine Armed!");
        }

        public void ActivateBubbleShield()
        {
            if (bubbleShieldActive) return;
            bubbleShieldActive = true;
            bubbleShieldTimer = 4.5f;
            if (bubbleShieldVisual != null) bubbleShieldVisual.SetActive(true);

            SoundManager.Instance?.PlaySound(SoundType.BubbleShield);
            GameManager.Instance?.Announce("🫧 Pressurized Bubble Shield Activated (4.5s)!");
        }

        public void ActivateSlowmo()
        {
            if (slowmoActive) return;
            slowmoActive = true;
            slowmoTimer = 6.0f;
            Time.timeScale = 0.5f;

            SoundManager.Instance?.PlaySound(SoundType.SlowMo);
            GameManager.Instance?.Announce("⚡ Adrenaline Slow-Mo Surge Triggered (6s)!");
        }

        public void ToggleNeonUnderglow()
        {
            neonUnderglowActive = !neonUnderglowActive;
            if (neonUnderglowVisual != null) neonUnderglowVisual.SetActive(neonUnderglowActive);
            SoundManager.Instance?.PlaySound(SoundType.SprayCapSelect);
            GameManager.Instance?.Announce(neonUnderglowActive ? "✨ Skateboard Neon Ground Underglow: ON" : "Skateboard Neon Ground Underglow: OFF");
        }

        public void ActivateThermalFoil()
        {
            thermalFoilActive = true;
            thermalFoilTimer = 8.0f;
            SoundManager.Instance?.PlaySound(SoundType.Whoosh);
            GameManager.Instance?.Announce("🕶️ Thermal Stealth Foil Deployed! Chopper radar scrambled!");
        }

        public void ThrowFrontlineFlare()
        {
            Vector3 spawnPos = transform.position + transform.forward * 2.0f;
            spawnPos.y = 0.2f;

            GameObject flare = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            flare.transform.position = spawnPos;
            flare.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);
            var ren = flare.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = Color.cyan;
                ren.material = m;
            }
            Destroy(flare, 25.0f);

            // Summon crew reinforcements
            if (CityController.Instance != null)
            {
                CityController.Instance.SpawnCrewRecruit(spawnPos + Vector3.right * 1.5f);
            }
            GameManager.Instance?.Announce("🔥 Frontline Turf Flare Burned! Crew backup incoming!");
        }

        public void StartZipline(Vector3 start, Vector3 end)
        {
            isZiplining = true;
            ziplineStart = start;
            ziplineEnd = end;
            ziplineProgress = 0f;
            SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.0f);
            GameManager.Instance?.Announce("⚡ Grinding Rooftop Cable Zipline!");
        }

        private void HandleZiplineMovement()
        {
            if (!isZiplining) return;

            ziplineProgress += Time.deltaTime * 0.85f;
            transform.position = Vector3.Lerp(ziplineStart, ziplineEnd, ziplineProgress);

            if (ziplineProgress >= 1.0f)
            {
                isZiplining = false;
                rb.linearVelocity = (ziplineEnd - ziplineStart).normalized * 8.0f;
            }
        }

        private void CycleCrewOrders()
        {
            int next = ((int)currentCrewOrder + 1) % 3;
            currentCrewOrder = (CrewOrder)next;

            ActorController[] crewMembers = Object.FindObjectsByType<ActorController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var member in crewMembers)
            {
                if (member.kind == ActorKind.Crew)
                {
                    member.crewOrder = currentCrewOrder;
                    member.guardPoint = transform.position;
                }
            }

            SoundManager.Instance?.PlaySound(SoundType.CrewOrder);
            GameManager.Instance?.Announce($"📢 Crew Squad Order: {currentCrewOrder.ToString().ToUpper()}");
        }

        public void TakeDamage(float dmg, Vector3 hitDirection)
        {
            if (dashTimer > 0f) return; // Invulnerable during evasive dash

            if (bubbleShieldActive)
            {
                SoundManager.Instance?.PlaySound(SoundType.ShieldHit);
                return;
            }

            currentHealth -= dmg * (1f - .08f * (CampaignManager.Instance?.GetUpgradeRank("gear") ?? 0));
            GameSession.Instance?.Notify("Taking fire / Find cover");
            CameraController.Instance?.TriggerScreenShake(0.2f, 0.15f);

            if (currentHealth <= 0f)
            {
                TriggerHospital();
            }
        }

        private void TriggerHospital()
        {
            if (GameSession.Instance != null) { GameSession.Instance.Recover(); return; }
            inHospital = true;
            hospitalTimer = 10f;
            currentHealth = maxHealth;
            transform.position = new Vector3(8.55f, 0.5f, 8.20f); // Spawn at shop/hospital
            GameManager.Instance?.AddHeat(-40f);
            GameManager.Instance?.Announce("🚑 Wiped out! Transported to District Medical Center.");
        }

        public void TriggerJail()
        {
            inJail = true;
            jailTimer = 15f;
            transform.position = new Vector3(15.70f, 0.5f, 18.00f); // Jail cell
            currentPaint = 0f;
            currentAmmo = 0;
            GameManager.Instance?.AddHeat(-100f);
            GameManager.Instance?.Announce("🚨 BUSTED BY POLICE! Sent to City Precinct Jail.");
        }

        private bool HandlePenalties()
        {
            if (inJail)
            {
                jailTimer -= Time.deltaTime;
                if (jailTimer <= 0f)
                {
                    inJail = false;
                    currentPaint = 40f;
                    currentAmmo = 24;
                    GameManager.Instance?.Announce("🔓 Released from Jail on street probation.");
                }
                return true;
            }

            if (inHospital)
            {
                hospitalTimer -= Time.deltaTime;
                if (hospitalTimer <= 0f)
                {
                    inHospital = false;
                    GameManager.Instance?.Announce("❤️ Discharged from Medical Center. Ready to skate!");
                }
                return true;
            }
            return false;
        }

        private void HandleTimers()
        {
            if (bubbleShieldActive)
            {
                bubbleShieldTimer -= Time.deltaTime;
                if (bubbleShieldTimer <= 0f)
                {
                    bubbleShieldActive = false;
                    if (bubbleShieldVisual != null) bubbleShieldVisual.SetActive(false);
                }
            }

            if (slowmoActive)
            {
                slowmoTimer -= Time.deltaTime;
                if (slowmoTimer <= 0f)
                {
                    slowmoActive = false;
                    Time.timeScale = 1.0f;
                }
            }

            if (thermalFoilActive)
            {
                thermalFoilTimer -= Time.deltaTime;
                if (thermalFoilTimer <= 0f) thermalFoilActive = false;
            }

            if (neonSodaTimer > 0f) neonSodaTimer -= Time.deltaTime;
            if (hypeOverdriveTimer > 0f) hypeOverdriveTimer -= Time.deltaTime;
            if (superchargedPaintTimer > 0f) superchargedPaintTimer -= Time.deltaTime;

            if (trickTimer > 0f)
            {
                trickTimer -= Time.deltaTime;
                if (trickTimer <= 0f) trickComboMultiplier = 1;
            }

            // Aerial Recon Drone tracking & lifetime
            if (reconDroneActive)
            {
                reconDroneTimer -= Time.deltaTime;
                if (playerReconDroneObj != null)
                {
                    Vector3 targetPos = transform.position + Vector3.up * 5.8f + Vector3.forward * 1.2f;
                    targetPos.y += Mathf.Sin(Time.time * 2.5f) * 0.35f;
                    playerReconDroneObj.transform.position = Vector3.Lerp(playerReconDroneObj.transform.position, targetPos, Time.deltaTime * 4f);
                    playerReconDroneObj.transform.Rotate(0f, 60f * Time.deltaTime, 0f);
                }
                if (reconDroneTimer <= 0f)
                {
                    reconDroneActive = false;
                    if (playerReconDroneObj != null) Destroy(playerReconDroneObj);
                    GameManager.Instance?.Announce("🛸 Recon Drone battery depleted. Returned to base.");
                }
            }

            // Stamina regeneration
            if (currentStamina < maxStamina)
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + 12f * Time.deltaTime);
            }
        }

        public void DeployOrTriggerReconDrone()
        {
            if (!reconDroneActive)
            {
                reconDroneActive = true;
                reconDroneTimer = 18f;

                playerReconDroneObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                playerReconDroneObj.name = "Player_Recon_Drone";
                playerReconDroneObj.transform.position = transform.position + Vector3.up * 5.5f;
                playerReconDroneObj.transform.localScale = new Vector3(0.9f, 0.2f, 0.9f);
                var c = playerReconDroneObj.GetComponent<Collider>();
                if (c != null) Destroy(c);

                var ren = playerReconDroneObj.GetComponent<Renderer>();
                if (ren != null)
                {
                    Material m = new Material(Shader.Find("Standard"));
                    m.color = new Color(0.1f, 0.15f, 0.25f);
                    m.SetFloat("_Glossiness", 0.9f);
                    ren.material = m;
                }

                // Spotlight
                GameObject spotObj = new GameObject("DroneSearchlight");
                spotObj.transform.SetParent(playerReconDroneObj.transform, false);
                spotObj.transform.localPosition = Vector3.zero;
                spotObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                reconDroneSpotlight = spotObj.AddComponent<Light>();
                reconDroneSpotlight.type = LightType.Spot;
                reconDroneSpotlight.color = new Color(0.3f, 0.96f, 0.84f);
                reconDroneSpotlight.spotAngle = 60f;
                reconDroneSpotlight.range = 25f;
                reconDroneSpotlight.intensity = 5.0f;

                SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.2f);

                // Scan 45m radius
                WallTag closestWall = null;
                float closestDist = 999f;
                if (CityController.Instance != null && CityController.Instance.districtWalls != null)
                {
                    foreach (var w in CityController.Instance.districtWalls)
                    {
                        if (w != null && w.owner != WallOwner.Crew)
                        {
                            float d = Vector3.Distance(transform.position, w.transform.position);
                            if (d < closestDist)
                            {
                                closestDist = d;
                                closestWall = w;
                            }
                        }
                    }
                }

                if (closestWall != null)
                {
                    GameManager.Instance?.Announce($"🛸 Recon Drone Active! Nearest unpainted wall: {closestWall.spotName} ({closestDist:F0}m)");
                }
                else
                {
                    GameManager.Instance?.Announce("🛸 Recon Drone Deployed (18s)! Press [V] again to drop Paint Cluster Bomb!");
                }
            }
            else
            {
                // Drop Paint Cluster Bomb!
                DropClusterPaintBomb();
            }
        }

        public void DropClusterPaintBomb()
        {
            if (playerReconDroneObj == null) return;

            Vector3 dropPos = playerReconDroneObj.transform.position;
            GameObject bomb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bomb.name = "ClusterPaintBomb";
            bomb.transform.position = dropPos;
            bomb.transform.localScale = Vector3.one * 0.7f;
            var c = bomb.GetComponent<Collider>();
            if (c != null) Destroy(c);

            var ren = bomb.GetComponent<Renderer>();
            if (ren != null)
            {
                Material bm = new Material(Shader.Find("Standard"));
                bm.color = new Color(0.3f, 0.96f, 0.84f);
                ren.material = bm;
            }

            var bRb = bomb.AddComponent<Rigidbody>();
            bRb.linearVelocity = Vector3.down * 15f;

            StartCoroutine(ClusterBombExplosionSequence(bomb));

            reconDroneActive = false;
            reconDroneTimer = 0f;
            Destroy(playerReconDroneObj, 0.2f);
        }

        private IEnumerator ClusterBombExplosionSequence(GameObject bomb)
        {
            yield return new WaitForSeconds(0.45f);
            if (bomb != null)
            {
                Vector3 expPos = bomb.transform.position;
                Destroy(bomb);

                // Splash sphere visual
                GameObject exp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                exp.transform.position = expPos;
                exp.transform.localScale = Vector3.one * 7.5f;
                var expCol = exp.GetComponent<Collider>();
                if (expCol != null) Destroy(expCol);
                var expRen = exp.GetComponent<Renderer>();
                if (expRen != null)
                {
                    Material em = new Material(Shader.Find("Standard"));
                    em.color = new Color(0.2f, 1.0f, 0.7f, 0.5f);
                    expRen.material = em;
                }
                Destroy(exp, 0.6f);

                // Damage enemies & paint walls
                Collider[] hits = Physics.OverlapSphere(expPos, 8.5f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Wall"))
                    {
                        var wall = hit.GetComponent<WallTag>();
                        if (wall != null) wall.ApplyInstantStencil("crew");
                    }
                    else if (hit.CompareTag("Enemy") || hit.CompareTag("Police"))
                    {
                        var act = hit.GetComponent<ActorController>();
                        if (act != null) act.TakeDamage(55f, (act.transform.position - transform.position).normalized);
                    }
                }

                SoundManager.Instance?.PlaySound(SoundType.Explosion, 1.2f);
                CameraController.Instance?.TriggerScreenShake(0.3f, 0.25f);
                GameManager.Instance?.Announce("💥 CLUSTER PAINT BOMB DETONATED! Massive territory claimed!");
            }
        }

        public void PerformSkateboardTrick(string trickName = null)
        {
            if (trickName == null)
            {
                int trickIdx = Random.Range(0, trickNames.Length);
                trickName = trickNames[trickIdx];
            }
            trickTimer = 3.0f;
            trickComboMultiplier = Mathf.Min(trickComboMultiplier + 1, 6);
            int repReward = 50 * trickComboMultiplier;
            GameManager.Instance?.AddScore(repReward);
            GameManager.Instance?.AddHeat(-5f); // Good tricks lower police heat
            SoundManager.Instance?.PlaySound(SoundType.Whoosh, 1.2f);
            GameManager.Instance?.Announce($"🛹 {trickName}! +{repReward} REP (x{trickComboMultiplier} COMBO)");
            CameraController.Instance?.TriggerScreenShake(0.12f, 0.08f);
        }

        private void RequestStreetContract()
        {
            if (CampaignManager.Instance != null)
            {
                CampaignManager.Instance.RequestContract();
            }
        }

        private void ToggleUpgradesPanel()
        {
            if (UIController.Instance != null)
            {
                UIController.Instance.ToggleUpgradesPanel();
            }
        }
    }
}
