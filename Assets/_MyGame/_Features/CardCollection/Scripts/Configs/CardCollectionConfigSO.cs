using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardCollectionConfigSO", menuName = "MyGame/SkewerJam/Features/CardCollection/CardCollectionConfigSO")]
    public class CardCollectionConfigSO : ScriptableObject
    {
        public int unlockLevel;
        // public LiveOpsPackData liveOpsPackData;

        [Header("Config expire")]
        public int durationMonth = 3;

        [Space(10)]
        [Header("Config reward")]
        public RewardData rewardInSeason;

        [Space(10)]
        public int maxStarView = 5;
        public int numStarShowSpecialFrame = 6;
        public List<int> listNumStarShowGoldFrame = new List<int> { 5, 4 };

        [Space(10)]
        public List<AlbumConfigSO> albums;
        public List<CardConfigSO> cards;

        [Space(10)]
        public RewardData RewardUnlock;

        private Dictionary<AlbumType, AlbumConfigSO> _albumConfigs = new();
        private Dictionary<CardType, CardConfigSO> _cardConfigs = new();
        private Dictionary<CardType, AlbumType> _albumTypeByCard = new();
        private int _maxStar;

        public int MaxStar => _maxStar;

        public async UniTask InitializeAsync()
        {
            foreach (var album in albums)
            {
                _albumConfigs.Add(album.type, album);
            }

            _maxStar = 0;
            foreach (var card in cards)
            {
                _maxStar = Mathf.Max(_maxStar, card.star);
                _cardConfigs.Add(card.type, card);
                _albumTypeByCard.Add(card.type, albums.Find(e => e.cards.Contains(card.type))?.type ?? AlbumType.None);
            }
        }

        public int GetNumCard()
        {
            return cards.Count;
        }

        public AlbumType GetAlbumType(CardType cardType)
        {
            if (_albumTypeByCard.TryGetValue(cardType, out var albumType))
            {
                return albumType;
            }
            albumType = albums.Find(e => e.cards.Contains(cardType))?.type ?? AlbumType.None;
            _albumTypeByCard.Add(cardType, albumType);
            return albumType;
        }

        public AlbumConfigSO GetAlbumConfig(AlbumType albumType)
        {
            if (_albumConfigs.TryGetValue(albumType, out var albumConfig))
            {
                return albumConfig;
            }
            albumConfig = albums.Find(album => album.type == albumType);
            _albumConfigs.Add(albumType, albumConfig);
            return albumConfig;
        }

        public CardConfigSO GetCardConfig(CardType cardType)
        {
            if (_cardConfigs.TryGetValue(cardType, out var cardConfig))
            {
                return cardConfig;
            }
            cardConfig = cards.Find(card => card.type == cardType);
            _cardConfigs.Add(cardType, cardConfig);
            return cardConfig;
        }
    }
}