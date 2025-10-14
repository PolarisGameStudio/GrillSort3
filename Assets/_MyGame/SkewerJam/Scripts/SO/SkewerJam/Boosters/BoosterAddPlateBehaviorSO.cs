using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        public override bool CanUseBooster()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            return waitingGrillManager.ListWaitingGrills.Count() < maxPlate;
        }

        public override async UniTask UseBooster(Vector3 position)
        {
            await PlayBoosterAnim(position);
            await UniTask.Delay((int)(delay * 1000));
            var gameLogicHanlder = GameController.Instance.GameLogicHandler;
            var waitingManager = gameLogicHanlder.WaitingGrillManager;
            await waitingManager.AddPlate();
            MySonatFramework.GetService<VibrationService>().Vibrate(50);
        }

        protected override async UniTask PlayBoosterAnim(Vector3 position)
        {
            var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim_BoosterAddPlate>(
                "BoosterAnimAddPlate",
                PanelManager.Instance.transform);

            var targetPosition = GameplayHelper.GetNewWaitingGrillPosition();
            boosterAnim.SetTargetPosition(targetPosition);
            boosterAnim.SetData(position);

        }
    }
}