using System.Collections;
using UnityEngine;

namespace Dopeboyz
{
    public enum ActorKind
    {
        Pedestrian,
        Crew,
        Rival,
        Cop,
        SwatRiot,
        RivalCaptain,
        PoliceK9
    }

    public enum CrewOrder
    {
        Follow,
        Guard,
        Regroup
    }

    public class ActorController : MonoBehaviour
    {
        [Header("Actor Identity")]
        public ActorKind kind = ActorKind.Pedestrian;
        public float health = 100f;
        public float maxHealth = 100f;
        public float moveSpeed = 5.5f;
        private float baseSpeed = 5.5f;
        private float slowTimer = 0f;

        [Header("Combat & Weapons")]
        public bool canShoot = true;
        public float attackRange = 10f;
        public float fireCooldown = 1.2f;
        private float fireTimer = 0f;
        public GameObject projectilePrefab;

        [Header("Crew Specific")]
        public CrewOrder crewOrder = CrewOrder.Follow;
        public Vector3 guardPoint;

        [Header("SWAT Riot Shield")]
        public bool hasRiotShield = false;
        public Transform shieldTransform;

        [Header("AI State")]
        public Transform target;
        public WallTag targetWall;
        private Rigidbody rb;
        private float wanderTimer = 0f;
        private Vector3 wanderDirection;
        private float repaintSprayTimer = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            baseSpeed = moveSpeed;
            if (kind == ActorKind.SwatRiot)
            {
                hasRiotShield = true;
                maxHealth = 180f;
                health = 180f;
            }
            else if (kind == ActorKind.RivalCaptain)
            {
                maxHealth = 350f;
                health = 350f;
                moveSpeed = 7.0f;
                baseSpeed = 7.0f;
            }
            else if (kind == ActorKind.PoliceK9)
            {
                moveSpeed = 8.5f;
                baseSpeed = 8.5f;
                maxHealth = 80f;
                health = 80f;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) return;
            if (fireTimer > 0f) fireTimer -= Time.deltaTime;
            wanderTimer -= Time.deltaTime;

