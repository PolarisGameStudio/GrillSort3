using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;
namespace MyGame.Modules.CardCollection
{
    public static class CardPackHelper
    {
        public static int GetNumCardInPack(GameResource packType)
        {
            int result = (int)packType - (int)GameResource.Card_Randomx1 + 1;
            Debug.Log($"[CardPackHelper] packType={packType} ({(int)packType}) → base={(int)GameResource.Card_Randomx1} → result={result}");
            return result;
        }


        public static CardType GetRandomCardType()
        {
            // 70% card 1x
            // 20% card 2x
            // 10% card 3x
            var random = UnityEngine.Random.Range(0, 100);
            var star = 0;

            if (random < 70)
            {
                star = 1;
            }
            else if (random < 90)
            {
                star = 2;
            }
            else
            {
                star = 3;
            }

            // if (star == 0)
            // {
            //     var randomType = UnityEngine.Random.Range(0, (int)CardType.MAX);
            //     return (CardType)randomType;
            // }

            var cardCollectionService = MySonatFramework.GetService<CardCollectionService>();
            var cards = cardCollectionService.config.cards.Where(card => card.star == star).ToList();
            var idx = UnityEngine.Random.Range(0, cards.Count);
            return cards[idx].type;
        }


        public static List<CardType> GetCardReward(RewardData rewardData)
        {
            List<CardType> cardList = new();

            foreach (var reward in rewardData.resourceDatas)
            {
                var packType = reward.resource;

                if (GameResourceHelper.ResourceType(reward.resource) == GameResourceType.Card)
                {
                    int quantity = GetNumCardInPack(packType); // ví dụ 6
                    for (int i = 0; i < quantity; i++)
                    {
                        var randomCardType = GetRandomCardType();
                        cardList.Add(randomCardType);
                    }
                }
            }

            return cardList;
        }

        public static List<CardType> GetCardReward(ResourceData resourceData)
        {
            var listCard = new List<CardType>();
            var packType = resourceData.resource;
            int quantity = GetNumCardInPack(packType);
            for (int i = 0; i < quantity; i++)
            {
                var randomCardType = GetRandomCardType();
                listCard.Add(randomCardType);
            }
            return listCard;
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
    }
}