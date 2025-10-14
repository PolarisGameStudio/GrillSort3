using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyGame.SkewerJam.Gameplay;
using DG.Tweening;
using System.Threading.Tasks;
using System.Linq;
using SonatFramework.Systems.SettingsManagement.Vibation;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterFoodBoxBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterFoodBoxBehaviorSO")]
    public class BoosterFoodBoxBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterFoodBox;

        [SerializeField] private float delayBetweenItems = 0.1f;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public override bool CanUseBooster()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            return waitingGrillManager.ListWaitingGrills.Count(e => e.GetSlot(0).GetItem() != null) > 0;
        }

        public override async UniTask UseBooster(Vector3 position)
        {
            await PlayBoosterAnim(position);
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var waitingGrillManager = gameLogicHandler.WaitingGrillManager;
            foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
            {
                var item = waitingGrill.GetSlot(0).GetItem();
                if (item == null) continue;

                waitingGrill.GetSlot(0).SetItem(null);
                // waitingGrill.transform.DOShakePosition(scaleDuration, 0.5f, 10, 90);
                item.transform.DOScale(0, scaleDuration).SetEase(Ease.InBack).OnComplete(() =>
                {
                    MySonatFramework.GetService<VibrationService>().Vibrate(50);
                    GameFactory.Instance.ReturnEntity(item);
                });
                await UniTask.Delay((int)(delayBetweenItems * 1000));
            }
        }
    }
}