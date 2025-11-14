using System.Collections.Generic;
using MyGame.SkewerJam.Objects.Entities;
using UnityEngine;
using Gameplay.Entities;
using Cysharp.Threading.Tasks;
using Manager;
using DG.Tweening;
using System.Linq;
using MyGame.SkewerJam.Level;
using static MyGame.SkewerJam.Objects.Entities.OrderEntity;
using SonatFramework.Systems.AudioManagement;
using Sonat.Enums;
using MyGame.SkewerJam.Utils;
using MyGame.SkewerJam.Gameplay.LogicOrder;

namespace MyGame.SkewerJam.Gameplay.Objects
{
    public class OrderManager : MonoBehaviour
    {
        [SerializeField] private OrderEntityConfigSO orderEntityConfigSO;

        [Header("Align")]
        [SerializeField] private float distance = 2.6f;
        [SerializeField] private Transform rightStartPos;
        [SerializeField] private Transform leftStartPos;

        [Header("Logic Order")]
        [SerializeField] private BaseLogicOrderHandler logicOrderHandler;
        public BaseLogicOrderHandler LogicOrderHandler => logicOrderHandler;


        private List<OrderData_SkewerJam> _listOrderData = new List<OrderData_SkewerJam>();
        public List<OrderData_SkewerJam> ListOrderData => _listOrderData;


        private List<OrderEntity> _listOrders = new List<OrderEntity>();
        private List<OrderEntity> _listOrdersToAlign = new List<OrderEntity>();
        private List<Vector3> _listOrderLocalPositions = new List<Vector3>();


        public List<OrderEntity> ListOrders => _listOrders;
        public Transform LeftStartPos => leftStartPos;
        public Transform RightStartPos => rightStartPos;

        #region Init
        public async UniTask Init()
        {
            // game events
            var gameLogicHandler = GameController.Instance.GameLogicHandler;

            // logic:
            // - Khi start switch: Tính luôn order mới và xóa order cũ ở list, chuyển trạng thái các order này
            // - Khi end switch: order mới playcomplete và nextorder playappear
            gameLogicHandler.OnItemStartSwitch += GameLogicHandler_OnItemStartSwitch;
            gameLogicHandler.OnItemEndSwitch += GameLogicHandler_OnItemEndSwitch;

            logicOrderHandler.Init();
        }

        public void Clear()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnItemStartSwitch -= GameLogicHandler_OnItemStartSwitch;
            gameLogicHandler.OnItemEndSwitch -= GameLogicHandler_OnItemEndSwitch;

            foreach (var order in _listOrders)
            {
                if (order != null)
                    GameFactory.Instance.ReturnEntity(order);
            }
            _listOrders.Clear();
            _listOrdersToAlign.Clear();

            logicOrderHandler.Clear();
        }

        private void GameLogicHandler_OnItemStartSwitch(Item item, SlotBase slot)
        {
            // check ngay hoàn thành order
            // nếu complete thì tạo ngay next order
            if (slot.GetGrill() is OrderEntity orderEntity)
            {
                if (orderEntity.CheckComplete())
                {
                    orderEntity.State = OrderEntityState.Complete;

                    var orderIndex = orderEntity.OrderIndex;
                    _listOrders.Remove(orderEntity);

                    var checkNextOrder = logicOrderHandler.CheckCreateNextOrder();
                    if (checkNextOrder)
                    {
                        CreateNextOrder(orderIndex).Forget();
                    }

                    // xóa các command liên quan tới order này
                    var items = orderEntity.GetSlots().Select(s => s.GetItem()).Where(i => i != null).ToList();
                    var gameLogicHandler = GameController.Instance.GameLogicHandler;
                    gameLogicHandler.CommandInvoker.ResetStack();
                }
            }
        }


