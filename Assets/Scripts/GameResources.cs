using System;

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
        BoosterAddPlate,

        Card_Randomx1 = 120,
        Card_Randomx2 = 121,
        Card_Randomx3 = 122,
        MAX = Byte.MaxValue, // 255
    }

    public enum GameResourceType : byte
    {
        None = 0,
        Currency,
        Booster,
        Card
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
                    return GameResourceType.Booster;
                case GameResource.Card_Randomx1:
                case GameResource.Card_Randomx2:
                case GameResource.Card_Randomx3:
                    return GameResourceType.Card;
            }

            return GameResourceType.None;
        }
    }
}