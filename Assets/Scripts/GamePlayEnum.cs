
namespace Sonat.Enums
{
    public enum GameMode : byte
    {
        Classic
    }

    public enum LevelDifficulty : byte
    {
        Easy,
        Medium,
        Hard
    }

    public enum GameState : byte
    {
        Loading = 0,
        Playing,
        Paused,
        GameOver,
        Tool
    }

    public enum GamePlacement : byte
    {
        Loading,
        Gameplay,
        Home
    }

    public enum NavigationType : byte
    {
        None = 0,
        Home,
        Shop,
        Leaderboard,
    }

    public enum StuckType : byte
    {
        Stuck = 0,
        OutOfMove = 1,
        OutOfTime = 2
    }
}