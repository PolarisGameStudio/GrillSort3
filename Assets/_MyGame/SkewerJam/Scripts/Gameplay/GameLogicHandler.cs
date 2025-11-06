using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay.Command;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Gameplay.Objects;
using MyGame.SkewerJam.Gameplay.Utils.CommandPattern;
using MyGame.SkewerJam.Objects;
using MyGame.SkewerJam.Objects.Entities;
using Sonat.Enums;
using SonatFramework.Systems.SettingsManagement.Vibation;
using UnityEngine;
using static MyGame.SkewerJam.Objects.Entities.OrderEntity;

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
        [SerializeField] private CommandInvoker commandInvoker;

        public OrderManager OrderManager => orderManager;
        public WaitingGrillManager WaitingGrillManager => waitingGrillManager;
        public GrillManager GrillManager => grillManager;
        public ConveyorManager ConveyorManager => conveyorManager;
        public ObstacleManager ObstacleManager => obstacleManager;
        public ItemManager ItemManager => itemManager;

        public BoosterManager BoosterManager => boosterManager;
        public CommandInvoker CommandInvoker => commandInvoker;

        #region Event Actions
        public event Action<Item, SlotBase> OnItemStartSwitch;
        public event Action<Item, bool, bool> OnItemStartSwitchAndCheck;
        public event Action<Item, SlotBase> OnItemStartSwitchUndo;
        public event Action<Item, SlotBase> OnItemMoveToSlot;
        public event Action<Item, SlotBase> OnItemEndSwitch;


        public event Action<OrderEntity> OnAppearNextOrder;
        public event Action<int> OnAppearNextOrderItem;
        public event Action<int> OnCollectItem;
        public event Action<OrderEntity> OnStartCollectItem;
        public event Action<OrderEntity> OnEndCollectItem;
        #endregion

        private bool _blockUIWhenEnd = false;
        public bool BlockUIWhenEnd => _blockUIWhenEnd;

        public void Init()
        {
            grillManager.Init();
            waitingGrillManager.Init();
            orderManager.Init();
            itemManager.Init();

            conveyorManager.Init();
            obstacleManager.Init();

            boosterManager.Init();
            suggestManager.Init();

            commandInvoker.Init();

            ResetCoroutine();
            _blockUIWhenEnd = false;
        }

        public void Clear()
        {
            orderManager.Clear();
            waitingGrillManager.Clear();
            grillManager.Clear();
            itemManager.Clear();

            conveyorManager.Clear();
            obstacleManager.Clear();

            boosterManager.Clear();
            suggestManager.Clear();

            commandInvoker.Clear();

            ResetCoroutine();
        }

        #region Select Item
        public bool SelectItem(Item item)
        {
            if (item == null)
            {
                return false;
            }

            if (item.OnCustomSelect() == true)
            {
                return true;
            }

            // ItemOnOrder thì không cần chặn
            // Nếu đang warning và số lần chặn click vẫn còn thì chặn click
            var checkItemOnOrder = ItemHelper.CheckSelectedItemOnOrder(item);
            if (checkItemOnOrder == false && CheckClickAndWarning(item) == true)
            {
                MySonatFramework.GetService<VibrationService>().Vibrate(100);
                return false;
            }

            // item bay
            bool isSwitchSuccess = false;

            var (order, slot) = orderManager.GetDestinationSlot(item);
            if (slot != null)
            {
                WaitingGrillHelper.ResetWarning();

                CreateCommand(item, slot);
                isSwitchSuccess = true;
            }
            else
            {
                var (waitingGrill, waitingGrillSlot) = waitingGrillManager.GetDestinationSlot();
                if (waitingGrillSlot != null)
                {
                    CreateCommand(item, waitingGrillSlot);
                    isSwitchSuccess = true;
                }
            }

            // sau khi item switch: Xem có cần warning không?
            if (isSwitchSuccess == true)
            {
                if (checkItemOnOrder == false && WaitingGrillHelper.CheckWarning() == true)
                {
                    WaitingGrillHelper.Warning();
                }
            }

            else
            {
                Debug.Log("<color=red>GameLogicHandler:</color> SelectItem: isSwitchSuccess == false, item: " + item.id);
            }
            return isSwitchSuccess;
        }

        private void CreateCommand(Item item, SlotBase slot)
        {
            commandInvoker.ExecuteCommand(new SelectItemCommand(item, slot));
        }

        private bool CheckClickAndWarning(Item item)
        {
            if (WaitingGrillHelper.IsWarning == true && WaitingGrillHelper.CheckWarningCount() == true)
            {
                if (WaitingGrillHelper.Warning() == true)
                {
                    return true;
                }
            }
            WaitingGrillHelper.ResetWarning();
            return false;
        }

        public void SwitchSlot(Item item, SlotBase slot)
        {
            bool fromWaitingGrill = item.Slot.GetGrill() is WaitingGrill;
            bool toOrder = slot.GetGrill() is OrderEntity;

            item.Moving = true;
            item.SetLockState(true);
            item.SwitchSlot(slot);
            OnItemStartSwitchAndCheck?.Invoke(item, fromWaitingGrill, toOrder);
            OnItemStartSwitch?.Invoke(item, slot);

            if (grillManager.CheckClearAllItems())
            {
                _blockUIWhenEnd = true;
                TryCheckWinGame().Forget();
            }

            // if (toOrder == true)
            // {
            // để cho hidden hiện luôn
            item.MoveToOrder();
            // }
        }

        public void ItemMoveToSlot(Item item, SlotBase slot)
        {
            OnItemMoveToSlot?.Invoke(item, slot);
        }

        public void EndSwitchSlot(Item item, SlotBase slot)
        {
            item.Moving = false;
            ItemMoveToSlot(item, slot);
            OnItemEndSwitch?.Invoke(item, slot);
        }
        #endregion

        #region MoveNextOrder
        public void StartMoveNextOrder(OrderEntity orderEntity)
        {
            orderEntity.State = OrderEntityState.Waiting;
        }

        public async UniTask EndMoveNextOrder(OrderEntity orderEntity, bool startLevel = false)
        {
            orderEntity.State = OrderEntityState.Ready;
            if (startLevel == false)
            {
                OnAppearNextOrder?.Invoke(orderEntity);
                OnAppearNextOrderItem?.Invoke((int)orderEntity.ItemIdTarget);
            }

            int count = 0;
            if (orderEntity.IsActive == true)
            {
                var targetItem = orderEntity.ItemIdTarget;
                await UniTask.WaitUntil(() => GameController.Instance.GameState == GameState.Playing);
                foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
                {
                    var slot = waitingGrill.GetSlot(0);
                    var item = slot.GetItem();
                    var orderSlot = orderEntity.GetAvailableSlot();
                    if (item != null && item.id == (int)targetItem && orderSlot != null && item.Moving == false)
                    {
                        // chờ tới khi item rơi hẳn xuống đĩa thì mới lấy
                        SwitchSlot(item, orderSlot);
                        count++;
                    }
                }
            }
            WaitingGrillHelper.ResetWarning();
        }

        public async UniTask TryCheckMatchItem(Item item)
        {
            var (_, slot) = orderManager.GetDestinationSlot(item);
            if (slot != null)
            {
                await UniTask.WaitUntil(() => GameController.Instance.GameState == GameState.Playing);
                SwitchSlot(item, slot);
                return;
            }
        }
        #endregion

        #region CollectItem
        public void StartCollectItem(OrderEntity orderEntity)
        {
            OnStartCollectItem?.Invoke(orderEntity);

            foreach (var slot in orderEntity.GetSlots())
            {
                slot.GetItem()?.OnComplete();
            }
            WaitingGrillHelper.ResetWarning();
        }

        public void EndCollectItem(OrderEntity orderEntity)
        {
            OnEndCollectItem?.Invoke(orderEntity);
            OnCollectItem?.Invoke((int)orderEntity.ItemIdTarget);

            GameController.Instance.TryWin();
        }
        #endregion

        #region Check Win Lose Game

        public async UniTask TryCheckWinGame()
        {
            if (CheckWinGame())
            {
                GameController.Instance.SetWin();
            }
        }

        public bool CheckWinGame()
        {
            // Win khi clear hết level hết order
            if (GameController.Instance.GameState != GameState.Playing && GameController.Instance.GameState != GameState.UsingBooster)
                return false;

            if (grillManager.CheckClearAllItems() && waitingGrillManager.CheckClearAllItems()) return true;
            return false;
        }


        private Coroutine _coroutineTryCheckLoseGame;

        public void TryCheckLoseGame()
        {
            ResetCoroutine();
            _coroutineTryCheckLoseGame = StartCoroutine(IETryCheckLoseGame());
        }

        private void ResetCoroutine()
        {
            if (_coroutineTryCheckLoseGame != null)
            {
                StopCoroutine(_coroutineTryCheckLoseGame);
            }
        }

        private IEnumerator IETryCheckLoseGame()
        {
            // check lose khi:
            // - order tĩnh: không có order nào đang di chuyển vào
            // - waiting grill tĩnh: không còn item nào nhảy lên đĩa
            yield return new WaitUntil(() =>
            {
                return orderManager.ListOrders.Where(e => e.State == OrderEntityState.Waiting && e.IsActive).Count() == 0;
            });
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() =>
            {
                return GameController.Instance.GameState == GameState.Playing;
            });
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
                    // Debug.Log("waitingGrill.GetSlots().Where(e => e.isEmpty() && waitingGrill.IsActive).Count() > 0");
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

        public void UndoSwitchSlot(Item item, SlotBase sourceSlot)
        {
            OnItemStartSwitchUndo?.Invoke(item, sourceSlot);
        }
        #endregion
    }
}
