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
        BoosterUndo_Test,
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
            }

            return GameResourceType.None;
        }
    }
}