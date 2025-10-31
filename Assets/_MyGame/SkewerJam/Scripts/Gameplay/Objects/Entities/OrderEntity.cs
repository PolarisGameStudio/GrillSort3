using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BoosteeManagement;
using Gameplay.Entities;
using Gameplay.LevelData;
using Manager;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using UnityEngine;
using static PopupUnlockInGame;

namespace MyGame.SkewerJam.Objects.Entities
{
    public class OrderEntity : GrillBase
    {
        public enum OrderEntityState
        {
            Waiting,
            Ready,
            Complete,
        }
        [Header("Order Entity Visual")]
        [SerializeField] private Transform container;
        [SerializeField] private OrderEntityVisual orderEntityVisual;
        private int orderIndex;
        private bool active = false; // đã unlock chưa
        private OrderEntityState state = OrderEntityState.Waiting;

        // private bool ready = false; // đã sẵn sàng nhận item chưa
        // private bool moving = false; // đang di chuyển không
        // private bool complete = false; // đã hoàn thành order chưa
        private int completeCount = 0; // số lượng item đã hoàn thành

        private ItemId itemIdTarget = ItemId.None;
        private int maxItems = 0;
        private LogicOrderType logicOrderType = LogicOrderType.None;


        public int MaxItems => maxItems;
        public int CurrentItems => slots.Count(e => e.GetItem() != null);
        public LogicOrderType LogicOrderType => logicOrderType;

        public ItemId ItemIdTarget => itemIdTarget;
        public bool IsActive => active;
        public int OrderIndex => orderIndex;
        public OrderEntityState State { get => state; set => state = value; }
        // public bool Ready { get => ready; set => ready = value; }
        // public bool Moving { get => moving; set => moving = value; }
        // public bool Complete { get => complete; set => complete = value; }
        public int CompleteCount { get => completeCount; set => completeCount = value; }

        #region Implementations
        public override EntityType entityType => EntityType.PrimaryGrill;

        public OrderEntityVisual Visual => orderEntityVisual;

        public override void ChangeItem(SlotBase slot, int newId)
        {

        }

        public override bool CheckItemWithId(int id)
        {
            return false;
        }

        public override Vector3 DestroyItem(int id)
        {
            return Vector3.zero;
        }

        public override ShuffleLayerData GetMagnetLayerData()
        {
            return null;
        }

        public override ShuffleLayerData GetShuffleLayerData()
        {
            return null;
        }

        public override void SetShuffleLayerData(LayerData layerData)
        {

        }
        #endregion

        public void Init(bool active)
        {
            SetActive(active);

            state = OrderEntityState.Waiting;
            // ready = false;
            // complete = false;
            // moving = false;

            completeCount = 0;
            transform.localScale = Vector3.one;
            itemIdTarget = ItemId.None;
        }

        public void SetData(ItemId itemId, int num, LogicOrderType logicOrderType)
        {
            SetTargetItem(itemId, num, logicOrderType);
        }

        public void SetActive(bool active)
        {
            this.active = active;
            orderEntityVisual.SetActive(active);
        }

        public void SetOrderIndex(int index)
        {
            orderIndex = index;
        }

        public void SetTargetItem(ItemId itemId, int num, LogicOrderType logicOrderType)
        {
            itemIdTarget = itemId;
            maxItems = num;
            this.logicOrderType = logicOrderType;
            checkCreateItemFaded = false;
            ShowTargetItem().Forget();
        }

