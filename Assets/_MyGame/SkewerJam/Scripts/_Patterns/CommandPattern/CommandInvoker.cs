using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay.Command;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Utils.CommandPattern
{
    public class CommandInvoker : MonoBehaviour
    {
        private Stack<IPatternCommand> undoStack = new Stack<IPatternCommand>();
        public event Action OnResetStack;
        public event Action OnAddCommand;
        public void Init()
        {
            ResetStack();
        }

        public void Clear()
        {
            ResetStack();
        }

        public void ExecuteCommand(IPatternCommand command)
        {
            OnAddCommand?.Invoke();
            undoStack.Push(command);
            command.Execute();
        }

        public void UndoCommand()
        {
            if (undoStack.Count > 0)
            {
                IPatternCommand activeCommand = undoStack.Pop();
                if (undoStack.Count == 0)
                {
                    OnResetStack?.Invoke();
                }
                activeCommand.Undo();
            }
        }

        public bool CanUndo()
        {
            return undoStack.Count > 0;
        }

        // reset khi tạo complete order
        public void ResetStack()
        {
            undoStack.Clear();
            OnResetStack?.Invoke();
        }

        public void RemoveCommands(List<Item> listItems)
        {
            var tempStack = new Stack<IPatternCommand>();
            while (undoStack.Count > 0)
            {
                var command = undoStack.Pop();
                var item = (command as SelectItemCommand)?.Item;
                if (listItems.Contains(item) == false)
                {
                    tempStack.Push(command);
                }
                else
                {
                    listItems.Remove(item);
                }

                if (listItems.Count == 0)
                {
                    break;
                }
            }

            while (tempStack.Count > 0)
            {
                undoStack.Push(tempStack.Pop());
            }

            if (undoStack.Count == 0)
            {
                OnResetStack?.Invoke();
            }
        }
    }
}