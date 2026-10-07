using UnityEngine;

namespace Dopeboyz
{
    public class DeployableTurret : MonoBehaviour
    {
        [Header("Turret Targeting")]
        public float range = 14f;
        public float fireRate = 0.55f;
        public float damage = 18f;
        public float projectileSpeed = 22f;
        public GameObject projectilePrefab;
        public Transform nozzleTransform;
        public Transform swivelTransform;

        [Header("Auto Tagging")]
        public float tagRadius = 5.5f;
        public float tagInterval = 1.0f;
        private float tagTimer = 0f;

        [Header("State")]
        public float lifetime = 45f;
        public float health = 120f;
        private float fireTimer = 0f;
        private Transform currentTarget;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            fireTimer -= Time.deltaTime;
            tagTimer -= Time.deltaTime;

            FindTarget();

            if (currentTarget != null)
            {
                // Aim swivel at target
                Vector3 dir = (currentTarget.position - transform.position);
                dir.y = 0f; // horizontal rotation
                if (dir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    if (swivelTransform != null)
                        swivelTransform.rotation = Quaternion.Slerp(swivelTransform.rotation, targetRot, Time.deltaTime * 10f);
                    else
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
                }

                if (fireTimer <= 0f)
                {
                    FirePaintball(currentTarget.position);
                    fireTimer = fireRate;
                }
            }

            // Auto-tag adjacent walls within radius
            if (tagTimer <= 0f)
            {
                tagTimer = tagInterval;
                AutoTagNearbyWalls();
            }
        }

        private void FindTarget()
        {
            currentTarget = null;
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            float closestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy") || hit.CompareTag("Cop") || hit.CompareTag("Rival"))
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        currentTarget = hit.transform;
                    }
                }
            }
        }

        private void FirePaintball(Vector3 targetPos)
        {
            Vector3 spawnPos = nozzleTransform != null ? nozzleTransform.position : transform.position + Vector3.up * 0.8f + transform.forward * 0.5f;
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
                var rb = proj.AddComponent<Rigidbody>();
                rb.useGravity = false;
            }

            var pComp = proj.GetComponent<Projectile>();
            if (pComp == null) pComp = proj.AddComponent<Projectile>();
            pComp.Initialize("Crew", aimDir, damage, projectileSpeed);

            SoundManager.Instance?.PlaySound(SoundType.ShootPaint, 0.6f);
        }

        private void AutoTagNearbyWalls()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, tagRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Wall"))
                {
                    var tag = hit.GetComponent<WallTag>();
                    if (tag != null && tag.owner != WallOwner.Crew)
                    {
                        tag.ApplySpray("crew", 0.08f);
                    }
                }
            }
        }

        public void TakeDamage(float dmg)
        {
            health -= dmg;
            if (health <= 0f)
            {
                SoundManager.Instance?.PlaySound(SoundType.Explosion, 0.8f);
                Destroy(gameObject);
            }
        }
    }
}
