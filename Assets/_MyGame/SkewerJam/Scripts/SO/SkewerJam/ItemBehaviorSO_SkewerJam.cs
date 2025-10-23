using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay;
using UnityEngine;
using DG.Tweening;
using Sonat.Enums;
using Gameplay.Entities.Items;
using Sirenix.OdinInspector;
using SonatFramework.Systems.SettingsManagement.Vibation;
using Gameplay.LevelData;
using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Utils;
using MyGame.SkewerJam.Scripts.SO.Configs;

namespace MyGame.SkewerJam.Scripts.SO.Behavior
{
    [CreateAssetMenu(fileName = "ItemBehaviorSO_SkewerJam", menuName = "MyGame/SkewerJam/ItemBehaviorSO_SkewerJam")]
    public class ItemBehaviorSO_SkewerJam : ItemBehaviorSO
    {
        [Space(10)]
        [Header("Animation")]
        [SerializeField] private ItemAnimationConfigSO_SkewerJam itemAnimConfig;

        private Item selectedItem;

        public override void OnSelected(Item item)
        {
            item.Visual.OnSelected();
            item.Slot.OnItemSelected();
        }

        public override void PlayDropSound(Item item)
        {
            // if (item.Slot.GetGrill().grillType == GrillType.Broken) return;
            // MySonatFramework.audioService.PlaySound(AudioId.Items_Pick_SMode_HLW_Grill_sort);
        }

        public override void OnMouseDown(Item item)
        {
            if (!item.isPrimary
            || item.IsMoveToPrimary == true
            || item.IsSelected
            || item.IsLocked
            || GameController.Instance.GameState != GameState.Playing
            || item.Slot.GetGrill().IsLock) return;

            selectedItem = item;
            item.DOKill();
            item.transform.DOScale(itemAnimConfig.scaleDown, itemAnimConfig.scaleDuration);
        }

        public override void OnMouseExit(Item item)
        {
            if (selectedItem == item)
            {
                selectedItem = null;
            }
            item.DOKill();
            item.transform.DOScale(Vector3.one, itemAnimConfig.scaleDuration);
        }

        public override void OnMouseUp(Item item)
        {
            if (GameController.Instance.GameState != GameState.Playing) return;

            if (selectedItem == item)
            {
                var gameLogicHandler = GameController.Instance.GameLogicHandler;
                var switchSuccess = gameLogicHandler.SelectItem(item);
                // MySonatFramework.GetService<VibrationService>().Vibrate(50);
                MySonatFramework.audioService.PlaySound(AudioId.Items_Pick_SMode_HLW_Grill_sort);
                if (switchSuccess == false)
                {
                    item.transform.DOKill();
                    item.transform.DOScale(Vector3.one, itemAnimConfig.scaleDuration);
                }
            }
            else
            {
                if (selectedItem != null)
                {
                    selectedItem.transform.DOKill();
                    selectedItem.transform.DOScale(Vector3.one, itemAnimConfig.scaleDuration);
                }
            }
            selectedItem = null;

        }

        public override void SwitchSlot(Item item, SlotBase slot)
        {
            item.transform.DOKill();
            if (item.Slot != null)
            {
                item.Slot.ItemOut();
            }

            item.SetSlot(slot);
            item.SetSelected(true);
            slot.AddItem(item);
            item.Visual.OnDeselected();
            item.Visual.SetSortingOrder(1);

            var seq = DOTween.Sequence();
            if (itemAnimConfig.useSpeed)
            {
                seq.Join(item.transform.DOLocalMoveX(0, itemAnimConfig.speed).SetSpeedBased(itemAnimConfig.useSpeed).SetEase(itemAnimConfig.curveX));
                seq.Join(item.transform.DOLocalMoveY(0, itemAnimConfig.speed).SetSpeedBased(itemAnimConfig.useSpeed).SetEase(itemAnimConfig.curveY));
            }
            else
            {
                seq.Join(item.transform.DOLocalMoveX(0, itemAnimConfig.durationMove).SetEase(itemAnimConfig.curveX));
                seq.Join(item.transform.DOLocalMoveY(0, itemAnimConfig.durationMove).SetEase(itemAnimConfig.curveY));
            }
            seq.Append(item.transform.DOScale(itemAnimConfig.scaleDown, itemAnimConfig.scaleDuration));
            seq.Append(item.transform.DOScale(Vector3.one, itemAnimConfig.scaleDuration));

            var currentItem = item;
            seq.OnComplete(() =>
            {
                currentItem.SetSelected(false);
                currentItem.OnDropToSlot(slot);
                currentItem.Visual.SetSortingOrder(0);
                // MySonatFramework.GetService<VibrationService>().Vibrate(50);
                GameController.Instance.GameLogicHandler.ItemMoveSlot(currentItem, slot);

                foreach (Transform child in slot.Container)
                {
                    if (child.gameObject != currentItem.gameObject)
                    {
                        GameFactory.Instance.ReturnEntity(child.GetComponent<Item>());
                    }
                }
            });
        }

        public override void OnExplodeBomb(ItemBombMove itemBombMove)
        {
            SonatUtils.DelayCall(1f, () =>
            {
                GameController.Instance.Lose(StuckType.OutOfMove);
            }, itemBombMove);
        }

        public override async UniTask ProcessExplodeBomb(ItemBombMove itemBombMove)
        {
            // GameController.Instance.ChangeGameState(GameState.Paused);
            // await UniTask.WhenAny(
            //     UniTask.WaitUntil(() => GameController.Instance.GameLogicHandler.HasCollectItem == true),
            //     UniTask.Delay(1000)
            // );
            // if (GameController.Instance.GameLogicHandler.HasCollectItem)
            // {
            //     // chờ thêm để xem có order nào ăn item bomb này không  
            //     await UniTask.Delay(4000);
            // }
            // GameController.Instance.ChangeGameState(GameState.Playing);
        }
    }
}