using System.Collections.Generic;
using Sonat.Enums;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
namespace MyGame.Modules.CardCollection
{
    public static class CheatCardHelper
    {
        public static void ForceUnboxAllCard()
        {
            var cardCollectionService = SonatSystem.GetService<CardCollectionService>();
            var cardInventoryModule = cardCollectionService.CardInventoryModule;
            var config = cardCollectionService.config;
            foreach (var card in config.cards)
            {
                if (cardInventoryModule.CheckExistCollectedCard(card.type)) continue;
                cardInventoryModule.CollectCard(card.type);
            }
        }
        public static void ForceUnboxPackCard(CardType forceCardType)
        {
            var cardCollectionService = SonatSystem.GetService<CardCollectionService>();

            var cardList = new List<CardType>() { forceCardType };
            cardCollectionService.ReceiveCards(cardList, GameResource.Card_Special);

            EventBus<AddItemEvent>.Raise(new AddItemEvent()
            {
                resource = GameResource.Card_Randomx1,
                quantity = 1
            });
        }

        public static void ForceUnboxPackCard(AlbumType forceAlbumType)
        {
            var cardCollectionService = SonatSystem.GetService<CardCollectionService>();
            var cardInventoryModule = cardCollectionService.CardInventoryModule;

            var albumConfig = cardCollectionService.config.GetAlbumConfig(forceAlbumType);
            var cardList = new List<CardType>();
            foreach (var cardType in albumConfig.cards)
            {
                if (cardInventoryModule.CheckExistCollectedCard(cardType) == false)
                {
                    cardList.Add(cardType);
                    if (cardList.Count >= 6)
                    {
                        break;
                    }
                }
            }

            while (cardList.Count < 6)
            {
                cardList.Add(albumConfig.cards.Rand());
            }

            cardCollectionService.ReceiveCards(cardList, GameResource.Card_Special);

            EventBus<AddItemEvent>.Raise(new AddItemEvent()
            {
                resource = GameResource.Card_Randomx1,
                quantity = 1
            });
        }
    }
}