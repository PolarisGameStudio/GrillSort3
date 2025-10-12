using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyGame.SkewerJam.Gameplay;
using MyGame.SO.Boosters;
using Sonat.Enums;
using SonatFramework.Systems.SettingsManagement.Vibation;
using UnityEngine;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterAddPlateBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterAddPlateBehaviorSO")]
    public class BoosterAddPlateBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterAddPlate;

        public override async UniTask UseBooster()
        {
            var gameLogicHanlder = GameController.Instance.GameLogicHandler;
            var waitingManager = gameLogicHanlder.WaitingGrillManager;
            await waitingManager.AddPlate();
            MySonatFramework.GetService<VibrationService>().Vibrate(50);
        }
    }
}