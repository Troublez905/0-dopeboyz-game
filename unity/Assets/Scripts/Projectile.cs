using UnityEngine;

namespace Dopeboyz
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Properties")]
        public string shooterTag = "Player";
        public float damage = 22f;
        public float speed = 18f;
        public float lifetime = 2.5f;
        private Rigidbody2D rb;

        public void Initialize(string shooter, Vector2 direction, float dmg = 22f, float spd = 18f)
        {
            shooterTag = shooter;
            damage = dmg;
            speed = spd;
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction.normalized * speed;
            }
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
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
                        actor.TakeDamage(damage, rb != null ? rb.linearVelocity.normalized : transform.right);
                    }
                    SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.75f);
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
                        player.TakeDamage(damage, rb != null ? rb.linearVelocity.normalized : transform.right);
                    }
                    SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.75f);
                    Destroy(gameObject);
                    return;
                }
                else if (collision.CompareTag("Crew"))
                {
                    ActorController crew = collision.GetComponent<ActorController>();
                    if (crew != null)
                    {
                        crew.TakeDamage(damage, rb != null ? rb.linearVelocity.normalized : transform.right);
                    }
                    SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.75f);
                    Destroy(gameObject);
                    return;
                }
            }

            // Hit solid wall or obstacle
            if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
            {
                SoundManager.Instance?.PlaySound(SoundType.PaintSplat, 0.6f);
                Destroy(gameObject);
            }
        }
    }
}
