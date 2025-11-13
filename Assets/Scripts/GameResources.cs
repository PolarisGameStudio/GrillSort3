using System;
using MyGame.Modules.CardCollection;

namespace Sonat.Enums
{
    public enum GameResource : byte
    {
        None,
        Coin = 1,
        Lives = 2,
        LivesService_SingleLive,
        Star,

        BoosterUndo = 101,
        BoosterSpatula,
        BoosterShuffle,
        BoosterFoodBox,
        BuffAddPlate, // bổ trợ thêm
        BuffAddOrder, // bổ trợ thêm

        Card_Randomx1 = 120,
        Card_Randomx2 = 121,
        Card_Randomx3 = 122,
        Card_Randomx4 = 123,
        Card_Randomx5 = 124,
        Card_Randomx6 = 125,
        Card_Special = 126,

        Badge_CardCollection = 200,
        MAX = Byte.MaxValue, // 255
    }

    public enum GameResourceType : byte
    {
        None = 0,
        Currency,
        Booster,
        Card,
        Badge,
    }

    public static class GameResourceHelper
    {
        public static GameResourceType ResourceType(this GameResource resource)
        {
            switch (resource)
            {
                case GameResource.Coin:
                case GameResource.Lives:
                    return GameResourceType.Currency;
                case GameResource.BoosterUndo:
                case GameResource.BoosterSpatula:
                case GameResource.BoosterShuffle:
                case GameResource.BoosterFoodBox:
                case GameResource.BuffAddPlate:
                case GameResource.BuffAddOrder:
                    return GameResourceType.Booster;
                case GameResource.Card_Randomx1:
                case GameResource.Card_Randomx2:
                case GameResource.Card_Randomx3:
                case GameResource.Card_Randomx4:
                case GameResource.Card_Randomx5:
                case GameResource.Card_Randomx6:
                case GameResource.Card_Special:
                    return GameResourceType.Card;
                case GameResource.Badge_CardCollection:
                    return GameResourceType.Badge;
            }

            return GameResourceType.None;
        }
    }
}