namespace MyGame.SkewerJam.Gameplay.Utils.CommandPattern
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}