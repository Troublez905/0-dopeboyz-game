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
        RivalCaptain
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

        [Header("Combat & Weapons")]
        public bool canShoot = true;
        public float attackRange = 7.5f;
        public float fireCooldown = 1.2f;
        private float fireTimer = 0f;
        public GameObject projectilePrefab;

        [Header("Crew Specific")]
        public CrewOrder crewOrder = CrewOrder.Follow;
        public Vector2 guardPoint;

        [Header("SWAT Riot Shield")]
        public bool hasRiotShield = false;
        public Transform shieldTransform;

        [Header("AI State")]
        public Transform target;
        public WallTag targetWall;
        private Rigidbody2D rb;
        private float wanderTimer = 0f;
        private Vector2 wanderDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (kind == ActorKind.SwatRiot) hasRiotShield = true;
            if (kind == ActorKind.RivalCaptain)
            {
                maxHealth = 320f;
                health = 320f;
                moveSpeed = 6.2f;
            }
        }

        private void Update()
        {
            if (fireTimer > 0f) fireTimer -= Time.deltaTime;
            wanderTimer -= Time.deltaTime;

            ExecuteAI();
        }

        private void ExecuteAI()
        {
            if (PlayerController.Instance == null) return;
            Vector2 playerPos = PlayerController.Instance.transform.position;
            float distToPlayer = Vector2.Distance(transform.position, playerPos);

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
                    ExecutePoliceBehavior(playerPos, distToPlayer);
                    break;

                case ActorKind.Rival:
                case ActorKind.RivalCaptain:
                    ExecuteRivalBehavior(playerPos, distToPlayer);
                    break;
            }
        }

        private void ExecutePedestrianBehavior(Vector2 playerPos, float distToPlayer)
        {
            // Flee if combat nearby or wander peacefully
            if (distToPlayer < 4.0f)
            {
                Vector2 fleeDir = ((Vector2)transform.position - playerPos).normalized;
                rb.linearVelocity = fleeDir * moveSpeed;
            }
            else
            {
                if (wanderTimer <= 0f)
                {
                    wanderDirection = Random.insideUnitCircle.normalized;
                    wanderTimer = Random.Range(2.0f, 5.0f);
                }
                rb.linearVelocity = wanderDirection * (moveSpeed * 0.4f);
            }
        }

        private void ExecuteCrewBehavior(Vector2 playerPos, float distToPlayer)
        {
            // First check if any rival or hostile cop is nearby to shoot
            ActorController enemy = FindHostileTarget();
            if (enemy != null)
            {
                Vector2 aimDir = ((Vector2)enemy.transform.position - (Vector2)transform.position).normalized;
                TryShoot(aimDir, "Crew");
            }

            Vector2 targetPos = playerPos;
            if (crewOrder == CrewOrder.Guard)
            {
                targetPos = guardPoint;
            }

            float stopDist = (crewOrder == CrewOrder.Regroup) ? 1.5f : 3.0f;
            if (Vector2.Distance(transform.position, targetPos) > stopDist)
            {
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                rb.linearVelocity = dir * (moveSpeed * 1.1f);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        private void ExecutePoliceBehavior(Vector2 playerPos, float distToPlayer)
        {
            if (GameManager.Instance != null && GameManager.Instance.wantedStars > 0)
            {
                Vector2 dir = (playerPos - (Vector2)transform.position).normalized;
                rb.linearVelocity = dir * (moveSpeed * (kind == ActorKind.SwatRiot ? 1.05f : 1.2f));

                if (hasRiotShield && shieldTransform != null)
                {
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    shieldTransform.rotation = Quaternion.Euler(0, 0, angle);
                }

                // Close range tackle / arrest
                if (distToPlayer < 1.4f)
                {
                    PlayerController.Instance.ArrestByPolice();
                }
                else if (distToPlayer < attackRange && fireTimer <= 0f)
                {
                    TryShoot(dir, "Cop");
                }
            }
            else
            {
                // Routine patrol
                if (wanderTimer <= 0f)
                {
                    wanderDirection = Random.insideUnitCircle.normalized;
                    wanderTimer = Random.Range(3.0f, 6.0f);
                }
                rb.linearVelocity = wanderDirection * (moveSpeed * 0.5f);
            }
        }

        private void ExecuteRivalBehavior(Vector2 playerPos, float distToPlayer)
        {
            // If near player or crew, engage in combat
            if (distToPlayer < attackRange)
            {
                Vector2 dir = (playerPos - (Vector2)transform.position).normalized;
                rb.linearVelocity = dir * (moveSpeed * 0.75f);
                if (fireTimer <= 0f)
                {
                    TryShoot(dir, "Rival");
                }
            }
            else
            {
                // Seek out walls to tag over
                if (targetWall == null || targetWall.owner == WallOwner.Rival)
                {
                    targetWall = FindWallToTag();
                }

                if (targetWall != null)
                {
                    Vector2 wallPos = targetWall.transform.position;
                    if (Vector2.Distance(transform.position, wallPos) > 1.8f)
                    {
                        Vector2 dir = (wallPos - (Vector2)transform.position).normalized;
                        rb.linearVelocity = dir * moveSpeed;
                    }
                    else
                    {
                        rb.linearVelocity = Vector2.zero;
                        targetWall.ApplySpray("rival", 0.35f * Time.deltaTime);
                    }
                }
                else
                {
                    // Wander
                    if (wanderTimer <= 0f)
                    {
                        wanderDirection = Random.insideUnitCircle.normalized;
                        wanderTimer = Random.Range(2.0f, 4.0f);
                    }
                    rb.linearVelocity = wanderDirection * (moveSpeed * 0.6f);
                }
            }
        }

        private void TryShoot(Vector2 direction, string shooterType)
        {
            if (fireTimer > 0f) return;
            fireTimer = fireCooldown;

            if (projectilePrefab != null)
            {
                Vector3 spawnPos = transform.position + (Vector3)direction * 0.6f;
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                Projectile proj = projObj.GetComponent<Projectile>();
                if (proj != null)
                {
                    float dmg = (kind == ActorKind.RivalCaptain) ? 35f : 18f;
                    proj.Initialize(shooterType, direction, dmg);
                }
            }
        }

        private WallTag FindWallToTag()
        {
            if (CityController.Instance == null) return null;
            WallTag closest = null;
            float minDist = float.MaxValue;
            foreach (var w in CityController.Instance.districtWalls)
            {
                if (w.owner != WallOwner.Rival)
                {
                    float d = Vector2.Distance(transform.position, w.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = w;
                    }
                }
            }
            return closest;
        }

        private ActorController FindHostileTarget()
        {
            ActorController[] actors = FindObjectsByType<ActorController>(FindObjectsSortMode.None);
            ActorController best = null;
            float minDist = 10f;
            foreach (var a in actors)
            {
                if (a != this && (a.kind == ActorKind.Rival || a.kind == ActorKind.RivalCaptain))
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
            if (hasRiotShield && shieldTransform != null)
            {
                Vector2 shieldForward = shieldTransform.right;
                if (Vector2.Dot(-hitDirection, shieldForward) > 0.4f)
                {
                    SoundManager.Instance?.PlaySound(SoundType.HitDeflect);
                    GameManager.Instance?.Announce("🛡️ Clang! Deflected by SWAT Riot Shield!");
                    return;
                }
            }

            health -= amount;
            rb.AddForce(hitDirection * 140f, ForceMode2D.Impulse);

            if (health <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            if (kind == ActorKind.RivalCaptain)
            {
                if (CampaignManager.Instance != null)
                {
                    CampaignManager.Instance.rivalCaptainDefeated = true;
                }
                GameManager.Instance?.AddCash(200);
                GameManager.Instance?.AddScore(300);
                GameManager.Instance?.Announce("💥 RIVAL CAPTAIN DEFEATED! +$200 & +300 Rep!");
            }
            else if (kind == ActorKind.Rival)
            {
                GameManager.Instance?.AddCash(35);
                GameManager.Instance?.AddScore(40);
                GameManager.Instance?.Announce("Enemy rival neutralized (+$35)");
            }

            Destroy(gameObject);
        }
    }
}
