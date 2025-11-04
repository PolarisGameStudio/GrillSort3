using System;
using System.Collections.Generic;
using System.Linq;
using SonatFramework.Scripts.Helper;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardInventoryModule", menuName = "MyGame/SkewerJam/Features/CardCollection/CardInventoryModule")]
    public class CardInventoryModule : ScriptableObject
    {
        [SerializeField] private CardCollectionConfigSO config;
        public const string DATA_KEY = "CARD_COLLECTION_CARD_INVENTORY_MODULE";

        private Dictionary<AlbumType, ListDataPref<int>> _dictAlbumAndCollectedCard = new();
        private Dictionary<AlbumType, ListDataPref<int>> _dictAlbumAndNewCard = new();

        private IntDataPref completedCardCollection;

        public bool CompletedCardCollection => completedCardCollection.Value == 1;

        public void LoadData()
        {
            _dictAlbumAndCollectedCard = new Dictionary<AlbumType, ListDataPref<int>>();
            _dictAlbumAndNewCard = new Dictionary<AlbumType, ListDataPref<int>>();

            foreach (var album in config.albums)
            {
                _dictAlbumAndCollectedCard.Add(album.type, new ListDataPref<int>($"{DATA_KEY}_collectedCardInAlbum_{album.type}"));
                _dictAlbumAndNewCard.Add(album.type, new ListDataPref<int>($"{DATA_KEY}_newCardInAlbum_{album.type}"));
            }

            completedCardCollection = new IntDataPref($"{DATA_KEY}_completedCardCollection", 0);
        }

        public bool CheckExistCollectedCard(CardType cardType)
        {
            var albumType = config.GetAlbumType(cardType);
            return _dictAlbumAndCollectedCard[albumType].Value.Contains((int)cardType);
        }

        public void CollectCard(CardType cardType)
        {
            var albumType = config.GetAlbumType(cardType);
            _dictAlbumAndCollectedCard[albumType].Add((int)cardType);
            _dictAlbumAndNewCard[albumType].Add((int)cardType);
        }


        public int GetNumberCollectedCardInAlbum(AlbumType albumType)
        {
            return _dictAlbumAndCollectedCard[albumType].Value.Count;
        }

        public int GetNumberNewCardInAlbum(AlbumType albumType)
        {
            return _dictAlbumAndNewCard[albumType].Value.Count;
        }

        public int GetTotalCards()
        {
            return _dictAlbumAndCollectedCard.Values.Sum(e => e.Value.Count);
        }

        public int GetTotalNewCards()
        {
            return _dictAlbumAndNewCard.Values.Sum(e => e.Value.Count);
        }

        public void RemoveNewCard(CardType cardType)
        {
            var albumType = config.GetAlbumType(cardType);
            _dictAlbumAndNewCard[albumType].Remove((int)cardType);
        }

        public bool IsNewCard(CardType cardType)
        {
            var albumType = config.GetAlbumType(cardType);
            return _dictAlbumAndNewCard[albumType].Contains((int)cardType);
        }

        public bool CheckCompleteAlbum(AlbumType albumType)
        {
            return _dictAlbumAndCollectedCard[albumType].Value.Count == config.GetAlbumConfig(albumType).cards.Count;
        }

        public bool CheckAllAlbumComplete()
        {
            foreach (var album in config.albums)
            {
                if (CheckCompleteAlbum(album.type) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetCompleteCardCollection(bool isComplete)
        {
            completedCardCollection.Value = isComplete ? 1 : 0;
        }
    }
}