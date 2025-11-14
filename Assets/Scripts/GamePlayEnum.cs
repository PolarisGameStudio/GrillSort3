
using Sonat.Attributes;

namespace Sonat.Enums
{
    public enum GameMode : byte
    {
        Classic
    }

    public enum LevelDifficulty : byte
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        SuperHard = 3,
        MAX = 4,
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
        CardCollection,
        Leaderboard,
    }

    public enum StuckType : byte
    {
        Stuck = 0,
        OutOfMove = 1,
        OutOfMove_AllLockedItem = 2
    }
}