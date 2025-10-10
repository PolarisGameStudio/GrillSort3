namespace Sonat.Enums
{
    public enum GameResource : byte
    {
        None,
        Coin = 1,
        Lives = 2,
        LivesService_SingleLive,
        Star,
        MAX,
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
            }

            return GameResourceType.None;
        }
    }
}