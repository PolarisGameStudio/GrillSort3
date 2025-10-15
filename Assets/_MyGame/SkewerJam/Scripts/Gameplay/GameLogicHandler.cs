using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Objects;
using MyGame.SkewerJam.Objects.Entities;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class GameLogicHandler : MonoBehaviour
    {
        private const string LOG_TAG = "<color=yellow>GameLogicHandler: </color>";

        [Header("Core")]
        [SerializeField] private GrillManager grillManager;
        [SerializeField] private WaitingGrillManager waitingGrillManager;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ItemManager itemManager;

        [SerializeField] private BoosterManager boosterManager;

        [Header("Additional")]
        [SerializeField] private ConveyorManager conveyorManager;
        [SerializeField] private ObstacleManager obstacleManager;

        [Header("Utils")]
        [SerializeField] private SuggestManager suggestManager;

        public OrderManager OrderManager => orderManager;
        public WaitingGrillManager WaitingGrillManager => waitingGrillManager;
        public GrillManager GrillManager => grillManager;
        public ConveyorManager ConveyorManager => conveyorManager;
        public ObstacleManager ObstacleManager => obstacleManager;
        public ItemManager ItemManager => itemManager;

        public BoosterManager BoosterManager => boosterManager;

        public Item ItemSelected { get; set; }
        public bool BlockClick { get; set; } = false;

        #region Event Actions
        public event Action<Item, SlotBase> OnItemStartSwitch;
        public event Action<Item, bool> OnItemStartSwitchSucess;

        public event Action<Item, SlotBase> OnItemEndSwitch;


        public event Action<OrderEntity> OnAppearNextOrder;
        public event Action<int> OnAppearNextOrderItem;
        public event Action<int> OnCollectItem;
        public event Action<OrderEntity> OnStartCollectItem;
        public event Action<OrderEntity> OnEndCollectItem;
        #endregion

        private int pumpkin = 0;
        public int Pumpkin => pumpkin;


        public void Init()
        {
            grillManager.Init();
            waitingGrillManager.Init();
            orderManager.Init();

            conveyorManager.Init();
            obstacleManager.Init();

            suggestManager.Init();

            pumpkin = 0;
            BlockClick = false;
        }

        public void Clear()
        {
            pumpkin = 0;
            orderManager.Clear();
            waitingGrillManager.Clear();
            grillManager.Clear();
            itemManager.Clear();

            conveyorManager.Clear();
            obstacleManager.Clear();

            suggestManager.Clear();
        }

        public bool CheckEnergy()
        {
            // var energy = MySonatFramework.GetService<InventoryService>().GetResource(GameResource.Energy);
            // if (energy <= 0 && GameController.Instance.GameState == GameState.Playing)
            // {
            //     GameController.Instance.ChangeGameState(GameState.Paused);
            //     PanelManager.Instance.OpenPanel<PopupWarningEnergy_SkewerJam>(new UIData().Add("GamePlacement", GamePlacement.Gameplay_SkewerJam));
            //     return false;
            // }
            return true;
        }

        #region Select Item
        public bool SelectItem(Item item)
        {
            // Kiểm tra có vị trí hợp lệ ở order không
            if (CheckEnergy() == false) return false;
            if (BlockClick) return false;

            // var listEmptyWaitingGrill = ListWaitingGrills.Where(e => e.IsActive && e.GetSlot(0).GetItem() == null).ToList();
            //     if (listEmptyWaitingGrill.Count == 1)
            //     {
            //         listEmptyWaitingGrill[0].Visual.PlayWarning();
            //         // chặn click
            //         var gameLogicHandler = GameController.Instance.GameLogicHandler;
            //         gameLogicHandler.BlockClick = true;
            //         SonatUtils.DelayCall(1f, () =>
            //         {
            //             gameLogicHandler.BlockClick = false;
            //         }, this);
            //     }

            ItemSelected = item;
            var (order, slot) = orderManager.GetDestinationSlot(item);
            if (slot != null)
            {
                // var log = new SpendResourceLogData()
                // {
                //     earnType = "energy",
                //     earnId = "energy",
                //     source = "gameplay"
                // };
                // MySonatFramework.GetService<InventoryService>().ReduceResource(GameResource.Pumpkin, 1, log);
                // EventBus<ReduceItemEvent>.Raise(new ReduceItemEvent() { resource = GameResource.Energy, quantity = 1 });
                SwitchSlot(slot);
                return true;
            }

            // Kiểm tra còn vị trí ở waiting grill không
            // Khi bay tới đĩa phải kiểm tra xem có order mới không thì nhảy lên ngay
            var (waitingGrill, waitingGrillSlot) = waitingGrillManager.GetDestinationSlot();
            if (waitingGrillSlot != null)
            {
                // var log = new SpendResourceLogData()
                // {
                //     earnType = "energy",
                //     earnId = "energy",
                //     source = "gameplay"
                // };
                // MySonatFramework.GetService<InventoryService>().ReduceResource(GameResource.Energy, 1, log);
                // EventBus<ReduceItemEvent>.Raise(new ReduceItemEvent() { resource = GameResource.Energy, quantity = 1 });
                SwitchSlot(waitingGrillSlot);
                return true;
            }
            return false;
        }

        private void SwitchSlot(SlotBase slot)
        {
            if (ItemSelected == null) return;

            ItemSelected.SetLockState(true);
            ItemSelected.SwitchSlot(slot);
            OnItemStartSwitchSucess?.Invoke(ItemSelected, true);
            OnItemStartSwitch?.Invoke(ItemSelected, slot);
        }


        private bool hasCollectItem = false;
        public bool HasCollectItem => hasCollectItem;
        public void ItemMoveSlot(Item item, SlotBase slot)
        {
            hasCollectItem = false;
            OnItemEndSwitch?.Invoke(item, slot);
        }
        #endregion

        #region MoveNextOrder
        public void StartMoveNextOrder(OrderEntity orderEntity)
        {
            orderEntity.Moving = true;
        }

        public void EndMoveNextOrder(OrderEntity orderEntity, bool startLevel = false)
        {
            orderEntity.Moving = false;
            orderEntity.Ready = true;

            if (startLevel == false)
            {
                OnAppearNextOrder?.Invoke(orderEntity);
                OnAppearNextOrderItem?.Invoke((int)orderEntity.ItemIdTarget);
            }

            int count = 0;
            var listOrders = new List<OrderEntity>(orderManager.ListOrders);
            foreach (var order in listOrders)
            {
                if (order.Ready == false || order.IsActive == false) continue;
                var targetItem = order.ItemIdTarget;
                foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
                {
                    var slot = waitingGrill.GetSlot(0);
                    var item = slot.GetItem();
                    var orderSlot = order.GetAvailableSlot();
                    if (item != null && item.id == (int)targetItem && orderSlot != null)
                    {
                        ItemSelected = item;
                        SwitchSlot(orderSlot);
                        count++;
                    }
                }
            }

            TryCheckLoseGame().Forget();
        }
        #endregion

        #region CollectItem
        public void StartCollectItem(OrderEntity orderEntity)
        {
            OnStartCollectItem?.Invoke(orderEntity);

            hasCollectItem = true;
            foreach (var slot in orderEntity.GetSlots())
            {
                slot.GetItem()?.OnComplete();
            }
        }

        public void EndCollectItem(OrderEntity orderEntity)
        {
            OnEndCollectItem?.Invoke(orderEntity);
            OnCollectItem?.Invoke((int)orderEntity.ItemIdTarget);

            if (CheckWinGame())
            {
                GameController.Instance.Win();
            }
        }
        #endregion

        #region Check Win Lose Game
        public bool CheckWinGame()
        {
            // Win khi clear hết level hết order
            if (GameController.Instance.GameState != GameState.Playing) return false;
            if (grillManager.CheckClearAllItems() && waitingGrillManager.CheckClearAllItems()) return true;
            return false;
        }

        public async UniTask TryCheckLoseGame()
        {
            Debug.Log("TryCheckLoseGame");
            var stuckType = CheckLoseGame();
            if (stuckType != null)
            {
                GameController.Instance.Stuck(stuckType.Value);
            }
        }

        public StuckType? CheckLoseGame()
        {
            if (GameController.Instance.GameState != GameState.Playing) return null;

            // waiting grill còn slot trống thì chưa thua
            var listWaitingGrill = waitingGrillManager.ListWaitingGrills;
            foreach (var waitingGrill in listWaitingGrill)
            {
                if (waitingGrill.GetSlots().Where(e => e.isEmpty() && waitingGrill.IsActive).Count() > 0)
                {
                    Debug.Log("waitingGrill.GetSlots().Where(e => e.isEmpty() && waitingGrill.IsActive).Count() > 0");
                    return null;
                }
            }

            // // nếu order còn có thể di chuyển item vào thì chưa thua + loại các item bị lock
            // var listTargetItemIds = orderManager.GetTargetItemIds();

            // var listItemIdInLayer1 = GrillHelper.GetItemIdListWithLayer(1, true);
            // var dictItems = listItemIdInLayer1.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());

            // var listLockedItems = ItemHelper.GetItemIdsInLockedGrill();
            // foreach (var item in listLockedItems)
            // {
            //     if (dictItems.ContainsKey(item) == false) continue;
            //     dictItems[item]--;
            // }

            // foreach (var id in listTargetItemIds)
            // {
            //     if (dictItems.ContainsKey(id) == true && dictItems[id] > 0)
            //     {
            //         Debug.Log("dictItems.ContainsKey(id) == true && dictItems[id] > 0");
            //         return null;
            //     }
            // }

            // else continue
            return StuckType.OutOfMove;
        }
        #endregion
    }
}