        private void GameLogicHandler_OnItemEndSwitch(Item item, SlotBase slot)
        {
            if (slot.GetGrill() is OrderEntity orderEntity)
            {
                orderEntity.CompleteCount++;

                // kiểm tra order này đã hoàn thành và các item bay đủ tới chưa
                if (orderEntity.State == OrderEntityState.Complete && orderEntity.CompleteCount >= orderEntity.MaxItems)
                {

                    var nextOrder = _listOrders.Where(e => e.OrderIndex == orderEntity.OrderIndex).FirstOrDefault();
                    var checkNextOrder = nextOrder != null;

                    orderEntity.PlayComplete(() =>
                        {
                            GameController.Instance.GameLogicHandler.EndCollectItem(orderEntity);
                            _listOrdersToAlign.Remove(orderEntity);
                            if (checkNextOrder == false)
                            {
                                AlignObjects(_listOrdersToAlign).Forget();
                            }
                        });

                    if (nextOrder != null)
                    {
                        PlayAppearNextOrder(nextOrder).Forget();
                        _listOrdersToAlign.Add(nextOrder);
                    }

                    GameController.Instance.GameLogicHandler.StartCollectItem(orderEntity);
                }

                // await GameController.Instance.GameLogicHandler.TryCheckMatchItem(item);
                GameController.Instance.GameLogicHandler.TryCheckLoseGame();
            }

        }
        #endregion

        #region DATA
        public async UniTask SetData(List<OrderData_SkewerJam> listOrderData)
        {
            this._listOrderData = listOrderData;
            // xác định ví trí các order
            var startPos = -(_listOrderData.Count - 1) * distance / 2;

            _listOrderLocalPositions.Clear();
            for (int i = 0; i < _listOrderData.Count; i++)
            {
                var orderPos = new Vector3(startPos + distance * i, 0, 0);
                _listOrderLocalPositions.Add(orderPos);
            }

            for (int i = 0; i < _listOrderData.Count; i++)
            {
                if (_listOrderData[i].active == 1)
                {
                    var (itemId, num, logicOrderType) = await logicOrderHandler.GetItemOrder();
                    var orderEntity = await CreateActiveNextOrder(itemId, num, logicOrderType);
                    orderEntity.SetOrderIndex(i);
                }
                else
                {
                    var orderEntity = await CreateNextLockedOrder();
                    orderEntity.SetOrderIndex(i);
                }
            }

            _listOrdersToAlign = new List<OrderEntity>(_listOrders);

            // tất cả order xuất hiện đầu game
            Canvas.ForceUpdateCanvases();
            for (int i = 0; i < _listOrders.Count; i++)
            {
                var order = _listOrders[i];
                order.transform.DOKill();

                var orderPos = rightStartPos.position;
                orderPos.z = 0;
                order.transform.position = orderPos;
            }
            // await PlayAppearOrders();
        }
        #endregion

        public async UniTask<OrderEntity> CreateActiveNextOrder(ItemId itemId, int num, LogicOrderType logicOrderType)
        {
            var orderEntity = await GameFactory.Instance.CreateEntityAsync<OrderEntity>("OrderEntity", transform);
            orderEntity.Init(true);
            orderEntity.SetData(itemId, num, logicOrderType);

            _listOrders.Add(orderEntity);
            GameController.Instance.GameLogicHandler.StartMoveNextOrder(orderEntity);

            return orderEntity;
        }

        public async UniTask<OrderEntity> CreateNextLockedOrder()
        {
            var orderEntity = await GameFactory.Instance.CreateEntityAsync<OrderEntity>("OrderEntity", transform);
            orderEntity.Init(false);

            _listOrders.Add(orderEntity);

            GameController.Instance.GameLogicHandler.StartMoveNextOrder(orderEntity);

            return orderEntity;
        }

        private async UniTask CreateNextOrder(int orderIndex)
        {
            var (itemId, num, logicOrderType) = await logicOrderHandler.GetItemOrder();

            var nextOrder = await CreateActiveNextOrder(itemId, num, logicOrderType);
            nextOrder.SetOrderIndex(orderIndex);

            var orderPos = leftStartPos.position;
            orderPos.z = 0;
            nextOrder.transform.position = orderPos;

            GameController.Instance.GameLogicHandler.StartMoveNextOrder(nextOrder);
        }

