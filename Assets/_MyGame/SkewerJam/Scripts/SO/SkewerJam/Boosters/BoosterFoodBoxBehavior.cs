using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyGame.SkewerJam.Gameplay;
using DG.Tweening;
using System.Threading.Tasks;
using System.Linq;
using SonatFramework.Systems.SettingsManagement.Vibation;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Scripts.UIModule;
using Gameplay.Entities;
using System.Collections.Generic;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterFoodBoxBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterFoodBoxBehaviorSO")]
    public class BoosterFoodBoxBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterFoodBox;

        [SerializeField] private float delayBetweenItems = 0.1f;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float delay = 0.5f;

        public override (bool canUse, string reason) CanUseBooster()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            if (waitingGrillManager.ListWaitingGrills.All(e => e.GetSlot(0).GetItem() == null)) return (false, "No items on plate");

            return (true, "");
        }

        public override async UniTask UseBooster(Vector3 position)
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var waitingGrillManager = gameLogicHandler.WaitingGrillManager;

            var listItem = new List<Item>();
            foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
            {
                var item = waitingGrill.GetSlot(0).GetItem();
                if (item == null) continue;

                waitingGrill.GetSlot(0).SetItem(null);
                // waitingGrill.transform.DOShakePosition(scaleDuration, 0.5f, 10, 90);
                // item.transform.DOScale(0, scaleDuration).SetEase(Ease.InBack).OnComplete(() =>
                // {
                //     MySonatFramework.GetService<VibrationService>().Vibrate(50);
                //     GameFactory.Instance.ReturnEntity(item);
                // });

                // GameFactory.Instance.ReturnEntity(item);
                listItem.Add(item);
            }

            var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim_BoosterFoodBox>(
                "BoosterAnimFoodBox",
                PanelManager.Instance.transform);
            await boosterAnim.SetData(position, listItem);
            await GameController.Instance.GameLogicHandler.TryCheckWinGame();
        }

        public async UniTask PlayBoosterAnim(Vector3 position)
        {

        }
    }
}