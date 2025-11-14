using System.Linq;
using MyGame.Modules.ProfileInGame;
using MyGame.SkewerJam.Gameplay.LogicOrder;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public class LogicOrderHandler_v2 : BaseLogicOrderHandler
    {
        public const string KEY_CURVE_INDEX = "CurveIndex";

        [Header("Configs")]
        [SerializeField] private LogicOrderConfigSO_v2 logicOrderConfigSO_v2;

        private FlowConfigSO_v2 flowConfigSO_v2;

        protected override void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            // thay đổi diff theo profile
            base.OnLoadLevelData(levelData);

            var randomFlowConfigSO = logicOrderConfigSO_v2.GetFlowConfigSO(_logicOrderData.difficulty, _logicOrderData.difficultyValue);
            flowConfigSO_v2 = randomFlowConfigSO;
        }

        protected override GameplayInfoForLogicOrder GetGameplayInfoForLogicOrder()
        {
            var phase = GetCurrentPhase();

            if (phase >= 8) _maxDepth = 3;

            var gameplayInfoForLogicOrder = new GameplayInfoForLogicOrder();
            gameplayInfoForLogicOrder.UpdateState(_maxDepth);

            var curveIndex = flowConfigSO_v2.GetCurveIndex(phase);
            Debug.Log("LogicOrderHandler_v2: GetGameplayInfoForLogicOrder: phase: " + phase + " >>> curveIndex: " + curveIndex);
            gameplayInfoForLogicOrder.SetTempData(KEY_CURVE_INDEX, curveIndex);
            return gameplayInfoForLogicOrder;
        }

        protected override BaseOrderSO ChooseLogicOrder()
        {
            return listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic_v2) as BasicOrderSO_v2;
        }

        private int GetCurrentPhase()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var itemManager = gameLogicHandler.ItemManager;
            var currentItems = itemManager.CurrentItems;
            var totalItems = itemManager.TotalItems;

            var percentage = (float)(totalItems - currentItems) / totalItems;
            return Mathf.FloorToInt(percentage * 10f);
        }
    }
}