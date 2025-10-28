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
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var commandInvoker = gameLogicHandler.CommandInvoker;
            if (commandInvoker.CanUndo())
            {
                return (true, "");
            }
            else
            {
                return (false, "Undo is not available");
            }

        }

        public override async UniTask<bool> UseBooster(Vector3 position, bool isForce = false)
        {
            if (isForce == false)
            {
                await PlayBoosterAnim(position);
            }

            // await UniTask.Delay((int)(delay * 1000));

            MySonatFramework.GetService<VibrationService>().Vibrate(50);
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.CommandInvoker.UndoCommand();

            await UniTask.Delay(500);

            return true;
        }

        public override async UniTask<bool> ForceUseBooster()
        {
            return false;
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

        public override async UniTask PrepareTutorialBooster()
        {
            await UniTask.Delay(1000);

            var listSelectedItems = ItemHelper.SelectItemsForForceBooster(1);
            foreach (var item in listSelectedItems)
            {
                GameController.Instance.GameLogicHandler.SelectItem(item);
            }
            await UniTask.Delay((int)(1000));
        }
    }
}