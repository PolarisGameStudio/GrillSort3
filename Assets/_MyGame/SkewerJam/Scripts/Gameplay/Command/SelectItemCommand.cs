using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay.Utils.CommandPattern;
using MyGame.SkewerJam.Objects.Entities;

namespace MyGame.SkewerJam.Gameplay.Command
{
    public class SelectItemCommand : IPatternCommand
    {
        private Item _item;
        private SlotBase _targetSlot;
        private SlotBase _sourceSlot;
        private GrillData _preGrillData;

        public Item Item => _item;

        public SelectItemCommand(Item item, SlotBase slot)
        {
            _item = item;
            _targetSlot = slot;
            _sourceSlot = item.Slot;

            var grill = _sourceSlot.GetGrill() as PrimaryGrill;
            _preGrillData = grill.GetGrillData();
        }
        public void Execute()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.SwitchSlot(_item, _targetSlot);
        }

        public void Undo()
        {
            UndoAsync().Forget();
        }

        private async UniTask UndoAsync()
        {
            await TryUndoPrimaryGrill();

            // nhảy item về vị trí cũ
            _item.Moving = false;
            _item.SetLockState(false);
            _item.UndoSwitchSlot(_sourceSlot);

            // nếu item đang ở order thì hiện lại target mờ cho order
            TryUndoOrder();
        }

        private void TryUndoOrder()
        {
            if (_targetSlot.GetGrill() is OrderEntity)
            {
                var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
                var orderEntity = _targetSlot.GetGrill() as OrderEntity;
                orderManager.Undo(orderEntity, _targetSlot);
            }
        }

        private async UniTask TryUndoPrimaryGrill()
        {
            var grill = _sourceSlot.GetGrill() as PrimaryGrill;
            if (grill != null)
            {
                await grill.UndoUpdateSubGrills(_preGrillData);
            }
        }


    }
}