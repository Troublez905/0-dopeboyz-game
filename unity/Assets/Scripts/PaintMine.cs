using UnityEngine;

namespace Dopeboyz
{
    public class PaintMine : MonoBehaviour
    {
        [Header("Mine Settings")]
        public float blastRadius = 5.0f;
        public float damage = 45f;
        public float slowDuration = 4.5f;
        public float armingDelay = 0.5f;
        private bool isArmed = false;
        private float armTimer = 0f;

        private void Start()
        {
            armTimer = armingDelay;
        }

        private void Update()
        {
            if (!isArmed)
            {
                armTimer -= Time.deltaTime;
                if (armTimer <= 0f) isArmed = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isArmed) return;

            if (other.CompareTag("Enemy") || other.CompareTag("Cop") || other.CompareTag("Rival"))
            {
                Detonate();
            }
        }

        public void Detonate()
        {
            SoundManager.Instance?.PlaySound(SoundType.Explosion, 0.9f);
            CameraController.Instance?.TriggerScreenShake(0.35f, 0.35f);

            // 360 degree blast
            Collider[] hits = Physics.OverlapSphere(transform.position, blastRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy") || hit.CompareTag("Cop") || hit.CompareTag("Rival"))
                {
                    var actor = hit.GetComponent<ActorController>();
                    if (actor != null)
                    {
                        Vector3 pushDir = (hit.transform.position - transform.position).normalized;
                        actor.TakeDamage(damage, pushDir);
                        actor.ApplySlow(0.4f, slowDuration);
                    }
                }
                else if (hit.CompareTag("Wall"))
                {
                    var tag = hit.GetComponent<WallTag>();
                    if (tag != null)
                    {
                        tag.ApplySpray("crew", 0.35f);
                    }
                }
            }

            // Visual explosion effect
            GameObject splat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            splat.transform.position = transform.position + Vector3.up * 0.05f;
            splat.transform.localScale = new Vector3(blastRadius * 1.5f, 0.02f, blastRadius * 1.5f);
            var col = splat.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var ren = splat.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = new Color(0.298f, 0.961f, 0.835f, 0.85f);
                ren.material = m;
            }
            Destroy(splat, 6.0f);

            Destroy(gameObject);
        }
    }
}
