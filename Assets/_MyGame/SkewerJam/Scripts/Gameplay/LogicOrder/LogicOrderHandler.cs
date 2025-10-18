using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Options;
using Manager;
using MyGame.SkewerJam.Gameplay.LogicOrder;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public class LogicOrderHandler : MonoBehaviour
    {
        [SerializeField] private List<BaseOrderSO> listLogicOrders;

        [Header("Configs")]
        [SerializeField] private LogicOrderConfigSO logicOrderConfigSO;
        [SerializeField] private SpecialOrderConfigSO specialOrderConfigSO;

        private SequenceConfigSO selectedSequenceConfig;

        public void Init()
        {
            foreach (var logicOrder in listLogicOrders)
            {
                logicOrder.Init();
            }

            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData += OnLoadLevelData;
        }

        public void Clear()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;
        }

        private void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            var sequenceIndex = ValidateSequenceIndex(levelData.sequenceLogicOrderIndex);
            selectedSequenceConfig = logicOrderConfigSO.listSequenceConfigs[sequenceIndex];
            foreach (var logicOrder in listLogicOrders)
            {
                logicOrder.SetLevelData(levelData);
            }
        }

        private int ValidateSequenceIndex(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            if (index >= logicOrderConfigSO.listSequenceConfigs.Count)
            {
                return logicOrderConfigSO.listSequenceConfigs.Count - 1;
            }
            return index;
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

        public async UniTask<(ItemId itemId, int num)> GetItemOrder(bool isRescue = false)
        {
            Debug.Log("<color=purple>LogicOrderHandler:</color> -----GetItemOrder----");
            var forceLogicOrder = listLogicOrders.FirstOrDefault(e => e.ForceUse(isRescue));
            if (forceLogicOrder != null)
            {
                Debug.Log("<color=blue>OrderHelper:</color> Use " + forceLogicOrder.name + " to rescue");
                var (rescueItemId, rescueNum) = forceLogicOrder.GetOrder();
                return (rescueItemId, rescueNum);
            }

            // lấy order info mỗi layer (2 layer đầu) --> OPTIMIZE: giảm tính toán
            var gameplayInfoForLogicOrder = new GameplayInfoForLogicOrder();
            gameplayInfoForLogicOrder.UpdateState();
            var selectedLogicOrder = ChooseLogicOrder();
            Debug.Log("<color=green>OrderHelper:</color> Use " + selectedLogicOrder.name);

            var (itemId, num) = selectedLogicOrder.GetOrder(gameplayInfoForLogicOrder);
            if (itemId != ItemId.None)
            {
                return (itemId, num);
            }
            else
            {
                Debug.Log("<color=red>OrderHelper:</color> GetItemOrder: No item found");
                var basicOrder = listLogicOrders.FirstOrDefault(e => e.GetType().Name == nameof(BasicOrderSO));
                var (i, n, s) = (basicOrder as BasicOrderSO).ForceGetItemOrderBasic(gameplayInfoForLogicOrder);
                return (i, n);
            }
        }

        private BaseOrderSO ChooseLogicOrder()
        {
            // kiểm tra xem có sử dụng được nó không

            var phase = GetCurrentPhase();

            var phaseConfig = selectedSequenceConfig.listPhaseConfigs[phase];
            var idxBO = phaseConfig.indexBO;
            var idxSO = phaseConfig.indexSO;
            var specialOrderConfig = specialOrderConfigSO.listSpecialOrderConfigs[idxSO];

            var random = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("<color=white>LogicOrderHandler:</color> ChooseLogicOrder: " + Mathf.Round(random * 100f) * 0.01f + " >>> " + specialOrderConfig.rateBasicOrder + " - " + specialOrderConfig.rateLockedOrder + " - " + specialOrderConfig.rateBlindedOrder);
            if (random < specialOrderConfig.rateBasicOrder)
            {
                var basicOrder = listLogicOrders.FirstOrDefault(e => e.GetType().Name == nameof(BasicOrderSO));
                (basicOrder as BasicOrderSO).SetData(idxBO);
                return basicOrder;
            }
            else if (random < specialOrderConfig.rateBasicOrder + specialOrderConfig.rateLockedOrder)
            {
                return listLogicOrders.FirstOrDefault(e => e.GetType().Name == nameof(LockedOrderSO));
            }
            else
            {
                return listLogicOrders.FirstOrDefault(e => e.GetType().Name == nameof(BlindedOrderSO));
            }
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
    }
}