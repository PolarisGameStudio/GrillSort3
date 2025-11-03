using SonatFramework.Scripts.Helper;
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

        public void LoadData()
        {
            _cardStarExchangeIndex = new IntDataPref($"{DATA_KEY}_cardStarExchangeIndex", -1);
            _isCompleteCardCollection = new IntDataPref($"{DATA_KEY}_isCompleteCardCollection", 0);
            _numCompleteCardCollection = new IntDataPref($"{DATA_KEY}_numCompleteCardCollection", _isCompleteCardCollection.BoolValue ? 1 : 0);
            _cardStar = new IntDataPref($"{DATA_KEY}_cardStar", 0);
        }

        public void SaveData()
        {
            _cardStarExchangeIndex.Value = _cardStarExchangeIndex.Value;
            _isCompleteCardCollection.Value = _isCompleteCardCollection.Value;
            _numCompleteCardCollection.Value = _numCompleteCardCollection.Value;
            _cardStar.Value = _cardStar.Value;
        }

        public bool ExchangeCardStarToReward(int index)
        {
            var milestone = starExchangeConfig.milestones[index];
            var neededStar = milestone.star;
            var reward = milestone.reward;
            return true;
        }

        public void AddCardStar(int numStar)
        {
            _cardStar.Value += numStar;
            SaveData();
        }


        public bool CheckHasClaimChest(int idGift)
        {
            return _cardStar.Value >= starExchangeConfig.milestones[idGift].star;
        }

    }
}