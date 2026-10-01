using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dopeboyz
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        [Header("HUD Sliders & Gauges")]
        public Slider healthSlider;
        public Slider paintSlider;
        public Slider staminaSlider;
        public Text ammoText;
        public Text cashText;
        public Text scoreText;
        public Text comboText;
        public Text wantedStarsText;
        public Text cameraViewText;

        [Header("Campaign & District Tracker")]
        public Text districtTitleText;
        public Text objectiveText;
        public Text holdTimerText;

        [Header("Toast Notification Banner")]
        public GameObject toastPanel;
        public Text toastText;
        private Coroutine toastCoroutine;

        [Header("Upgrades Shop Panel")]
        public GameObject upgradesPanel;
        public Text creditsAvailableText;

        [Header("Contextual Interaction Prompt")]
        public GameObject interactionPromptPanel;
        public Text interactionPromptKeyText;
        public Text interactionPromptActionText;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAnnouncement += ShowToast;
                GameManager.Instance.OnCashChanged += UpdateCash;
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnWantedLevelChanged += UpdateWantedStars;
            }

            if (CampaignManager.Instance != null)
            {
                CampaignManager.Instance.OnDistrictChanged += (idx) => UpdateDistrictInfo();
                CampaignManager.Instance.OnCreditsChanged += (credits) => UpdateCredits(credits);
            }

            UpdateDistrictInfo();
        }

        private void Update()
        {
            UpdatePlayerGauges();
            UpdateComboDisplay();
            UpdateCameraViewDisplay();
            UpdateDistrictHoldTimer();

            // Toggle Upgrades Menu with Tab or Key U
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleUpgradesPanel();
            }
        }

        private void UpdatePlayerGauges()
        {
            if (PlayerController.Instance == null) return;

            if (healthSlider != null)
            {
                healthSlider.value = PlayerController.Instance.currentHealth / PlayerController.Instance.maxHealth;
            }

            if (paintSlider != null)
            {
                paintSlider.value = PlayerController.Instance.currentPaint / PlayerController.Instance.maxPaint;
            }

            if (staminaSlider != null)
            {
                staminaSlider.value = PlayerController.Instance.currentStamina / PlayerController.Instance.maxStamina;
            }

            if (ammoText != null)
            {
                ammoText.text = $"AMMO: {PlayerController.Instance.currentAmmo} / {PlayerController.Instance.maxAmmo}";
            }
        }

        private void UpdateComboDisplay()
        {
            if (GameManager.Instance == null || comboText == null) return;

            if (GameManager.Instance.combo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"x{GameManager.Instance.combo} COMBO ({GameManager.Instance.comboTimer:F0}s)";
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }

        private void UpdateCameraViewDisplay()
        {
            if (CameraController.Instance != null && cameraViewText != null)
            {
                cameraViewText.text = CameraController.Instance.zoomTierNames[CameraController.Instance.currentTierIndex];
            }
        }

        private void UpdateDistrictInfo()
        {
            if (CampaignManager.Instance == null || CampaignManager.Instance.districts.Count == 0) return;
            int idx = CampaignManager.Instance.currentDistrictIndex;
            var dist = CampaignManager.Instance.districts[idx];

            if (districtTitleText != null)
            {
                districtTitleText.text = $"DISTRICT {idx + 1}: {dist.name}";
            }

            if (objectiveText != null)
            {
                objectiveText.text = $"OBJECTIVE: {dist.taskDescription} (Walls: {GameManager.Instance?.crewWalls}/{dist.requiredWalls})";
            }
        }

        private void UpdateDistrictHoldTimer()
        {
            if (CampaignManager.Instance == null || holdTimerText == null) return;
            int idx = CampaignManager.Instance.currentDistrictIndex;
            var dist = CampaignManager.Instance.districts[idx];

            if (dist.holdTimeRequired > 0f)
            {
                holdTimerText.gameObject.SetActive(true);
                holdTimerText.text = $"HOLD TIME: {CampaignManager.Instance.currentHoldTimer:F1}s / {dist.holdTimeRequired:F0}s";
            }
            else
            {
                holdTimerText.gameObject.SetActive(false);
            }
        }

        public void UpdateCash(int newCash)
        {
            if (cashText != null) cashText.text = $"$ {newCash}";
        }

        public void UpdateScore(int newScore)
        {
            if (scoreText != null) scoreText.text = $"REP: {newScore}";
        }

        public void UpdateCredits(int credits)
        {
            if (creditsAvailableText != null) creditsAvailableText.text = $"UPGRADE CREDITS: {credits}";
        }

        public void UpdateWantedStars(int stars)
        {
            if (wantedStarsText != null)
            {
                string starString = "";
                for (int i = 0; i < stars; i++) starString += "★ ";
                wantedStarsText.text = starString;
                wantedStarsText.color = stars > 2 ? new Color(1f, 0.25f, 0.35f) : new Color(1f, 0.88f, 0.43f);
            }
        }

        public void ShowToast(string message)
        {
            if (toastPanel == null || toastText == null) return;
            if (toastCoroutine != null) StopCoroutine(toastCoroutine);
            toastCoroutine = StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            toastText.text = message;
            toastPanel.SetActive(true);
            yield return new WaitForSeconds(3.5f);
            toastPanel.SetActive(false);
        }

        public void ToggleUpgradesPanel()
        {
            if (upgradesPanel == null) return;
            bool active = !upgradesPanel.activeSelf;
            upgradesPanel.SetActive(active);
            if (active && CampaignManager.Instance != null)
            {
                UpdateCredits(CampaignManager.Instance.levelCredits);
            }
        }

        public void SetInteractionPrompt(string key, string actionText)
        {
            if (interactionPromptPanel == null) return;
            if (interactionPromptKeyText != null) interactionPromptKeyText.text = key;
            if (interactionPromptActionText != null) interactionPromptActionText.text = actionText;
            if (!interactionPromptPanel.activeSelf) interactionPromptPanel.SetActive(true);
        }

        public void ClearInteractionPrompt()
        {
            if (interactionPromptPanel != null && interactionPromptPanel.activeSelf)
            {
                interactionPromptPanel.SetActive(false);
            }
        }
    }
}
