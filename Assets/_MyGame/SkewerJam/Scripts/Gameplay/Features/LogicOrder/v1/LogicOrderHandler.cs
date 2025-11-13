using System.Linq;
using MyGame.SkewerJam.Gameplay.LogicOrder;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public class LogicOrderHandler_v1 : BaseLogicOrderHandler
    {
        [Header("Configs")]
        [SerializeField] private LogicOrderConfigSO logicOrderConfigSO;
        [SerializeField] private SpecialOrderConfigSO specialOrderConfigSO;

        private SequenceConfigSO selectedSequenceConfig;

        protected override void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            base.OnLoadLevelData(levelData);
            selectedSequenceConfig = logicOrderConfigSO.GetSequenceConfigSO(levelData.sequenceLogicOrderIndex, levelData.difficulty);
        }

        protected override BaseOrderSO ChooseLogicOrder()
        {
            if (CheckChooseRandomOrder() == true)
            {
                return listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Random);
            }

            var phase = GetCurrentPhase();
            // Nếu ở phase cuối mà 
            if (CheckRescueOrderInFinalPhase(phase) == true)
            {
                return listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Rescue);
            }

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

        private bool CheckRescueOrderInFinalPhase(int phase)
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            return phase == selectedSequenceConfig.listPhaseConfigs.Count - 1 && waitingGrillManager.GetWaitingGrillIds().Count >= 3;
        }

        private bool CheckExistBasicOrder()
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            foreach (var order in orderManager.ListOrders)
            {
                if (order.LogicOrderType == LogicOrderType.Basic || order.LogicOrderType == LogicOrderType.Random) return true;
            }
            return false;
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

        private bool CheckChooseRandomOrder()
        {
            // khi mà có nhỏ hơn 5 order ở layer 1
            var itemManager = GameController.Instance.GameLogicHandler.ItemManager;
            var currentItems = itemManager.CurrentItems;
            return currentItems < 10f;
        }
    }
}