        private async UniTask PlayAppearNextOrder(OrderEntity nextOrder)
        {
            await UniTask.Delay((int)(orderEntityConfigSO.delayAppearNextOrder * 1000));

            MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Box_Appear_Grill3);
            nextOrder.Visual.OpenGrill(true, false);
            nextOrder.Visual.SetSortingGroup(true, LayerManager.Default, -1);
            await nextOrder.transform.DOLocalMove(_listOrderLocalPositions[nextOrder.OrderIndex], orderEntityConfigSO.durationMoveIn).SetEase(Ease.OutSine);
            nextOrder.Visual.SetSortingGroup(false);
            GameController.Instance.GameLogicHandler.EndMoveNextOrder(nextOrder);
        }

        public async UniTask PlayAppearOrders()
        {
            for (int i = 0; i < _listOrders.Count; i++)
            {
                var order = _listOrders[i];
                MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Box_Appear_Grill3);
                order.transform.DOLocalMove(_listOrderLocalPositions[order.OrderIndex], orderEntityConfigSO.durationMoveInStart).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    if (order.IsActive)
                    {
                        order.Visual.OpenGrill(true, true);
                    }
                    GameController.Instance.GameLogicHandler.EndMoveNextOrder(order, true);
                });
                await UniTask.Delay(200);
            }
        }

        #region Functions
        public (OrderEntity order, SlotBase slot) GetDestinationSlot(Item item)
        {
            var options = new List<(OrderEntity order, SlotBase slot)>();
            foreach (var order in _listOrders)
            {
                if (order.ItemIdTarget == (ItemId)item.id && order.State == OrderEntityState.Ready && order.IsActive == true)
                {
                    var orderSlot = order.GetAvailableSlot();
                    if (orderSlot != null)
                    {
                        options.Add((order, orderSlot));
                        // return (order, orderSlot);
                    }
                }
            }

            if (options.Count == 0)
            {
                return (null, null);
            }
            var selectedOption = options.OrderBy(e => e.order.MaxItems - e.order.CurrentItems).FirstOrDefault();
            return selectedOption;
        }

        public List<ItemId> GetTargetItemIds()
        {
            var list = new List<ItemId>();
            foreach (var order in _listOrders)
            {
                if (order.IsActive == false) continue;
                if (order.ItemIdTarget == ItemId.None) continue;
                list.Add(order.ItemIdTarget);
            }
            return list.Distinct().ToList();
        }

        public Dictionary<ItemId, (int maxItems, int num)> GetOrderItemsDict()
        {
            var dict = new Dictionary<ItemId, (int maxItems, int num)>();
            foreach (var order in _listOrders)
            {
                var targetItem = order.ItemIdTarget;
                if (order.IsActive == false || targetItem == ItemId.None) continue;

                if (dict.ContainsKey(targetItem) == false)
                {
                    dict[targetItem] = (0, 0);
                }

                var newMaxItems = dict[targetItem].maxItems + order.MaxItems;
                dict[targetItem] = (newMaxItems, dict[targetItem].num);

                foreach (var slot in order.GetSlots())
                {
                    if (slot.GetItem() != null)
                    {
                        dict[targetItem] = (newMaxItems, dict[targetItem].num + 1);
                    }
                }
            }
            return dict;
        }
        #endregion

        private bool _isAligning = false;

        public async UniTask AlignObjects(List<OrderEntity> listOrdersToAlign)
        {
            // chờ tới khi order hoàn thành dừng lại thì mới căn lại
            await UniTask.Delay((int)(orderEntityConfigSO.durationAlignOrders * 1000));
            await UniTask.WaitUntil(() => _isAligning == false);
            _isAligning = true;
            var start = -(listOrdersToAlign.Count - 1) * distance / 2;
            var sortedOrders = listOrdersToAlign.OrderBy(e => e.OrderIndex).ToList();
            var listLocalTargetPositions = new List<Vector3>();
            for (int i = 0; i < sortedOrders.Count; i++)
            {
                listLocalTargetPositions.Add(new Vector3(start + distance * i, 0, 0));

                var orderEntity = sortedOrders[i];
                orderEntity.transform.DOLocalMove(listLocalTargetPositions[i], 0.1f).SetEase(Ease.OutSine);
                await UniTask.Delay(75);
            }

            await UniTask.Delay(100);
            _isAligning = false;

        }

        public void Unlock(OrderEntity orderEntity = null, bool isRescue = false)
        {
            if (orderEntity == null)
            {
                orderEntity = _listOrders.Where(e => e.IsActive == false).FirstOrDefault();
                if (orderEntity == null) return;
            }
            logicOrderHandler.ResetData();
            logicOrderHandler.SetForceRescue(isRescue, -2);
            orderEntity.PlayUnlock(isRescue);
        }

        public void Undo(OrderEntity orderEntity, SlotBase targetSlot)
        {
            orderEntity.ShowTargetItemOnSlot(targetSlot);
            orderEntity.CompleteCount--;
        }
    }
}