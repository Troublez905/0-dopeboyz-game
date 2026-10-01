using UnityEngine;

namespace Dopeboyz
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Movement Physics")]
        public float moveSpeed = 8.5f;
        public float sprintMultiplier = 1.4f;
        public float acceleration = 25f;
        public float friction = 20f;
        private Rigidbody2D rb;
        private Vector2 moveInput;
        private Vector2 currentVelocity;

        [Header("Evasive Dash")]
        public float dashSpeed = 22f;
        public float dashDuration = 0.22f;
        public float dashCooldown = 1.0f;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector2 dashDirection;

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
        public int maxAmmo = 72;
        public int currentAmmo = 48;
        public float fireRate = 0.2f;
        private float fireCooldown = 0f;
        public GameObject projectilePrefab;

        [Header("Penalties & State")]
        public bool inJail = false;
        public float jailTimer = 0f;
        public bool inHospital = false;
        public float hospitalTimer = 0f;
        public bool carryingDelivery = false;

        [Header("Aiming & Spraying")]
        public Transform nozzleTransform;
        public ParticleSystem sprayParticles;
        public LineRenderer sprayStreamLine;
        public float sprayRange = 3.5f;
        private Vector2 aimDirection = Vector2.right;
        public bool isSpraying = false;
        private float spraySfxTimer = 0f;

        [Header("Abilities")]
        public bool bubbleShieldActive = false;
        public float bubbleShieldTimer = 0f;
        public GameObject bubbleShieldVisual;

        public bool slowmoActive = false;
        public float slowmoTimer = 0f;

        public bool neonUnderglowActive = false;
        public GameObject neonUnderglowVisual;

        public float neonSodaTimer = 0f;

        [Header("Landmark & Temporary Buffs")]
        public float hypeOverdriveTimer = 0f;
        public float superchargedPaintTimer = 0f;
        public bool calibratedNozzle = false;

        [Header("Crew Management")]
        public CrewOrder currentCrewOrder = CrewOrder.Follow;

        private void Awake()
        {
            Instance = this;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (HandlePenalties()) return;

            HandleInputs();
            HandleTimers();
        }

        private void FixedUpdate()
        {
            if (inJail || inHospital)
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
                return;
            }
            ApplyMovement();
        }

        private bool HandlePenalties()
        {
            if (inJail)
            {
                jailTimer -= Time.deltaTime;
                if (jailTimer <= 0f)
                {
                    inJail = false;
                    transform.position = CityController.Instance != null ? CityController.Instance.hqPosition : new Vector2(3.1f, 5.2f);
                    GameManager.Instance?.Announce("Released from Police Custody. Back on the block!");
                }
                return true;
            }

            if (inHospital)
            {
                hospitalTimer -= Time.deltaTime;
                if (hospitalTimer <= 0f)
                {
                    inHospital = false;
                    currentHealth = maxHealth;
                    transform.position = CityController.Instance != null ? CityController.Instance.hqPosition : new Vector2(3.1f, 5.2f);
                    GameManager.Instance?.Announce("Discharged from Hospital. Fully healed!");
                }
                return true;
            }

            return false;
        }

        private void HandleInputs()
        {
            // 1. Movement Inputs
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(moveX, moveY).normalized;

            // 2. Aiming
            float lookX = Input.GetAxisRaw("RightStickX");
            float lookY = Input.GetAxisRaw("RightStickY");
            if (Mathf.Abs(lookX) > 0.2f || Mathf.Abs(lookY) > 0.2f)
            {
                aimDirection = new Vector2(lookX, lookY).normalized;
            }
            else
            {
                if (Camera.main != null)
                {
                    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    aimDirection = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
                }
            }

            if (nozzleTransform != null)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                nozzleTransform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // 3. Evasive Dash
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) && dashCooldownTimer <= 0f && moveInput.sqrMagnitude > 0.05f)
            {
                StartDash(moveInput);
            }

            // 4. Spray Wall (Left Click or Right Trigger)
            bool sprayTrigger = Input.GetMouseButton(0) || Input.GetAxisRaw("RightTrigger") > 0.3f;
            if (sprayTrigger && currentPaint > 0f)
            {
                PerformSpray();
            }
            else
            {
                StopSpray();
            }

            // 5. Fire Weapon (Right Click, Left Trigger, or F Key)
            bool fireTrigger = Input.GetMouseButtonDown(1) || Input.GetAxisRaw("LeftTrigger") > 0.5f || Input.GetKeyDown(KeyCode.F);
            if (fireTrigger && fireCooldown <= 0f)
            {
                FireWeapon(Input.GetKeyDown(KeyCode.F));
            }

            // 6. Special Abilities
            if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                ActivateBubbleShield();
            }

            if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.JoystickButton4))
            {
                ActivateSlowmo();
            }

            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.O))
            {
                ToggleNeonUnderglow();
            }

            // 7. Cycle Crew Orders (Q key)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CycleCrewOrders();
            }

            // 8. Accept / Refresh Street Contract (C key)
            if (Input.GetKeyDown(KeyCode.C))
            {
                AcceptOrCycleContract();
            }

            // 9. Deliver Shipment at HQ (E key near HQ)
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckHQInteraction();
            }

            // 10. Stencil Stamps (Keys 1-6)
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                {
                    ApplyStencilStamp();
                    break;
                }
            }
        }

        private void ApplyMovement()
        {
            if (dashTimer > 0f)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
                return;
            }

            float currentSpeed = moveSpeed;
            if (carryingDelivery) currentSpeed *= 0.88f; // 12% slowdown carrying heavy contraband
            if (hypeOverdriveTimer > 0f)
            {
                currentSpeed *= 1.8f;
                currentStamina = maxStamina;
            }
            else if (neonSodaTimer > 0f || Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= sprintMultiplier;
            }

            Vector2 targetVelocity = moveInput * currentSpeed;
            currentVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, (moveInput.sqrMagnitude > 0.01f ? acceleration : friction) * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;
        }

        private void StartDash(Vector2 dir)
        {
            dashDirection = dir;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            SoundManager.Instance?.PlaySound(SoundType.Dash);
            CameraController.Instance?.TriggerScreenShake(0.15f, 0.15f);
        }

        private void PerformSpray()
        {
            isSpraying = true;
            currentPaint = Mathf.Max(0f, currentPaint - paintConsumptionRate * Time.deltaTime);

            spraySfxTimer -= Time.deltaTime;
            if (spraySfxTimer <= 0f)
            {
                SoundManager.Instance?.PlaySound(SoundType.SprayHiss, 0.4f);
                spraySfxTimer = 0.25f;
            }

            if (sprayParticles != null && !sprayParticles.isPlaying)
            {
                sprayParticles.Play();
            }

            float effectiveRange = calibratedNozzle ? sprayRange * 1.35f : sprayRange;
            float sprayRate = calibratedNozzle ? 0.65f : 0.4f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, effectiveRange);
            if (hit.collider != null)
            {
                WallTag wall = hit.collider.GetComponent<WallTag>();
                if (wall != null)
                {
                    wall.ApplySpray("crew", sprayRate * Time.deltaTime);
                }
            }
        }

        private void StopSpray()
        {
            isSpraying = false;
            if (sprayParticles != null && sprayParticles.isPlaying)
            {
                sprayParticles.Stop();
            }
        }

        private void FireWeapon(bool autoTargetNearest)
        {
            if (!hasWeapon || currentAmmo <= 0)
            {
                GameManager.Instance?.Announce("Out of ammunition! Collect ammo crates on the street.");
                return;
            }

            currentAmmo--;
            fireCooldown = fireRate;
            SoundManager.Instance?.PlaySound(SoundType.Shoot);
            CameraController.Instance?.TriggerScreenShake(0.08f, 0.1f);
            GameManager.Instance?.AddHeat(1.5f);

            Vector2 shootDir = aimDirection;
            if (autoTargetNearest)
            {
                ActorController nearest = FindNearestTarget();
                if (nearest != null)
                {
                    shootDir = ((Vector2)nearest.transform.position - (Vector2)transform.position).normalized;
                }
            }

            Vector3 spawnPos = nozzleTransform != null ? nozzleTransform.position : transform.position + (Vector3)shootDir * 0.5f;

            float dmg = superchargedPaintTimer > 0f ? 38f : 26f;
            float spd = superchargedPaintTimer > 0f ? 28f : 18f;

            if (projectilePrefab != null)
            {
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                Projectile proj = projObj.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Initialize("Player", shootDir, dmg, spd);
                }
            }
            else
            {
                // Fallback hitscan raycast if prefab not assigned
                RaycastHit2D hit = Physics2D.Raycast(spawnPos, shootDir, 16f);
                if (hit.collider != null)
                {
                    ActorController actor = hit.collider.GetComponent<ActorController>();
                    if (actor != null && (actor.kind == ActorKind.Rival || actor.kind == ActorKind.Cop || actor.kind == ActorKind.SwatRiot))
                    {
                        actor.TakeDamage(26f, shootDir);
                    }
                }
            }
        }

        private ActorController FindNearestTarget()
        {
            ActorController[] actors = FindObjectsByType<ActorController>(FindObjectsSortMode.None);
            ActorController best = null;
            float minDist = 15f;
            foreach (var a in actors)
            {
                if (a.kind == ActorKind.Rival || a.kind == ActorKind.Cop || a.kind == ActorKind.SwatRiot)
                {
                    float d = Vector2.Distance(transform.position, a.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        best = a;
                    }
                }
            }
            return best;
        }

        public void TakeDamage(float amount, Vector2 hitDirection)
        {
            if (bubbleShieldActive)
            {
                SoundManager.Instance?.PlaySound(SoundType.HitDeflect);
                GameManager.Instance?.Announce("🛡️ Shot deflected by Bubble Shield!");
                return;
            }

            currentHealth -= amount;
            rb.AddForce(hitDirection * 180f, ForceMode2D.Impulse);
            CameraController.Instance?.TriggerScreenShake(0.25f, 0.2f);
            SoundManager.Instance?.PlaySound(SoundType.HitDeflect, 0.7f);

            if (currentHealth <= 0f)
            {
                SendToHospital();
            }
        }

        public void ArrestByPolice()
        {
            if (inJail || bubbleShieldActive) return;
            inJail = true;
            jailTimer = 11.0f;
            currentPaint = 0f;
            currentAmmo = 0;
            hasWeapon = false;
            carryingDelivery = false;
            GameManager.Instance?.CoolHeat(60f);
            GameManager.Instance?.Announce("🚨 BUSTED! Sent to jail for 11 seconds. Contraband confiscated!");
        }

        private void SendToHospital()
        {
            inHospital = true;
            hospitalTimer = 8.0f;
            carryingDelivery = false;
            int fee = Mathf.Min(GameManager.Instance != null ? GameManager.Instance.cash : 0, 35);
            GameManager.Instance?.SpendCash(fee);
            GameManager.Instance?.Announce($"🏥 Flatlined! Rushed to hospital for 8 seconds. Medical bill: ${fee}");
        }

        public void CycleCrewOrders()
        {
            currentCrewOrder = (CrewOrder)(((int)currentCrewOrder + 1) % 3);
            ActorController[] actors = FindObjectsByType<ActorController>(FindObjectsSortMode.None);
            foreach (var a in actors)
            {
                if (a.kind == ActorKind.Crew)
                {
                    a.crewOrder = currentCrewOrder;
                    if (currentCrewOrder == CrewOrder.Guard)
                    {
                        a.guardPoint = transform.position;
                    }
                }
            }
            GameManager.Instance?.Announce($"📢 Crew Orders set to: {currentCrewOrder.ToString().ToUpper()}");
        }

        public void AcceptOrCycleContract()
        {
            if (CityController.Instance == null || CityController.Instance.districtWalls.Count == 0) return;

            // Pick a non-owned wall
            WallTag target = null;
            foreach (var w in CityController.Instance.districtWalls)
            {
                if (w.owner != WallOwner.Crew)
                {
                    target = w;
                    break;
                }
            }

            if (target != null)
            {
                GameManager.Instance.currentContractWall = target.spotName;
                GameManager.Instance.contractActive = true;
                GameManager.Instance.Announce($"🎯 CONTRACT ACCEPTED: Tag '{target.spotName}' for +${GameManager.Instance.contractReward} & +60 Rep!");
            }
            else
            {
                GameManager.Instance?.Announce("All walls currently held by your crew! Waiting for rival incursion.");
            }
        }

        private void CheckHQInteraction()
        {
            if (CityController.Instance == null) return;
            if (Vector2.Distance(transform.position, CityController.Instance.hqPosition) < 3.5f)
            {
                if (carryingDelivery)
                {
                    carryingDelivery = false;
                    GameManager.Instance?.AddCash(100);
                    GameManager.Instance?.AddScore(75);
                    SoundManager.Instance?.PlaySound(SoundType.CashChime);
                    GameManager.Instance?.Announce("📦 Contraband shipment delivered to HQ! +$100 & +75 Rep!");
                    if (CampaignManager.Instance != null) CampaignManager.Instance.hasActiveDelivery = false;
                }
                else
                {
                    // Stash deposit
                    CampaignManager.Instance?.DepositStash(40f);
                }
            }
            else if (Vector2.Distance(transform.position, CityController.Instance.shopPosition) < 3.5f)
            {
                // Shop pickup for delivery
                if (!carryingDelivery)
                {
                    carryingDelivery = true;
                    GameManager.Instance?.Announce("🚚 Picked up contraband delivery! Bring it to HQ!");
                }
            }
        }

        public void ActivateBubbleShield()
        {
            if (bubbleShieldTimer > 0f || currentPaint < 20f) return;
            currentPaint -= 20f;
            bubbleShieldTimer = 4.5f;
            bubbleShieldActive = true;
            if (bubbleShieldVisual != null) bubbleShieldVisual.SetActive(true);
            SoundManager.Instance?.PlaySound(SoundType.HitDeflect);
            GameManager.Instance?.Announce("🛡️ Pressurized Bubble Shield Active (4.5s)!");
        }

        public void ActivateSlowmo()
        {
            if (slowmoTimer > 0f) return;
            slowmoTimer = 6.0f;
            slowmoActive = true;
            Time.timeScale = 0.5f;
            SoundManager.Instance?.PlaySound(SoundType.SlowmoWarp);
            GameManager.Instance?.Announce("⚡ Adrenaline Slow-Mo Focus Enabled (6s)!");
        }

        public void ToggleNeonUnderglow()
        {
            neonUnderglowActive = !neonUnderglowActive;
            if (neonUnderglowVisual != null) neonUnderglowVisual.SetActive(neonUnderglowActive);
            GameManager.Instance?.Announce($"✨ Neon Ground Underglow: {(neonUnderglowActive ? "ON" : "OFF")}");
        }

        public void ApplyStencilStamp()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, sprayRange);
            if (hit.collider != null)
            {
                WallTag wall = hit.collider.GetComponent<WallTag>();
                if (wall != null && currentPaint >= 10f)
                {
                    currentPaint -= 10f;
                    wall.ApplyInstantStencil("crew");
                    SoundManager.Instance?.PlaySound(SoundType.WallClaim);
                    GameManager.Instance?.Announce("🎨 Instant Stencil Stamp Applied!");
                }
            }
        }

        private void HandleTimers()
        {
            if (dashTimer > 0f) dashTimer -= Time.deltaTime;
            if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
            if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;

            if (bubbleShieldTimer > 0f)
            {
                bubbleShieldTimer -= Time.deltaTime;
                if (bubbleShieldTimer <= 0f)
                {
                    bubbleShieldActive = false;
                    if (bubbleShieldVisual != null) bubbleShieldVisual.SetActive(false);
                }
            }

            if (slowmoTimer > 0f)
            {
                slowmoTimer -= Time.unscaledDeltaTime;
                if (slowmoTimer <= 0f)
                {
                    slowmoActive = false;
                    Time.timeScale = 1.0f;
                }
            }

            if (neonSodaTimer > 0f)
            {
                neonSodaTimer -= Time.deltaTime;
            }

            if (hypeOverdriveTimer > 0f)
            {
                hypeOverdriveTimer -= Time.deltaTime;
            }

            if (superchargedPaintTimer > 0f)
            {
                superchargedPaintTimer -= Time.deltaTime;
            }
        }

        public void ActivateHypeOverdrive(float duration)
        {
            hypeOverdriveTimer = Mathf.Max(hypeOverdriveTimer, duration);
            currentStamina = maxStamina;
            GameManager.Instance?.Announce("⚡ HYPE OVERDRIVE! 1.8x speed & infinite stamina!");
        }

        public void ActivateSuperchargedPaint(float duration)
        {
            superchargedPaintTimer = Mathf.Max(superchargedPaintTimer, duration);
            GameManager.Instance?.Announce("🔥 SUPERCHARGED PRESSURE! High-velocity armor piercing paintballs!");
        }

        public void CalibrateSprayNozzles()
        {
            calibratedNozzle = true;
            GameManager.Instance?.Announce("🎯 NOZZLES CALIBRATED! +35% reach and faster tags!");
        }
    }
}
