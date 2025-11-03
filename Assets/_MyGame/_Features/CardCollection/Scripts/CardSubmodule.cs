using System;
using System.Collections.Generic;
using System.Linq;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems;
using SonatFramework.Systems.GameDataManagement;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardSubmodule", menuName = "MyGame/SkewerJam/Features/CardCollection/CardSubmodule")]
    public class CardSubmodule : ScriptableObject
    {
        public const string DATA_KEY = "CARD_COLLECTION_CARD_SUBMODULE";

        private ListDataPref<int> _collectedCardsData;
        private ListDataPref<int> _newCardData;

        private HashSet<CardType> _collectedCardTypes = new();
        private HashSet<CardType> _newCardTypes = new();

        public int TotalCard => _collectedCardTypes.Count;
        public int TotalNewCard => _newCardTypes.Count;
        public HashSet<CardType> CollectedCardTypes => _collectedCardTypes;
        public HashSet<CardType> NewCardTypes => _newCardTypes;

        public event Action OnNewCardCountChanged;

        public void LoadData()
        {
            // collected cards
            _collectedCardsData = new ListDataPref<int>($"{DATA_KEY}_collectedCards");
            if (_collectedCardsData.Value.Count > 0)
            {
                _collectedCardTypes = new HashSet<CardType>(_collectedCardsData.Value.Select(e => (CardType)e));
            }

            // new cards
            _newCardData = new ListDataPref<int>($"{DATA_KEY}_newCards");
            if (_newCardData.Value.Count > 0)
            {
                _newCardTypes = new HashSet<CardType>(_newCardData.Value.Select(e => (CardType)e));
            }
        }

        public void SaveData()
        {
            _collectedCardsData.Value = _collectedCardTypes.Select(e => (int)e).ToList();
            _newCardData.Value = _newCardTypes.Select(e => (int)e).ToList();
        }


        public void RemoveNewCard(CardType cardType)
        {
            _newCardTypes.Remove(cardType);

            SaveData();
            OnNewCardCountChanged?.Invoke();
        }


        public bool IsNewCard(CardType cardType)
        {
            return _newCardTypes.Contains(cardType);
        }

        public bool IsNewCardButNotSeen(CardType cardType)
        {
            return _newCardTypes.Contains(cardType);
        }

        public void AddCard(CardType cardType, int numCard)
        {
            _newCardTypes.Add(cardType);
            SaveData();
            OnNewCardCountChanged?.Invoke();
        }

        public int GetNumCard(CardType cardType)
        {
            return _collectedCardTypes.Count(type => type == cardType);
        }

    }
}