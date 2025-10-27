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

            // dynamicLogicOrder.Init();
        }

        public void Clear()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;
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

            var dictAllItemIds = ItemHelper.GetItemIdDictInGameplay(-1, true);
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
                    return (rescueItemId, rescueNum, LogicOrderType.Rescue);
                }
                else
                {
                    Debug.Log("<color=red>OrderHelper:</color> GetItemOrder: No rescue item found, use basic order");
                    var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
                    var (i, n, s) = (basicOrder as BasicOrderSO).ForceGetItemOrderBasic(gameplayInfoForLogicOrder);
                    return (i, n, LogicOrderType.Basic);
                }
            }

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            IsForceRescue = false;

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
                return (i, n, LogicOrderType.Basic);
            }
        }

        private BaseOrderSO ChooseLogicOrder()
        {
            var phase = GetCurrentPhase();

            var phaseConfig = selectedSequenceConfig.listPhaseConfigs[phase];
            var (idxBO, idxSO) = dynamicLogicOrder.GetDynamicIndex(phaseConfig.indexBO, phaseConfig.indexSO);

            Debug.Log("<color=white>LogicOrderHandler:</color> Dynamic Index: " + idxBO + " - " + idxSO + "<<< " + phaseConfig.indexBO + " - " + phaseConfig.indexSO);

            var minNum = phaseConfig.minNum;
            var specialOrderConfig = specialOrderConfigSO.listSpecialOrderConfigs[idxSO];

            // - Cùng 1 thời điểm, luôn tồn tại 1 Basic Order
            if (CheckExistBasicOrder() == true)
            {
                var random = UnityEngine.Random.Range(0f, 1f);
                Debug.Log("<color=white>LogicOrderHandler:</color> ChooseLogicOrder: " + Mathf.Round(random * 100f) * 0.01f + " >>> " + specialOrderConfig.rateBasicOrder + " - " + specialOrderConfig.rateLockedOrder + " - " + specialOrderConfig.rateBlindedOrder);
                if (random < specialOrderConfig.rateBasicOrder)
                {
                    var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
                    var basicOrderSO = basicOrder as BasicOrderSO;
                    basicOrderSO.SetData(idxBO);
                    basicOrderSO.SetMinNum(minNum);
                    return basicOrder;
                }
                else if (random < specialOrderConfig.rateBasicOrder + specialOrderConfig.rateLockedOrder)
                {
                    var lockedOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Locked);
                    if (lockedOrder.CanUse()) return lockedOrder;
                }
                else
                {
                    var blindedOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Blinded);
                    if (blindedOrder.CanUse()) return blindedOrder;
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
    }

    public enum LogicOrderType
    {
        None,
        Basic,
        Locked,
        Blinded,
        Rescue,
        Random
    }

    public class ForceRescueData
    {
        public int deltaSlot = -1;
    }
}