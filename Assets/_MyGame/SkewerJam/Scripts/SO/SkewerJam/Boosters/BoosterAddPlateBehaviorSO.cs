using System.Linq;
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
        [SerializeField] private int maxPlate = 10;
        public override GameResource boosterType => GameResource.BoosterAddPlate;

        public override bool CanUseBooster()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            return waitingGrillManager.ListWaitingGrills.Count() < maxPlate;
        }

        public override async UniTask UseBooster(Vector3 position)
        {
            await PlayBoosterAnim(position);
            var gameLogicHanlder = GameController.Instance.GameLogicHandler;
            var waitingManager = gameLogicHanlder.WaitingGrillManager;
            await waitingManager.AddPlate();
            MySonatFramework.GetService<VibrationService>().Vibrate(50);
        }
    }
}