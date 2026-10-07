using UnityEngine;

namespace Dopeboyz
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [Header("Target Tracking")]
        public Transform target;
        public float smoothSpeed = 8.0f;
        public Vector3 lookAtOffset = new Vector3(0, 1.2f, 0);

        [Header("3-Tier 3D Camera Zoom System")]
        public Camera cam;
        // Tier 0: Close / Action View (Height: 4.8m, Distance: 6.2m, FOV: 50f)
        // Tier 1: Street Medium View (Height: 8.5m, Distance: 11.0m, FOV: 58f)
        // Tier 2: Tactical Overview (Height: 14.5m, Distance: 18.0m, FOV: 65f)
        public float[] zoomDistances = new float[] { 6.2f, 11.0f, 18.0f };
        public float[] zoomHeights = new float[] { 4.8f, 8.5f, 14.5f };
        public float[] zoomFOVs = new float[] { 50f, 58f, 65f };
        public string[] zoomTierNames = new string[] { "CLOSE / ACTION VIEW (1.85x)", "STREET MEDIUM VIEW (1.35x)", "TACTICAL OVERVIEW (0.95x)" };
        public int currentTierIndex = 0;
        public float zoomLerpSpeed = 5.0f;

        [Header("District Boundaries")]
        public Vector3 minBounds = new Vector3(0f, 0f, 0f);
        public Vector3 maxBounds = new Vector3(60f, 50f, 60f);

        [Header("Screen Shake")]
        private float shakeIntensity = 0f;
        private float shakeDuration = 0f;
        private float yaw;
        private float pitch = 18f;

        private void Awake()
        {
            Instance = this;
            if (cam == null) cam = GetComponent<Camera>();
            if (cam != null)
            {
                cam.fieldOfView = zoomFOVs[currentTierIndex];
            }

            if (GetComponent<AudioListener>() == null && FindFirstObjectByType<AudioListener>() == null)
            {
                gameObject.AddComponent<AudioListener>();
            }
        }

        private void Update()
        {
            if (GameSession.Instance != null) return;
            // Toggle Camera View with F1 or Gamepad Right Stick Click
            if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.JoystickButton9))
            {
                CycleZoomTier();
            }

            // Smoothly interpolate FOV
            if (cam != null)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomFOVs[currentTierIndex], Time.deltaTime * zoomLerpSpeed);
            }
        }

        private void LateUpdate()
        {
            if (GameSession.Instance != null)
            {
                if (target == null && PlayerController.Instance != null) target = PlayerController.Instance.transform;
                if (target == null) return;
                bool playing = GameManager.Instance != null && GameManager.Instance.currentState == GameState.Playing;
                if (playing)
                {
                    yaw += Input.GetAxisRaw("Mouse X") * 2.5f;
                    pitch = Mathf.Clamp(pitch - Input.GetAxisRaw("Mouse Y") * 2f, -25f, 65f);
                }
                Quaternion orbit = Quaternion.Euler(pitch, yaw, 0);
                Vector3 pivot = target.position + Vector3.up * 1.5f;
                bool aiming = playing && Input.GetMouseButton(1);
                Vector3 offset = orbit * new Vector3(aiming ? 0.65f : 0.35f, 0, aiming ? -2.3f : -4.8f);
                float distance = offset.magnitude;
                foreach (var hit in Physics.SphereCastAll(pivot, 0.2f, offset.normalized, distance, ~0, QueryTriggerInteraction.Ignore))
                    if (!hit.transform.IsChildOf(target) && (hit.collider.CompareTag("Obstacle") || hit.collider.GetComponentInParent<WallTag>() != null || hit.collider.name == "City_Ground_Asphalt"))
                        distance = Mathf.Min(distance, Mathf.Max(0.3f, hit.distance - 0.15f));
                transform.position = pivot + offset.normalized * distance;
                transform.rotation = orbit;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, aiming ? 48f : 65f, Time.unscaledDeltaTime * 10f);
                return;
            }
            if (target == null)
            {
                var p = GameObject.FindWithTag("Player");
                if (p != null) target = p.transform;
                else return;
            }

            float curDist = zoomDistances[currentTierIndex];
            float curHeight = zoomHeights[currentTierIndex];
            Vector3 targetOffset = new Vector3(0, curHeight, -curDist);

            Vector3 desiredPosition = target.position + targetOffset;

            // Apply screen shake offset
            if (shakeDuration > 0f)
            {
                shakeDuration -= Time.deltaTime;
                Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
                desiredPosition += shakeOffset;
            }

            // Clamp position within district boundaries
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minBounds.z, maxBounds.z);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

            // Always aim camera smoothly down at player
            Vector3 lookTarget = target.position + lookAtOffset;
            Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * smoothSpeed);
        }

        public void CycleZoomTier()
        {
            currentTierIndex = (currentTierIndex + 1) % zoomTierNames.Length;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Announce($"🎥 3D Camera View: {zoomTierNames[currentTierIndex]}");
            }
            if (UIController.Instance != null && UIController.Instance.cameraViewText != null)
            {
                UIController.Instance.cameraViewText.text = zoomTierNames[currentTierIndex];
            }
        }

        public void TriggerScreenShake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
        }
    }
}
