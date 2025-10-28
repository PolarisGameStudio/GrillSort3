using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Utils.CommandPattern
{
    public class CommandInvoker : MonoBehaviour
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            undoStack.Push(command);
        }

        public void UndoCommand()
        {
            if (undoStack.Count > 0)
            {
                ICommand activeCommand = undoStack.Pop();
                activeCommand.Undo();
            }
        }
    }
}