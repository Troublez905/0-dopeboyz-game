using UnityEngine;

namespace Dopeboyz
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Properties")]
        public string shooterTag = "Player";
        public float damage = 22f;
        public float speed = 25f;
        public float lifetime = 2.5f;
        private Rigidbody rb;

        public void Initialize(string shooter, Vector3 direction, float dmg = 22f, float spd = 25f)
        {
            shooterTag = shooter;
            damage = dmg;
            speed = spd;
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearVelocity = direction.normalized * speed;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.isTrigger && !collision.CompareTag("Player") && !collision.CompareTag("Enemy") && !collision.CompareTag("Crew"))
            {
                return; // ignore trigger zones like pickups
            }

            // Prevent shooting self/allies
            if (shooterTag == "Player" || shooterTag == "Crew")
            {
                if (collision.CompareTag("Player") || collision.CompareTag("Crew")) return;

                if (collision.CompareTag("Enemy") || collision.CompareTag("Rival") || collision.CompareTag("Cop"))
                {
                    ActorController actor = collision.GetComponent<ActorController>();
                    if (actor != null)
                    {
                        actor.TakeDamage(damage, rb != null ? rb.linearVelocity.normalized : transform.forward);
                    }
                    SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.75f);
                    SpawnHitSplat(collision.transform.position);
                    Destroy(gameObject);
                    return;
                }
            }
            else // Fired by rival or cop
            {
                if (collision.CompareTag("Enemy") || collision.CompareTag("Rival") || collision.CompareTag("Cop")) return;

                if (collision.CompareTag("Player"))
                {
                    PlayerController player = collision.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        player.TakeDamage(damage, rb != null ? rb.linearVelocity.normalized : transform.forward);
                    }
                    SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.75f);
                    SpawnHitSplat(collision.transform.position);
                    Destroy(gameObject);
                    return;
                }
                else if (collision.CompareTag("Crew"))
                {
                    ActorController crew = collision.GetComponent<ActorController>();
                    if (crew != null)
                    {
                        crew.TakeDamage(damage, rb != null ? rb.linearVelocity.normalized : transform.forward);
                    }
                    SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.75f);
                    SpawnHitSplat(collision.transform.position);
                    Destroy(gameObject);
                    return;
                }
            }

            // Hit solid wall or obstacle
            if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
            {
                if (collision.CompareTag("Wall"))
                {
                    var tag = collision.GetComponent<WallTag>();
                    if (tag != null)
                    {
                        string sprayer = (shooterTag == "Player" || shooterTag == "Crew") ? "crew" : "rival";
                        tag.ApplySpray(sprayer, 0.12f);
                    }
                }
                SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.6f);
                SpawnHitSplat(transform.position);
                Destroy(gameObject);
            }
        }

        private void SpawnHitSplat(Vector3 hitPos)
        {
            // Small visual paint splat sphere
            GameObject splat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            splat.transform.position = hitPos;
            splat.transform.localScale = Vector3.one * 0.45f;
            var col = splat.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var ren = splat.GetComponent<Renderer>();
            if (ren != null)
            {
                Material m = new Material(Shader.Find("Standard"));
                m.color = (shooterTag == "Player" || shooterTag == "Crew") ? new Color(0.298f, 0.961f, 0.835f) : new Color(1.0f, 0.231f, 0.467f);
                ren.material = m;
            }
            Destroy(splat, 2.0f);
        }
    }
}
