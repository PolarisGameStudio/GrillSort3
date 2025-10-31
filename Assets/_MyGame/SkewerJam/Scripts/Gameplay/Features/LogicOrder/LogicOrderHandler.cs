using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.SkewerJam.Gameplay.LogicOrder;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using MyGame.SkewerJam.Level;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public class LogicOrderHandler : MonoBehaviour
    {
        [SerializeField] private List<BaseOrderSO> listLogicOrders;
        [SerializeField] private DynamicLogicOrder dynamicLogicOrder;

        [Header("Configs")]
        [SerializeField] private LogicOrderConfigSO logicOrderConfigSO;
        [SerializeField] private SpecialOrderConfigSO specialOrderConfigSO;


        public bool IsForceRescue { get; private set; }
        public ForceRescueData ForceRescueData => _forceRescueData;
        private ForceRescueData _forceRescueData = new ForceRescueData();
        private SequenceConfigSO selectedSequenceConfig;

        public void Init()
        {
            foreach (var logicOrder in listLogicOrders)
            {
                logicOrder.Init();
            }

            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData += OnLoadLevelData;

            SetForceRescue(false, -1);

            // dynamicLogicOrder.Init();
        }

        public void Clear()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;

            SetForceRescue(false, -1);
            // dynamicLogicOrder.Clear();
        }

        private void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            // var sequenceIndex = ValidateSequenceIndex(levelData.sequenceLogicOrderIndex, levelData.difficulty);
            Debug.Log("<color=purple>LogicOrderHandler:</color> OnLoadLevelData: " + levelData.sequenceLogicOrderIndex + " - " + levelData.difficulty);
            selectedSequenceConfig = logicOrderConfigSO.GetSequenceConfigSO(levelData.sequenceLogicOrderIndex, levelData.difficulty);
            foreach (var logicOrder in listLogicOrders)
            {
                logicOrder.SetLevelData(levelData);
            }
        }

        public bool CheckCreateNextOrder()
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;

            var dictAllItemIds = ItemHelper.GetItemIdDictInGameplay(-1);
            var orderItemsDict = orderManager.GetOrderItemsDict();

            // số item còn lại <= số item còn lại tạo order
            foreach (var id in dictAllItemIds.Keys)
            {
                if (orderItemsDict.ContainsKey(id) == true)
                {
                    // Nếu item target + item còn lại cùng loại nó > maxItems thì cần tạo thêm order
                    if (dictAllItemIds[id] + orderItemsDict[id].num > orderItemsDict[id].maxItems) return true;
                }
                else
                {
                    // Nếu item còn lại không có trong target thì cần tạo order
                    return true;
                }
            }
            return false;
        }

        public async UniTask<(ItemId itemId, int num, LogicOrderType logicOrderType)> GetItemOrder(bool isRescue = false)
        {
            // ưu tiện chọn theo logic được chọn
            // otherwise chọn theo basic order
            // otherwise chọn theo force basic order
            // otherwise chọn theo random order
            var gameplayInfoForLogicOrder = new GameplayInfoForLogicOrder();
            gameplayInfoForLogicOrder.UpdateState();


            Debug.Log("<color=purple>LogicOrderHandler:</color> -----GetItemOrder----");
            var forceLogicOrder = listLogicOrders.FirstOrDefault(e => e.ForceUse(isRescue));
            if (forceLogicOrder != null)
            {
                Debug.Log("<color=blue>OrderHelper:</color> Use " + forceLogicOrder.name + " to rescue");
                var (rescueItemId, rescueNum) = forceLogicOrder.GetOrder(gameplayInfoForLogicOrder);
                if (rescueItemId != ItemId.None)
                {
                    SetForceRescue(false, -1);
                    return (rescueItemId, rescueNum, LogicOrderType.Rescue);
                }
                else
                {
                    Debug.Log("<color=red>OrderHelper:</color> GetItemOrder: No rescue item found, use basic order");
                    var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
                    var (i, n, s) = (basicOrder as BasicOrderSO).ForceGetItemOrderBasic(gameplayInfoForLogicOrder);
                    SetForceRescue(false, -1);
                    return (i, n, LogicOrderType.Basic);
                }
            }

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            SetForceRescue(false, -1);

            // lấy order info mỗi layer (2 layer đầu) --> OPTIMIZE: giảm tính toán
            var selectedLogicOrder = ChooseLogicOrder();
            Debug.Log("<color=green>OrderHelper:</color> Use " + selectedLogicOrder.name);

            var (itemId, num) = selectedLogicOrder.GetOrder(gameplayInfoForLogicOrder);
            if (itemId != ItemId.None)
            {
                return (itemId, num, selectedLogicOrder.LogicOrderType);
            }
            else
            {
                Debug.Log("<color=red>OrderHelper:</color> GetItemOrder: No item found");
                var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
                var (i, n, s) = (basicOrder as BasicOrderSO).ForceGetItemOrderBasic(gameplayInfoForLogicOrder);
                if (i != ItemId.None)
                {
                    return (i, n, LogicOrderType.Basic);
                }
                else
                {
                    var randomOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Random);
                    var (randomItemId, randomNum) = randomOrder.GetOrder(gameplayInfoForLogicOrder);
                    return (randomItemId, randomNum, LogicOrderType.Random);
                }
            }
        }

        private BaseOrderSO ChooseLogicOrder()
        {
            if (CheckChooseRandomOrder() == true)
            {
                return listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Random);
            }

            var phase = GetCurrentPhase();

            var phaseConfig = selectedSequenceConfig.listPhaseConfigs[phase];
            var (idxBO, idxSO) = dynamicLogicOrder.GetDynamicIndex(phaseConfig.indexBO, phaseConfig.indexSO);

            Debug.Log("<color=white>LogicOrderHandler:</color> Dynamic Index: " + idxBO + " - " + idxSO + "<<< " + phaseConfig.indexBO + " - " + phaseConfig.indexSO);

            var minNum = phaseConfig.minNum;
            var specialOrderConfig = specialOrderConfigSO.listSpecialOrderConfigs[idxSO];

            // - Cùng 1 thời điểm, luôn tồn tại 1 Basic Order
            if (CheckExistBasicOrder() == true)
            {
                var logicOrderType = specialOrderConfig.GetLogicOrderType();
                switch (logicOrderType)
                {
                    case LogicOrderType.Basic:
                        var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
                        var basicOrderSO = basicOrder as BasicOrderSO;
                        basicOrderSO.SetData(idxBO);
                        basicOrderSO.SetMinNum(minNum);
                        return basicOrder;
                    case LogicOrderType.Locked:
                        var lockedOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Locked);
                        if (lockedOrder.CanUse()) return lockedOrder;
                        break;
                    case LogicOrderType.Blinded:
                        var blindedOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Blinded);
                        if (blindedOrder.CanUse()) return blindedOrder;
                        break;
                    case LogicOrderType.Tricky1:
                        var tricky1Order = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Tricky1);
                        if (tricky1Order.CanUse()) return tricky1Order;
                        break;
                    case LogicOrderType.Tricky2:
                        var tricky2Order = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Tricky2);
                        if (tricky2Order.CanUse()) return tricky2Order;
                        break;
                }
            }

            var order = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
            var orderSO = order as BasicOrderSO;
            orderSO.SetData(idxBO);
            orderSO.SetMinNum(minNum);
            return order;

        }

        private bool CheckExistBasicOrder()
        {
            return listLogicOrders.Any(e => e.LogicOrderType == LogicOrderType.Basic);
        }

        private int GetCurrentPhase()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var itemManager = gameLogicHandler.ItemManager;
            var currentItems = itemManager.CurrentItems;
            var totalItems = itemManager.TotalItems;

            var percentage = (float)(totalItems - currentItems) / totalItems;
            var phase = selectedSequenceConfig.listPhaseConfigs.Where(e => e.threshold >= percentage).OrderBy(e => e.index).FirstOrDefault();
            Debug.Log("<color=purple>LogicOrderHandler:</color> GetCurrentPhase: " + phase.index + " - " + Mathf.Round(percentage * 100f) * 0.01f);
            return phase.index;
        }

        public void SetForceRescue(bool isForceRescue, int deltaSlot = -1)
        {
            IsForceRescue = isForceRescue;
            if (isForceRescue == true)
            {
                _forceRescueData.deltaSlot = deltaSlot;
            }
            else
            {
                _forceRescueData.deltaSlot = -1;
            }
        }

        private bool CheckChooseRandomOrder()
        {
            // khi mà có nhỏ hơn 5 order ở layer 1
            var itemManager = GameController.Instance.GameLogicHandler.ItemManager;
            var currentItems = itemManager.CurrentItems;
            return currentItems < 10f;
        }
    }

    public enum LogicOrderType
    {
        None,
        Basic,
        Locked,
        Blinded,
        Rescue,
        Random,
        Tricky1,
        Tricky2
    }

    public class ForceRescueData
    {
        public int deltaSlot = -1;
    }
}