            if (slowTimer > 0f)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0f) moveSpeed = baseSpeed;
            }

            ExecuteAI();
        }

        private void ExecuteAI()
        {
            if (PlayerController.Instance == null) return;
            Vector3 playerPos = PlayerController.Instance.transform.position;
            float distToPlayer = Vector3.Distance(transform.position, playerPos);

            switch (kind)
            {
                case ActorKind.Pedestrian:
                    ExecutePedestrianBehavior(playerPos, distToPlayer);
                    break;

                case ActorKind.Crew:
                    ExecuteCrewBehavior(playerPos, distToPlayer);
                    break;

                case ActorKind.Cop:
                case ActorKind.SwatRiot:
                case ActorKind.PoliceK9:
                    ExecutePoliceBehavior(playerPos, distToPlayer);
                    break;

                case ActorKind.Rival:
                case ActorKind.RivalCaptain:
                    ExecuteRivalBehavior(playerPos, distToPlayer);
                    break;
            }
        }

        private void ExecutePedestrianBehavior(Vector3 playerPos, float distToPlayer)
        {
            // Flee if gunfight nearby
            if (distToPlayer < 7.0f && GameManager.Instance != null && GameManager.Instance.cityHeat > 20f)
            {
                Vector3 fleeDir = (transform.position - playerPos).normalized;
                fleeDir.y = 0f;
                MoveInDirection(fleeDir, moveSpeed * 1.3f);
            }
            else
            {
                Wander();
            }
        }

        private void ExecuteCrewBehavior(Vector3 playerPos, float distToPlayer)
        {
            // Attack nearest rival or cop within range
            Transform nearestEnemy = FindNearestTargetWithTags("Enemy", "Cop", "Rival");
            if (nearestEnemy != null)
            {
                float dist = Vector3.Distance(transform.position, nearestEnemy.position);
                if (dist <= attackRange)
                {
                    AimAt(nearestEnemy.position);
                    if (fireTimer <= 0f)
                    {
                        ShootProjectile(nearestEnemy.position);
                        fireTimer = fireCooldown;
                    }
                }
            }

            // Follow squad orders
            switch (crewOrder)
            {
                case CrewOrder.Follow:
                    if (distToPlayer > 3.0f)
                    {
                        Vector3 dir = (playerPos - transform.position).normalized;
                        dir.y = 0f;
                        MoveInDirection(dir, moveSpeed);
                    }
                    else
                    {
                        StopMoving();
                    }
                    break;

                case CrewOrder.Guard:
                    float distToGuard = Vector3.Distance(transform.position, guardPoint);
                    if (distToGuard > 1.5f)
                    {
                        Vector3 dir = (guardPoint - transform.position).normalized;
                        dir.y = 0f;
                        MoveInDirection(dir, moveSpeed);
                    }
                    else
                    {
                        StopMoving();
                    }
                    break;

                case CrewOrder.Regroup:
                    if (distToPlayer > 1.8f)
                    {
                        Vector3 dir = (playerPos - transform.position).normalized;
                        dir.y = 0f;
                        MoveInDirection(dir, moveSpeed * 1.4f);
                    }
                    else
                    {
                        crewOrder = CrewOrder.Follow;
                    }
                    break;
            }
        }

        private void ExecutePoliceBehavior(Vector3 playerPos, float distToPlayer)
        {
            int wantedStars = GameManager.Instance != null ? GameManager.Instance.wantedStars : 0;
            if (wantedStars == 0)
            {
                Wander();
                return;
            }

            // Thermal foil stealth check
            if (PlayerController.Instance.thermalFoilActive && distToPlayer > 6.0f)
            {
                Wander();
                return;
            }

            // Chase and shoot player
            Vector3 chaseDir = (playerPos - transform.position).normalized;
            chaseDir.y = 0f;

            if (distToPlayer > (kind == ActorKind.PoliceK9 ? 1.0f : 4.5f))
            {
                MoveInDirection(chaseDir, moveSpeed);
            }
            else
            {
                StopMoving();
            }

            AimAt(playerPos);

            if (canShoot && distToPlayer <= attackRange && fireTimer <= 0f)
            {
                ShootProjectile(playerPos);
                fireTimer = (kind == ActorKind.SwatRiot) ? 0.7f : 1.2f;
            }
        }

        private void ExecuteRivalBehavior(Vector3 playerPos, float distToPlayer)
        {
            // If near player, engage in spray combat
            if (distToPlayer < attackRange)
            {
                AimAt(playerPos);
                if (fireTimer <= 0f)
                {
                    ShootProjectile(playerPos);
                    fireTimer = (kind == ActorKind.RivalCaptain) ? 0.6f : 1.4f;
                }

                if (distToPlayer > 3.5f)
                {
                    Vector3 dir = (playerPos - transform.position).normalized;
                    dir.y = 0f;
                    MoveInDirection(dir, moveSpeed);
                }
                else
                {
                    StopMoving();
                }
                return;
            }

            // Hunt for crew-owned or unclaimed walls to repaint
            if (targetWall == null || targetWall.owner == WallOwner.Rival)
            {
                FindBestWallToRepaint();
            }

            if (targetWall != null)
            {
                float distToWall = Vector3.Distance(transform.position, targetWall.transform.position);
                if (distToWall > 2.5f)
                {
                    Vector3 dir = (targetWall.transform.position - transform.position).normalized;
                    dir.y = 0f;
                    MoveInDirection(dir, moveSpeed);
                }
                else
                {
                    StopMoving();
                    AimAt(targetWall.transform.position);

                    repaintSprayTimer += Time.deltaTime;
                    if (repaintSprayTimer >= 0.2f)
                    {
                        repaintSprayTimer = 0f;
                        targetWall.ApplySpray("rival", (kind == ActorKind.RivalCaptain ? 0.08f : 0.04f));
                    }
                }
            }
            else
            {
                Wander();
            }
        }

        private void FindBestWallToRepaint()
        {
            if (CityController.Instance == null) return;
            WallTag best = null;
            float closestDist = float.MaxValue;

            foreach (var wall in CityController.Instance.districtWalls)
            {
                if (wall != null && wall.owner != WallOwner.Rival)
                {
                    float dist = Vector3.Distance(transform.position, wall.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        best = wall;
                    }
                }
            }
            targetWall = best;
        }

        private void Wander()
        {
            if (wanderTimer <= 0f)
            {
                wanderTimer = Random.Range(2.5f, 5.0f);
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                wanderDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            }
            MoveInDirection(wanderDirection, moveSpeed * 0.45f);
        }

        private void MoveInDirection(Vector3 dir, float speed)
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(dir.x * speed, rb.linearVelocity.y, dir.z * speed);
            }
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
        }

        private void StopMoving()
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }

        private void AimAt(Vector3 targetPos)
        {
            Vector3 aimDir = (targetPos - transform.position);
            aimDir.y = 0f;
            if (aimDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(aimDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 12f);
            }
        }

        private void ShootProjectile(Vector3 targetPos)
        {
            if (kind == ActorKind.Crew) { FireProjectile(targetPos); return; }
            StartCoroutine(TelegraphShot(targetPos));
        }

        private IEnumerator TelegraphShot(Vector3 targetPos)
        {
            var warning = new GameObject("Incoming paint shot");
            var line = warning.AddComponent<LineRenderer>();
            var material = new Material(Shader.Find("Sprites/Default"));
            line.material = material; line.positionCount = 2; line.startWidth = .025f; line.endWidth = .055f;
            line.startColor = line.endColor = new Color(1,.3f,.2f,.7f);
            Vector3 origin = transform.position + Vector3.up;
            line.SetPosition(0, origin); line.SetPosition(1, targetPos + Vector3.up);
            yield return new WaitForSeconds(.45f);
            Destroy(warning); Destroy(material);
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) yield break;
            Vector3 delta = targetPos + Vector3.up - origin;
            if (Physics.Raycast(origin, delta.normalized, out var hit, delta.magnitude, ~0, QueryTriggerInteraction.Ignore)
                && hit.collider.GetComponentInParent<PlayerController>() == null) yield break;
            FireProjectile(targetPos);
        }

        private void FireProjectile(Vector3 targetPos)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f + transform.forward * 0.6f;
            Vector3 aimDir = (targetPos + Vector3.up * 0.5f - spawnPos).normalized;

            GameObject proj = null;
            if (projectilePrefab != null)
            {
                proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(aimDir));
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
            string shooter = (kind == ActorKind.Crew) ? "Crew" : (kind == ActorKind.Cop || kind == ActorKind.SwatRiot || kind == ActorKind.PoliceK9) ? "Cop" : "Rival";
            pComp.Initialize(shooter, aimDir, 16f, 22f);

            SoundManager.Instance?.PlaySound(SoundType.ShootPaint, 0.5f);
        }

        public void TakeDamage(float dmg, Vector3 hitDirection)
        {
            // SWAT front shield deflection
            if (hasRiotShield)
            {
                float dot = Vector3.Dot(transform.forward, -hitDirection);
                if (dot > 0.25f) // Deflected from front
                {
                    SoundManager.Instance?.PlaySound(SoundType.ShieldHit, 0.8f);
                    GameManager.Instance?.Announce("🛡️ SWAT Shield Deflected Paint Round! Flank them!");
                    return;
                }
            }

            health -= dmg;

            // Knockback
            if (rb != null)
            {
                rb.AddForce(hitDirection.normalized * 7.5f, ForceMode.Impulse);
            }

            if (health <= 0f)
            {
                Die();
            }
        }

        public void ApplySlow(float factor, float duration)
        {
            moveSpeed = baseSpeed * factor;
            slowTimer = duration;
        }

        private void Die()
        {
            SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 1.0f);

            if (kind == ActorKind.RivalCaptain)
            {
                GameManager.Instance?.AddCash(300);
                GameManager.Instance?.AddScore(500);
                GameManager.Instance?.Announce("👑 RIVAL BOSS CAPTAIN DEFEATED! Earned $300 + 500 Rep!");
                if (CampaignManager.Instance != null)
                {
                    CampaignManager.Instance.rivalCaptainDefeated = true;
                }
            }
            else if (kind == ActorKind.Rival)
            {
                GameManager.Instance?.AddCash(35);
                GameManager.Instance?.AddScore(50);
            }

            Destroy(gameObject);
        }

        private Transform FindNearestTargetWithTags(params string[] tags)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
            Transform closest = null;
            float minDist = float.MaxValue;

            foreach (var hit in hits)
            {
                foreach (string t in tags)
                {
                    if (hit.CompareTag(t))
                    {
                        float d = Vector3.Distance(transform.position, hit.transform.position);
                        if (d < minDist)
                        {
                            minDist = d;
                            closest = hit.transform;
                        }
                    }
                }
            }
            return closest;
        }
    }
}
