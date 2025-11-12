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
    public class LogicOrderHandler_v2 : BaseLogicOrderHandler
    {
        // [SerializeField] private DynamicLogicOrder dynamicLogicOrder;

        [Header("Configs")]
        [SerializeField] private LogicOrderConfigSO_v2 logicOrderConfigSO_v2;

        private FlowConfigSO_v2 flowConfigSO_v2;

        public override void Init()
        {
            base.Init();
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData += OnLoadLevelData;
        }

        public override void Clear()
        {
            base.Clear();
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;
        }

        private void OnLoadLevelData(LevelData_SkewerJam levelData)
        {

            var randomDifficultyValue = levelData.level % 4;
            var randomDifficulty = levelData.level % 3;
            Debug.Log("<color=purple>LogicOrderHandler_v2:</color> OnLoadLevelData: " + (LevelDifficulty)randomDifficulty + " " + randomDifficultyValue);
            var randomFlowConfigSO = logicOrderConfigSO_v2.GetFlowConfigSO((LevelDifficulty)randomDifficulty, randomDifficultyValue);
            flowConfigSO_v2 = randomFlowConfigSO;
        }

        public override async UniTask<(ItemId itemId, int num, LogicOrderType logicOrderType)> GetItemOrder(bool isRescue = false)
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
                    SetForceRescue(false, -1);
                    Debug.Log("<color=green>LogicOrderHandler_v2:</color> Use " + forceLogicOrder.name + " to rescue: " + rescueItemId + " " + rescueNum);
                    return (rescueItemId, rescueNum, LogicOrderType.Rescue);
                }
                else
                {
                    SetForceRescue(false, -1);
                    return ForceGetItemOrder(gameplayInfoForLogicOrder);
                }
            }

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            SetForceRescue(false, -1);

            var phase = GetCurrentPhase();
            var curveIndex = flowConfigSO_v2.GetCurveIndex(phase);
            gameplayInfoForLogicOrder.phase = curveIndex;

            var selectedFlowConfigSO = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic_v2) as BasicOrderSO_v2;
            var (item, num) = selectedFlowConfigSO.GetOrder(gameplayInfoForLogicOrder);

            if (item != ItemId.None)
            {
                Debug.Log("<color=green>LogicOrderHandler_v2:</color> Use " + selectedFlowConfigSO.name + " to create order: " + item + " " + num);
                return (item, num, LogicOrderType.Basic_v2);
            }
            else
            {
                return ForceGetItemOrder(gameplayInfoForLogicOrder);
            }
            // lấy order info mỗi layer (2 layer đầu) --> OPTIMIZE: giảm tính toán
            // var selectedLogicOrder = ChooseLogicOrder();
            // Debug.Log("<color=green>OrderHelper:</color> Use " + selectedLogicOrder.name);

            // var (itemId, num) = selectedLogicOrder.GetOrder(gameplayInfoForLogicOrder);
            // if (itemId != ItemId.None)
            // {
            //     return (itemId, num, selectedLogicOrder.LogicOrderType);
            // }
            // else
            // {

            //     return ForceGetItemOrder(gameplayInfoForLogicOrder);
            //     // }
        }

        private int GetCurrentPhase()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var itemManager = gameLogicHandler.ItemManager;
            var currentItems = itemManager.CurrentItems;
            var totalItems = itemManager.TotalItems;

            var percentage = (float)(totalItems - currentItems) / totalItems;
            return Mathf.FloorToInt(percentage * 100f);
        }

        private (ItemId itemId, int num, LogicOrderType logicOrderType) ForceGetItemOrder(GameplayInfoForLogicOrder gameplayInfoForLogicOrder)
        {
            Debug.Log("<color=red>OrderHelper:</color> ForceGetItemOrder");
            var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
            var (i, n, s) = (basicOrder as BasicOrderSO).ForceGetItemOrderBasic(gameplayInfoForLogicOrder);
            if (i != ItemId.None)
            {
                Debug.Log("<color=red>LogicOrderHandler_v2:</color> ForceGetItemOrder: " + i + " " + n);
                return (i, n, LogicOrderType.Basic);
            }
            else
            {
                var randomOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Random);
                var (randomItemId, randomNum) = randomOrder.GetOrder(gameplayInfoForLogicOrder);
                Debug.Log("<color=red>LogicOrderHandler_v2:</color> ForceGetItemOrder: " + randomItemId + " " + randomNum);
                return (randomItemId, randomNum, LogicOrderType.Random);
            }
        }
    }
}