using System;
using System.Collections.Generic;
using System.Linq;
using Sonat.Enums;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    public static class CardPackHelper
    {
        public static int GetNumCardInPack(GameResource packType)
        {
            int result = (int)packType - (int)GameResource.Card_Randomx1 + 1;
            // Debug.Log($"[CardPackHelper] packType={packType} ({(int)packType}) → base={(int)GameResource.Card_Randomx1} → result={result}");
            return Mathf.Min(result, 6);
        }

        public static int GetNumberStarOfCard(CardType cardType)
        {
            var config = MySonatFramework.GetService<CardCollectionService>().config;
            var cardConfig = config.cards.FirstOrDefault(e => e.type == cardType);
            return cardConfig.star;

        }

        public static int GetPackIndex(int count)
        {
            return (count - 1) / 2;
        }

        public static CardDataToRandom GetDictCardTypeToRandom()
        {
            var dictCard = new Dictionary<int, (List<CardType> listNewCards, List<CardType> listOldCards)>();
            var listAllCards = new List<CardType>();
            var cardCollectionService = MySonatFramework.GetService<CardCollectionService>();
            var cardInventoryModule = cardCollectionService.CardInventoryModule;
            var config = cardCollectionService.config;
            foreach (var card in config.cards)
            {
                listAllCards.Add(card.type);
                var star = card.star;
                var isNewCard = cardInventoryModule.CheckExistCollectedCard(card.type);

                if (dictCard.ContainsKey(star) == false)
                {
                    dictCard.Add(star, (new List<CardType>(), new List<CardType>()));
                }

                // add dict
                if (isNewCard)
                {
                    dictCard[star].listNewCards.Add(card.type);
                }
                else
                {
                    dictCard[star].listOldCards.Add(card.type);
                }
            }
            return new CardDataToRandom()
            {
                dictCard = dictCard,
                listAllCards = listAllCards
            };

        }
    }

    public class CardDataToRandom
    {
        public Dictionary<int, (List<CardType> listNewCards, List<CardType> listOldCards)> dictCard;
        public List<CardType> listAllCards;
    }
}