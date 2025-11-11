using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public class DynamicLogicOrder : MonoBehaviour
    {
        [SerializeField] private DynamicLogicOrderConfigSO dynamicLogicOrderConfigSO;

        public const string KEY = "DynamicLogicOrder";
        private int countLose { get => PlayerPrefs.GetInt(KEY + "_countLose", 0); set => PlayerPrefs.SetInt(KEY + "_countLose", value); }

        private EventBinding<LevelStartedEvent> levelStartedEvent;
        private EventBinding<LevelEndedEvent> levelEndedEvent;
        private EventBinding<LevelQuitEvent> levelQuitEvent;

        public void OnEnable()
        {
            levelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);
            levelEndedEvent = new EventBinding<LevelEndedEvent>(OnLevelEnded);
            levelQuitEvent = new EventBinding<LevelQuitEvent>(OnLevelQuit);
        }

        public void OnDisable()
        {
            EventBus<LevelStartedEvent>.Deregister(levelStartedEvent);
            EventBus<LevelEndedEvent>.Deregister(levelEndedEvent);
            EventBus<LevelQuitEvent>.Deregister(levelQuitEvent);
        }


        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            var startCount = MySonatFramework.gameplayAnalyticsService.levelStartCount;
            if (startCount == 1) ResetCounLose();
        }

        private void OnLevelEnded(LevelEndedEvent eventData)
        {
            if (eventData.success == false) TryAddCountLose();
        }

        private void OnLevelQuit(LevelQuitEvent eventData)
        {
            TryAddCountLose();
        }

        public void TryAddCountLose()
        {
            if (CheckAddCountLose()) countLose += 1;
        }

        public void ResetCounLose()
        {
            countLose = 0;
        }

        private bool CheckAddCountLose()
        {
            var itemManager = GameController.Instance.GameLogicHandler.ItemManager;
            var currentItem = itemManager.CurrentItems;
            var totalItem = itemManager.TotalItems;
            return (totalItem - currentItem) > totalItem * dynamicLogicOrderConfigSO.completionRate;
        }

        public (int, int) GetDynamicIndex(int idxBO, int idxSO)
        {
            var difficulty = GameController.Instance.LevelGenerator.LevelData.difficulty;
            var configByDifficulty = dynamicLogicOrderConfigSO.GetConfigByDifficulty(difficulty);

            if (configByDifficulty == null) return (idxBO, idxSO);

            if (idxBO <= configByDifficulty.thresholdBO) return (idxBO, idxSO);

            var reduce = Mathf.FloorToInt(countLose / configByDifficulty.gapCountLose);
            var newIdxBO = idxBO - reduce;
            newIdxBO = Mathf.Max(newIdxBO, configByDifficulty.minIndexBO);
            return (newIdxBO, idxSO);
        }
    }
}