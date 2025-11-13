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
        public const string KEY_CURVE_INDEX = "CurveIndex";

        [Header("Configs")]
        [SerializeField] private LogicOrderConfigSO_v2 logicOrderConfigSO_v2;

        private FlowConfigSO_v2 flowConfigSO_v2;

        protected override void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            var randomDifficultyValue = levelData.level % 4;
            var randomDifficulty = levelData.level % 3;
            Debug.Log("<color=purple>LogicOrderHandler_v2:</color> OnLoadLevelData: " + (LevelDifficulty)randomDifficulty + " " + randomDifficultyValue);
            var randomFlowConfigSO = logicOrderConfigSO_v2.GetFlowConfigSO((LevelDifficulty)randomDifficulty, randomDifficultyValue);
            flowConfigSO_v2 = randomFlowConfigSO;
        }

        protected override GameplayInfoForLogicOrder GetGameplayInfoForLogicOrder()
        {
            var gameplayInfoForLogicOrder = base.GetGameplayInfoForLogicOrder();

            var phase = GetCurrentPhase();
            var curveIndex = flowConfigSO_v2.GetCurveIndex(phase);
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
            return Mathf.FloorToInt(percentage * 100f);
        }
    }
}