        private bool checkCreateItemFaded = false;
        private async UniTask ShowTargetItem()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i >= maxItems)
                {
                    var slot = slots[i];
                    slot.ClearItem();
                    continue;
                }
                else
                {
                    ShowTargetItemOnSlot(slots[i]).Forget();
                }
            }
            checkCreateItemFaded = true;

            // // căn lại ví trí slot
            // var dist = slots[1].transform.localPosition.x - slots[0].transform.localPosition.x;
            // var startPos = -(maxItems - 1) * dist / 2;
            // for (int i = 0; i < slots.Length; i++)
            // {
            //     var slot = slots[i];
            //     slot.transform.localPosition = new Vector3(startPos + dist * i, 0, 0);
            // }
        }

        public async UniTask ShowTargetItemOnSlot(SlotBase slot)
        {
            var itemData = new ItemData()
            {
                itemType = ItemType.Normal,
                id = (int)itemIdTarget,
            };
            var item = await GrillBaseBehaviorSO.gameFactorySO.CreateItem<Item>($"ItemFaded");
            item.SetPrimary(entityType == EntityType.PrimaryGrill);
            item.SetItemData(itemData, slot);
            item.SetIsOnConveyor(isOnConveyor);
            item.SetLockState(true);

            slot.ClearItem();
            slot.SetItem(item); // căn đúng vị trí

            slot.SetItem(null); // không tồn tại trong slot
        }

        public SlotBase GetAvailableSlot()
        {
            for (int i = 0; i < maxItems; i++)
            {
                var slot = slots[i];
                if (slot.GetItem() == null)
                {
                    return slot;
                }
            }
            return null;
        }

        public bool CheckComplete()
        {
            var slots = GetSlots();
            for (int i = 0; i < maxItems; i++)
            {
                var slot = slots[i];
                if (slot.GetItem() == null)
                {
                    return false;
                }
            }
            return true;
        }

        public void PlayComplete(Action onComplete)
        {
            foreach (var slot in slots)
            {
                slot.GetItem()?.OnComplete();
            }
            orderEntityVisual.PlayComplete(onComplete);
        }

        public override void OnReturnObj()
        {
            base.OnReturnObj();
            foreach (var slot in slots)
            {
                foreach (Transform child in slot.Container)
                {
                    if (child.TryGetComponent<Item>(out var item))
                    {
                        GrillBaseBehaviorSO.gameFactorySO.ReturnEntity(item);
                    }
                }
            }
            itemIdTarget = ItemId.None;
            orderIndex = 0;
            state = OrderEntityState.Waiting;
            // ready = false;

            orderEntityVisual.ResetLid();
            transform.localScale = Vector3.one;
        }

        #region Interact
        private void Update()
        {
            // Khi nhả chuột trái
            //  var popup = PanelManager.Instance.GetPanel<PopupUnlock_SkewerJam>();
            if (GameController.Instance.CheckBlockUI() == false && !active && Input.GetMouseButtonUp(0))
            {
                var hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                foreach (var hit in hits)
                {
                    if (hit.gameObject == gameObject)
                    {
                        OpenPopupUnlock();
                        break;
                    }
                }
            }
        }
        #endregion

        private void OpenPopupUnlock()
        {
            var uiData = new UIData();
            uiData.Add("SelectedObjectType", SelectedObjectType.Tray);
            uiData.Add("Price", GameController.Instance.GameConfig.unlockTrayPrice);
            uiData.Add("OnSuccess", (Action)(() =>
            {
                var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
                orderManager.Unlock(this);
            }));
            PanelManager.Instance.OpenPanel<PopupUnlockInGame>(uiData);
        }

        public async UniTask PlayUnlock(bool isRescue = false)
        {
            SetActive(true);
            orderEntityVisual.OpenGrill(false, true);


            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var orderManager = gameLogicHandler.OrderManager;

            if (orderManager.LogicOrderHandler.CheckCreateNextOrder())
            {
                var (itemId, num, logicOrderType) = await orderManager.LogicOrderHandler.GetItemOrder(isRescue);
                Debug.Log("<color=red>itemId: " + itemId + ", num: " + num + "</color>");
                SetTargetItem(itemId, num, logicOrderType);
                var orderPos = orderManager.LeftStartPos.position;
                orderPos.z = 0;
                container.position = orderPos;

                await UniTask.Delay(300);
                gameLogicHandler.StartMoveNextOrder(this);
                container.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    gameLogicHandler.EndMoveNextOrder(this);
                });
            }
        }
    }
}