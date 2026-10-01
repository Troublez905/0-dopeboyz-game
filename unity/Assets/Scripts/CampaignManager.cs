using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dopeboyz
{
    [Serializable]
    public class DistrictData
    {
        public string name;
        public int requiredWalls;
        public int requiredRep;
        public string taskDescription;
        public int rewardCredits;
        public float holdTimeRequired = 0f;
        public bool requireDelivery = false;
        public int requireContracts = 0;
        public int requireCrew = 0;
        public bool requireRivalCaptain = false;
    }

    [Serializable]
    public class UpgradeData
    {
        public string id;
        public string name;
        public string detail;
        public int unlockDistrict;
        public int currentRank;
        public const int MAX_RANK = 5;
    }

    public class CampaignManager : MonoBehaviour
    {
        public static CampaignManager Instance { get; private set; }

        [Header("Campaign State")]
        public int currentDistrictIndex = 0;
        public int clearedDistricts = 0;
        public int levelCredits = 0;
        public bool campaignCompleted = false;

        [Header("HQ Stash")]
        public float hqStashPaint = 50f;
        public float maxStashPaint = 50f;

        [Header("Objective Tracking")]
        public float currentHoldTimer = 0f;
        public int contractsCompletedThisDistrict = 0;
        public bool hasActiveDelivery = false;
        public bool rivalCaptainDefeated = false;

        [Header("Districts Configuration")]
        public List<DistrictData> districts = new List<DistrictData>();

        [Header("Upgrades Library")]
        public Dictionary<string, UpgradeData> upgrades = new Dictionary<string, UpgradeData>();

        public event Action<int> OnDistrictChanged;
        public event Action<int> OnCreditsChanged;
        public event Action OnUpgradesChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDistricts();
            InitializeUpgrades();
        }

        private void InitializeDistricts()
        {
            districts = new List<DistrictData>
            {
                new DistrictData { name = "CANAL INK YARD", requiredWalls = 3, requiredRep = 200, taskDescription = "Claim 3 walls and earn 200 rep", rewardCredits = 3 },
                new DistrictData { name = "NEON MARKET STRIP", requiredWalls = 4, requiredRep = 450, taskDescription = "Hold 4 walls and finish a street contract", rewardCredits = 3, requireContracts = 1 },
                new DistrictData { name = "RAILCUT BACKLOTS", requiredWalls = 5, requiredRep = 750, taskDescription = "Hold 5 walls with 2 crew members", rewardCredits = 4, requireCrew = 2 },
                new DistrictData { name = "DOWNTOWN CROWN", requiredWalls = 7, requiredRep = 1100, taskDescription = "Hold 7 walls for 30s at 1100 rep", rewardCredits = 5, holdTimeRequired = 30f },
                new DistrictData { name = "CAP ALLEY ARRIVAL", requiredWalls = 4, requiredRep = 450, taskDescription = "Hold 4 walls and deliver a shipment to HQ", rewardCredits = 3, requireDelivery = true },
                new DistrictData { name = "BASS BLOCK TAKEOVER", requiredWalls = 6, requiredRep = 750, taskDescription = "Hold 6 walls and complete 2 contracts", rewardCredits = 4, requireContracts = 2 },
                new DistrictData { name = "STICKER TUNNEL", requiredWalls = 7, requiredRep = 1000, taskDescription = "Hold 7 walls and defeat the rival captain", rewardCredits = 4, requireRivalCaptain = true },
                new DistrictData { name = "ROLLER HEIGHTS FINALE", requiredWalls = 9, requiredRep = 1400, taskDescription = "Hold 9 walls with 3 crew for 45s", rewardCredits = 5, holdTimeRequired = 45f, requireCrew = 3 }
            };
        }

        private void InitializeUpgrades()
        {
            upgrades.Clear();
            AddUpgrade("speed", "Runner", "+18 move speed per rank", 0);
            AddUpgrade("paint", "Paint rig", "+30 paint capacity, +12% tagging speed", 0);
            AddUpgrade("health", "Resilience", "+25 maximum health per rank", 0);
            AddUpgrade("crew", "Crew training", "+1 recruit slot, +8 crew damage per rank", 1);
            AddUpgrade("hq", "Headquarters", "+50 stash capacity, +20 starting paint", 1);
            AddUpgrade("gear", "Armor & gear", "+6 weapon damage, 8% less damage per rank", 2);
            AddUpgrade("lookout", "Lookout network", "+4 heat before police pursuit per rank", 4);
            AddUpgrade("influence", "Street influence", "+15% passive reputation per rank", 4);
        }

        private void AddUpgrade(string id, string name, string detail, int unlockDist)
        {
            upgrades[id] = new UpgradeData
            {
                id = id,
                name = name,
                detail = detail,
                unlockDistrict = unlockDist,
                currentRank = 0
            };
        }

        public int GetUpgradeRank(string id)
        {
            return upgrades.ContainsKey(id) ? upgrades[id].currentRank : 0;
        }

        public bool CanBuyUpgrade(string id)
        {
            if (!upgrades.ContainsKey(id)) return false;
            var u = upgrades[id];
            return levelCredits > 0 && clearedDistricts >= u.unlockDistrict && u.currentRank < UpgradeData.MAX_RANK;
        }

        public bool BuyUpgrade(string id)
        {
            if (!CanBuyUpgrade(id)) return false;
            levelCredits--;
            upgrades[id].currentRank++;
            OnCreditsChanged?.Invoke(levelCredits);
            OnUpgradesChanged?.Invoke();
            ApplyUpgradeEffects(id);
            GameManager.Instance?.Announce($"⭐ Upgraded {upgrades[id].name} to Rank {upgrades[id].currentRank}!");
            return true;
        }

        private void ApplyUpgradeEffects(string id)
        {
            if (PlayerController.Instance == null) return;

            if (id == "speed")
            {
                PlayerController.Instance.moveSpeed = 8.5f + GetUpgradeRank("speed") * 1.5f;
            }
            else if (id == "paint")
            {
                PlayerController.Instance.maxPaint = 100f + GetUpgradeRank("paint") * 30f;
            }
            else if (id == "health")
            {
                PlayerController.Instance.maxHealth = 100f + GetUpgradeRank("health") * 25f;
            }
            else if (id == "hq")
            {
                maxStashPaint = 50f + GetUpgradeRank("hq") * 50f;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) return;

            CheckDistrictProgress();
        }

        private void CheckDistrictProgress()
        {
            if (currentDistrictIndex >= districts.Count) return;
            var dist = districts[currentDistrictIndex];

            bool wallsMet = GameManager.Instance.crewWalls >= dist.requiredWalls;
            bool repMet = GameManager.Instance.score >= dist.requiredRep;
            bool contractsMet = contractsCompletedThisDistrict >= dist.requireContracts;
            bool captainMet = !dist.requireRivalCaptain || rivalCaptainDefeated;

            int activeCrew = 0;
            if (CityController.Instance != null)
            {
                activeCrew = CityController.Instance.GetActiveCrewCount();
            }
            bool crewMet = activeCrew >= dist.requireCrew;

            if (dist.holdTimeRequired > 0f)
            {
                if (wallsMet && repMet && crewMet)
                {
                    currentHoldTimer += Time.deltaTime;
                    if (currentHoldTimer >= dist.holdTimeRequired)
                    {
                        CompleteCurrentDistrict();
                    }
                }
                else
                {
                    currentHoldTimer = 0f;
                }
            }
            else
            {
                if (wallsMet && repMet && contractsMet && crewMet && captainMet)
                {
                    CompleteCurrentDistrict();
                }
            }
        }

        public void CompleteCurrentDistrict()
        {
            var dist = districts[currentDistrictIndex];
            levelCredits += dist.rewardCredits;
            clearedDistricts = Mathf.Max(clearedDistricts, currentDistrictIndex + 1);

            GameManager.Instance?.Announce($"🎉 DISTRICT CLEARED: {dist.name}! +{dist.rewardCredits} Upgrade Credits!");
            OnCreditsChanged?.Invoke(levelCredits);

            if (currentDistrictIndex + 1 < districts.Count)
            {
                currentDistrictIndex++;
                currentHoldTimer = 0f;
                contractsCompletedThisDistrict = 0;
                rivalCaptainDefeated = false;
                hasActiveDelivery = false;
                GameManager.Instance.districtLevel = currentDistrictIndex;
                GameManager.Instance.districtName = districts[currentDistrictIndex].name;
                OnDistrictChanged?.Invoke(currentDistrictIndex);
                GameManager.Instance?.Announce($"📍 Entering District: {districts[currentDistrictIndex].name}");
            }
            else
            {
                campaignCompleted = true;
                GameManager.Instance?.Announce("👑 CAMPAIGN COMPLETE! YOU OWN THE ENTIRE CITY!");
            }
        }

        public void DepositStash(float amount)
        {
            if (PlayerController.Instance == null) return;
            float toTransfer = Mathf.Min(amount, PlayerController.Instance.currentPaint);
            toTransfer = Mathf.Min(toTransfer, maxStashPaint - hqStashPaint);
            if (toTransfer > 0f)
            {
                PlayerController.Instance.currentPaint -= toTransfer;
                hqStashPaint += toTransfer;
                GameManager.Instance?.Announce($"📦 Deposited {toTransfer:F0} paint in HQ Stash ({hqStashPaint}/{maxStashPaint})");
            }
        }

        public void WithdrawStash(float amount)
        {
            if (PlayerController.Instance == null) return;
            float toTransfer = Mathf.Min(amount, hqStashPaint);
            toTransfer = Mathf.Min(toTransfer, PlayerController.Instance.maxPaint - PlayerController.Instance.currentPaint);
            if (toTransfer > 0f)
            {
                hqStashPaint -= toTransfer;
                PlayerController.Instance.currentPaint += toTransfer;
                GameManager.Instance?.Announce($"📦 Withdrew {toTransfer:F0} paint from HQ Stash ({hqStashPaint}/{maxStashPaint})");
            }
        }
    }
}
