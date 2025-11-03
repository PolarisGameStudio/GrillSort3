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
        private IntDataPref _isCompleteCardCollection;
        private IntDataPref _numCompleteCardCollection;
        private IntDataPref _cardStar;

        public int NumberStar => _cardStar.Value;

        public event Action<int> OnStarCountChanged;

        private int _numStarView = 0;

        public void LoadData()
        {
            _cardStarExchangeIndex = new IntDataPref($"{DATA_KEY}_cardStarExchangeIndex", -1);
            _isCompleteCardCollection = new IntDataPref($"{DATA_KEY}_isCompleteCardCollection", 0);
            _numCompleteCardCollection = new IntDataPref($"{DATA_KEY}_numCompleteCardCollection", _isCompleteCardCollection.BoolValue ? 1 : 0);
            _cardStar = new IntDataPref($"{DATA_KEY}_cardStar", 0);

            _numStarView = _cardStar.Value;
        }

        public void SaveData()
        {
            _cardStarExchangeIndex.Value = _cardStarExchangeIndex.Value;
            _isCompleteCardCollection.Value = _isCompleteCardCollection.Value;
            _numCompleteCardCollection.Value = _numCompleteCardCollection.Value;
            _cardStar.Value = _cardStar.Value;
        }


        public void AddCardStar(int numStar)
        {
            _cardStar.Value += numStar;
            SaveData();
        }

        public bool OnClickReceiveChest(int index)
        {
            if (CanReceiveChest(index) == false)
            {
                PopupToast.Cretate("You don't have enough stars");
                return false;
            }
            else
            {
                ExchangeCardStarToReward(index);
                return true;
            }
        }

        public bool CanReceiveChest(int index)
        {
            return _cardStar.Value >= starExchangeConfig.milestones[index].star && index > _cardStarExchangeIndex.Value;
        }

        public void ExchangeCardStarToReward(int index)
        {
            var milestone = starExchangeConfig.milestones[index];
            var neededStar = milestone.star;
            var reward = milestone.reward;

            _cardStar.Value -= neededStar;
            _cardStarExchangeIndex.Value = index;
            SaveData();

            var logData = new EarnResourceLogData
            {
                spendType = "card_collection_star_exchange",
                spendId = "card_collection_star_exchange",
                isFirstBuy = false,
                source = "non_iap"
            };
            MySonatFramework.inventoryService.AddReward(reward, logData);

            UIData uiData = new UIData();
            uiData.Add(PopupReward.REWARD_KEY, reward);
            PanelManager.Instance.OpenPanel<PopupReward>(uiData);
        }

        public void AddCardStarView(int numStar)
        {
            OnStarCountChanged?.Invoke(numStar);
        }
    }
}