using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SO.Boosters;
using SkewerJam.Gameplay.Effect;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Systems.SettingsManagement.Vibation;
using UnityEngine;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterAddPlateBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterAddPlateBehaviorSO")]
    public class BoosterAddPlateBehaviorSO : BaseBoosterBehaviorSO
    {
        [SerializeField] private int maxPlate = 10;
        [SerializeField] private float delay = 2f;
        public override GameResource boosterType => GameResource.BoosterAddPlate;
        public int MaxPlate => maxPlate;

        #region Behavior
        public override (bool canUse, string reason) CanUseBooster()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            if (waitingGrillManager.ListWaitingGrills.Count() >= maxPlate) return (false, "Max plate reached");
            return (true, "");
        }

        public override async UniTask<bool> UseBooster(Vector3 position, bool isForce = false)
        {
            if (isForce == false)
            {
                await PlayBoosterAnim(position);
            }

            await UniTask.Delay((int)(delay * 1000));
            var gameLogicHanlder = GameController.Instance.GameLogicHandler;
            var waitingManager = gameLogicHanlder.WaitingGrillManager;
            await waitingManager.AddPlate();
            MySonatFramework.GetService<VibrationService>().Vibrate(50);

            WaitingGrillHelper.ResetWarning();
            return true;
        }

        public override async UniTask<bool> ForceUseBooster()
        {
            return false;
        }

        protected override async UniTask PlayBoosterAnim(Vector3 position)
        {
            var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim_BoosterAddPlate>(
                "BoosterAnimAddPlate",
                PanelManager.Instance.transform);

            var targetPosition = GameplayHelper.GetNewWaitingGrillPosition(maxPlate);
            boosterAnim.SetTargetPosition(targetPosition);
            boosterAnim.SetData(position);

        }

        #endregion

        public override bool CheckSuggest()
        {
            // nếu chỉ còn 1 plate trống
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            var currentMaxPlate = waitingGrillManager.ListWaitingGrills.Count();
            var noEmtpyPlate = waitingGrillManager.ListWaitingGrills.Where(e => e.GetSlot(0).GetItem() != null).Count();
            var remainingPlate = currentMaxPlate - noEmtpyPlate;

            // nếu ăn order hiện tại cần thêm 1 slot nữa có thể ăn
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrderItems = orderManager.GetOrderItemsDict();
            var grills = GameController.Instance.GameLogicHandler.GrillManager.ListGrills;

            // quét layer1
            foreach (var grill in grills)
            {
                if (grill.IsLock) continue;
                var slots = grill.GetSlots();
                foreach (var slot in slots)
                {
                    if (slot.GetItem() == null) continue;
                    var item = slot.GetItem();
                    if (item.IsLocked) continue;
                    if (dictOrderItems.ContainsKey((ItemId)item.id))
                    {
                        return false;
                    }
                }
            }

            // quét layer2
            foreach (var grill in grills)
            {
                if (grill.IsLock) continue;
                var subGrills = grill.GetSubGrills();
                if (subGrills == null || subGrills.Count == 0) continue;

                var slotsInPrimaryGrill = grill.GetSlots();
                var numSlotsInPrimaryGrill = slotsInPrimaryGrill.Count(e => e.GetItem() != null);
                if (numSlotsInPrimaryGrill > remainingPlate) continue;

                var slots = subGrills[0].GetSlots();
                foreach (var slot in slots)
                {
                    if (slot.GetItem() == null) continue;
                    var item = slot.GetItem();
                    if (item.IsLocked) continue;
                    if (dictOrderItems.ContainsKey((ItemId)item.id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override async UniTask PostUseBooster()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var orderManager = gameLogicHandler.OrderManager;
            orderManager.LogicOrderHandler.SetForceRescue(true);
        }
    }
}