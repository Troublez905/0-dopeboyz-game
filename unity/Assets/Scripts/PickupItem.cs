using UnityEngine;

namespace Dopeboyz
{
    public enum PickupKind
    {
        Paint,
        Ammo,
        Health,
        Weapon,
        Recruit
    }

    public class PickupItem : MonoBehaviour
    {
        [Header("Pickup Configuration")]
        public PickupKind kind = PickupKind.Paint;
        public float cooldownTime = 25f;
        private float currentCooldown = 0f;

        [Header("Visual Effects")]
        public SpriteRenderer spriteRenderer;
        public float floatAmplitude = 0.15f;
        public float floatFrequency = 2.5f;
        private Vector3 initialLocalPos;

        private void Start()
        {
            initialLocalPos = transform.position;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (currentCooldown > 0f)
            {
                currentCooldown -= Time.deltaTime;
                if (currentCooldown <= 0f)
                {
                    Respawn();
                }
            }
            else
            {
                // Subtle floating bob
                float offset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
                transform.position = initialLocalPos + new Vector3(0, offset, 0);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (currentCooldown > 0f) return;

            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null && Collect(player))
                {
                    currentCooldown = cooldownTime;
                    if (spriteRenderer != null) spriteRenderer.enabled = false;
                    SoundManager.Instance?.PlaySound(SoundType.Pickup);
                }
            }
        }

        private bool Collect(PlayerController player)
        {
            switch (kind)
            {
                case PickupKind.Paint:
                    if (player.currentPaint >= player.maxPaint) return false;
                    player.currentPaint = Mathf.Min(player.maxPaint, player.currentPaint + 35f);
                    GameManager.Instance?.Announce("🎨 Collected Spray Paint (+35)!");
                    return true;

                case PickupKind.Ammo:
                    if (player.currentAmmo >= player.maxAmmo) return false;
                    player.currentAmmo = Mathf.Min(player.maxAmmo, player.currentAmmo + 24);
                    GameManager.Instance?.Announce("🔫 Collected Paintball Rounds (+24)!");
                    return true;

                case PickupKind.Health:
                    if (player.currentHealth >= player.maxHealth) return false;
                    player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + 35f);
                    GameManager.Instance?.Announce("❤️ Collected Medical Bandages (+35 HP)!");
                    return true;

                case PickupKind.Weapon:
                    player.hasWeapon = true;
                    player.currentAmmo = player.maxAmmo;
                    GameManager.Instance?.Announce("🔥 Found Heavy Spray Gun & Full Ammo!");
                    return true;

                case PickupKind.Recruit:
                    if (CityController.Instance != null && CityController.Instance.SpawnCrewRecruit(transform.position))
                    {
                        GameManager.Instance?.Announce("🤝 Recruited Crew Member to your squad!");
                        return true;
                    }
                    return false;
            }
            return false;
        }

        private void Respawn()
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;
        }
    }
}
