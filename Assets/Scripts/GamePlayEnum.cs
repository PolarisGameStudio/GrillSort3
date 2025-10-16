
namespace Sonat.Enums
{
    public enum GameMode : byte
    {
        Classic
    }

    public enum LevelDifficulty : byte
    {
        Normal,
        Hard,
        SuperHard
    }

    public enum LevelType : byte
    {
        Food,
        Flower,
        SkewerJam
    }

    public enum GameState : byte
    {
        Loading = 0,
        Playing,
        Paused,
        GameOver,
        Tool,
        UsingBooster
    }

    public enum GameResult : byte
    {
        None,
        Win,
        Lose,
    }

    public enum GamePlacement : byte
    {
        Loading,
        Gameplay_SkewerJam,
        Home
    }

    public enum NavigationType : byte
    {
        None = 0,
        Home,
        Shop,
        Settings,
        Leaderboard,
    }

    public enum StuckType : byte
    {
        Stuck = 0,
        OutOfMove = 1,
        OutOfTime = 2
    }
}