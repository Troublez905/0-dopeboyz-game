using UnityEngine;

namespace Dopeboyz
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [Header("Target Tracking")]
        public Transform target;
        public Vector3 offset = new Vector3(0, 0, -10f);
        public float smoothSpeed = 7.5f;

        [Header("3-Tier Camera Zoom System")]
        public Camera cam;
        // Tier 0: Close / Action View (Default - 4.5f size ~ 1.85x zoom)
        // Tier 1: Street Medium View (6.5f size ~ 1.35x zoom)
        // Tier 2: Tactical Overview (9.0f size ~ 0.95x zoom)
        public float[] zoomTiers = new float[] { 4.5f, 6.5f, 9.0f };
        public string[] zoomTierNames = new string[] { "CLOSE / ACTION VIEW (1.85x)", "STREET MEDIUM VIEW (1.35x)", "TACTICAL OVERVIEW (0.95x)" };
        public int currentTierIndex = 0;
        public float zoomLerpSpeed = 6.0f;

        [Header("District Boundaries")]
        public Vector2 minBounds = new Vector2(200f, 200f);
        public Vector2 maxBounds = new Vector2(2300f, 2200f);

        [Header("Screen Shake")]
        private float shakeIntensity = 0f;
        private float shakeDuration = 0f;

        private void Awake()
        {
            Instance = this;
            if (cam == null) cam = GetComponent<Camera>();
            if (cam != null) cam.orthographicSize = zoomTiers[currentTierIndex];
        }

        private void Update()
        {
            // Toggle Camera View with F1 or Right Stick Click
            if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.JoystickButton9))
            {
                CycleZoomTier();
            }

            // Smoothly interpolate orthographic size to target zoom tier
            if (cam != null)
            {
                float targetSize = zoomTiers[currentTierIndex];
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomLerpSpeed);
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;

            // Apply screen shake offset
            if (shakeDuration > 0f)
            {
                shakeDuration -= Time.deltaTime;
                Vector3 shakeOffset = (Vector3)Random.insideUnitCircle * shakeIntensity;
                desiredPosition += shakeOffset;
            }

            // Clamp position within district boundaries
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        }

        public void CycleZoomTier()
        {
            currentTierIndex = (currentTierIndex + 1) % zoomTiers.Length;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Announce($"🎥 Camera View: {zoomTierNames[currentTierIndex]}");
            }
        }

        public void TriggerScreenShake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
        }
    }
}
