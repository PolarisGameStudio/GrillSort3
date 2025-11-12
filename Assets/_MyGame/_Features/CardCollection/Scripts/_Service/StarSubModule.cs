using System;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "StarSubmodule", menuName = "MyGame/SkewerJam/Features/CardCollection/StarSubmodule")]
    public class StarSubmodule : ScriptableObject
    {
        public const string DATA_KEY = "CARD_COLLECTION_STAR_SUBMODULE";

        public StarExchangeConfig starExchangeConfig;

        private IntDataPref _cardStarExchangeIndex;
        private IntDataPref _cardStar;

        private int _numStarView = 0;

        public int NumberStar => _cardStar.Value;
        public int NumberStarView => _numStarView;
        public int CardStarExchangeIndex => _cardStarExchangeIndex.Value;

        public event Action<int> OnStarChanged;


        public void LoadData()
        {
            _cardStarExchangeIndex = new IntDataPref($"{DATA_KEY}_cardStarExchangeIndex", -1);
            _cardStar = new IntDataPref($"{DATA_KEY}_cardStar", 0);
        }

        public void ResetData()
        {
            _cardStarExchangeIndex.Value = -1;
            _cardStar.Value = 0;
            OnStarChanged?.Invoke(_cardStar.Value);
        }

        public void SetNumberStarView()
        {
            _numStarView = _cardStar.Value;
        }

        public void AddCardStar(int numStar)
        {
            _cardStar.Value += numStar;
            OnStarChanged?.Invoke(_cardStar.Value);
        }

        public (bool success, string message) OnClickReceiveChest(int index)
        {
            if (CheckAlreadyReceived(index) == false)
            {
                return (false, "You have already received the reward");
            }
            if (CheckStarEnough(index) == false)
            {
                return (false, "You don't have enough stars");
            }

            ExchangeCardStarToReward(index);
            return (true, "");
        }

        public bool CheckStarEnough(int index)
        {
            return _cardStar.Value >= starExchangeConfig.milestones[index].star;
        }
        public bool CheckAlreadyReceived(int index)
        {
            return index > _cardStarExchangeIndex.Value;
        }

        public bool CanReceiveChest(int index)
        {
            return CheckStarEnough(index) && CheckAlreadyReceived(index);
        }

        private void ExchangeCardStarToReward(int index)
        {
            var milestone = starExchangeConfig.milestones[index];
            var neededStar = milestone.star;
            var reward = milestone.reward;

            _cardStar.Value -= neededStar;
            _cardStarExchangeIndex.Value = index;
            OnStarChanged?.Invoke(_cardStar.Value);

            var logData = new EarnResourceLogData
            {
                spendType = "card_collection_star_exchange",
                spendId = "card_collection_star_exchange",
                isFirstBuy = false,
                source = "non_iap"
            };
            MySonatFramework.inventoryService.AddReward(reward, logData);
        }
    }
}