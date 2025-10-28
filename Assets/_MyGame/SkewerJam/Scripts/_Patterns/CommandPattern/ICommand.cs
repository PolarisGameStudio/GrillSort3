namespace MyGame.SkewerJam.Gameplay.Utils.CommandPattern
{
    public interface IPatternCommand
    {
        void Execute();
        void Undo();
    }
}