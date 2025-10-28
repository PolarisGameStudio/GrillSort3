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
    [CreateAssetMenu(fileName = "BoosterUndoBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterUndoBehaviorSO")]
    public class BoosterUndoBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterUndo;

        #region Behavior
        public override (bool canUse, string reason) CanUseBooster()
        {
            return (true, "");
        }

        public override async UniTask<bool> UseBooster(Vector3 position, bool isForce = false)
        {
            // if (isForce == false)
            // {
            //     await PlayBoosterAnim(position);
            // }

            // await UniTask.Delay((int)(delay * 1000));
            // var gameLogicHanlder = GameController.Instance.GameLogicHandler;
            // var waitingManager = gameLogicHanlder.WaitingGrillManager;
            // await waitingManager.AddPlate();
            // MySonatFramework.GetService<VibrationService>().Vibrate(50);
            PopupToast.Cretate("Undo!!!");
            await UniTask.Delay(2000);
            return true;
        }

        public override async UniTask<bool> ForceUseBooster()
        {
            return false;
        }

        protected override async UniTask PlayBoosterAnim(Vector3 position)
        {
            // var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim_BoosterAddPlate>(
            //     "BoosterAnimAddPlate",
            //     PanelManager.Instance.transform);

            // var targetPosition = GameplayHelper.GetNewWaitingGrillPosition();
            // boosterAnim.SetTargetPosition(targetPosition);
            // boosterAnim.SetData(position);

        }

        #endregion

        public override bool CheckSuggest()
        {
            return true;
        }

        public override async UniTask PostUseBooster()
        {
            // var gameLogicHandler = GameController.Instance.GameLogicHandler;
            // var orderManager = gameLogicHandler.OrderManager;
            // orderManager.LogicOrderHandler.SetForceRescue(true);
        }
    }
}