using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dopeboyz
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Shop,
        HQ,
        BlackMarket,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameState currentState = GameState.Playing;
        public int districtLevel = 0;
        public string districtName = "CANAL CORNER DISTRICT";

        [Header("Economy & Progression")]
        public int cash = 150;
        public int score = 0;
        public int combo = 0;
        public float comboTimer = 0f;
        public const float COMBO_MAX_TIME = 40f;

        [Header("Heat & Law Enforcement")]
        [Range(0f, 100f)]
        public float cityHeat = 0f;
        public int wantedStars = 0; // 0 to 5

        [Header("Territory Status")]
        public int totalWalls = 12;
        public int crewWalls = 0;
        public int rivalWalls = 0;
        public bool goldenMasteryUnlocked = false;

        [Header("Active Mission / Contract")]
        public string currentContractWall = "";
        public int contractReward = 90;
        public bool contractActive = false;

        public event Action<int> OnCashChanged;
        public event Action<int> OnScoreChanged;
        public event Action<float> OnHeatChanged;
        public event Action<int> OnWantedLevelChanged;
        public event Action<string> OnAnnouncement;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (currentState != GameState.Playing) return;

            // Decay combo timer
            if (comboTimer > 0f)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f)
                {
                    combo = 0;
                }
            }

            // Update heat and wanted stars
            UpdateWantedLevel();
        }

        public void AddCash(int amount)
        {
            cash += amount;
            OnCashChanged?.Invoke(cash);
        }

        public bool SpendCash(int amount)
        {
            if (cash < amount) return false;
            cash -= amount;
            OnCashChanged?.Invoke(cash);
            return true;
        }

        public void AddScore(int amount)
        {
            score += amount;
            OnScoreChanged?.Invoke(score);
        }

        public void AddHeat(float amount)
        {
            cityHeat = Mathf.Clamp(cityHeat + amount, 0f, 100f);
            OnHeatChanged?.Invoke(cityHeat);
            UpdateWantedLevel();
        }

        public void CoolHeat(float amount)
        {
            cityHeat = Mathf.Clamp(cityHeat - amount, 0f, 100f);
            OnHeatChanged?.Invoke(cityHeat);
            UpdateWantedLevel();
        }

        private void UpdateWantedLevel()
        {
            int previousStars = wantedStars;
            if (cityHeat >= 80f) wantedStars = 5;
            else if (cityHeat >= 60f) wantedStars = 4;
            else if (cityHeat >= 40f) wantedStars = 3;
            else if (cityHeat >= 20f) wantedStars = 2;
            else if (cityHeat > 5f) wantedStars = 1;
            else wantedStars = 0;

            if (previousStars != wantedStars)
            {
                OnWantedLevelChanged?.Invoke(wantedStars);
            }
        }

        public void RegisterWallClaim(string owner, string wallName, int wallSize)
        {
            if (owner == "crew")
            {
                combo = Mathf.Min(combo + 1, 5);
                comboTimer = COMBO_MAX_TIME;
                int reward = (15 + wallSize * 5) * combo;
                AddCash(reward);
                AddScore(25 * combo);
                crewWalls++;

                Announce($"{wallName} claimed by 404 CREW! x{combo} Combo (+$ {reward})");

                // Check 100% district dominance
                if (crewWalls >= totalWalls && !goldenMasteryUnlocked)
                {
                    goldenMasteryUnlocked = true;
                    AddCash(500);
                    AddScore(1000);
                    Announce("🏆 100% DISTRICT DOMINANCE! 24K Golden Mastery Can Awarded!");
                }
            }
            else if (owner == "rival")
            {
                rivalWalls++;
                Announce($"⚠️ {wallName} tagged over by RIVALS!");
            }
        }

        public void Announce(string message)
        {
            Debug.Log($"[404 DOPEBOYZ] {message}");
            OnAnnouncement?.Invoke(message);
        }
    }
}
