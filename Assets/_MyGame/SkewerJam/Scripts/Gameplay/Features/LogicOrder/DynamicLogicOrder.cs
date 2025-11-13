using MyGame.Modules.ProfileInGame;
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
        private EventBinding<LevelReplayEvent> levelReplayEvent;

        public void OnEnable()
        {
            levelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);
            levelEndedEvent = new EventBinding<LevelEndedEvent>(OnLevelEnded);
            levelQuitEvent = new EventBinding<LevelQuitEvent>(OnLevelQuit);
            levelReplayEvent = new EventBinding<LevelReplayEvent>(OnLevelReplay);
        }

        public void OnDisable()
        {
            EventBus<LevelStartedEvent>.Deregister(levelStartedEvent);
            EventBus<LevelEndedEvent>.Deregister(levelEndedEvent);
            EventBus<LevelQuitEvent>.Deregister(levelQuitEvent);
            EventBus<LevelReplayEvent>.Deregister(levelReplayEvent);
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

        private void OnLevelReplay(LevelReplayEvent eventData)
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
            // var difficulty = GameController.Instance.LevelGenerator.LevelData.difficulty;
            // var configByDifficulty = dynamicLogicOrderConfigSO.GetConfigByDifficulty(difficulty);

            // if (configByDifficulty == null) return (idxBO, idxSO);

            // if (idxBO <= configByDifficulty.thresholdBO) return (idxBO, idxSO);

            // var reduce = Mathf.FloorToInt(countLose / configByDifficulty.gapCountLose);
            // var newIdxBO = idxBO - reduce;
            // newIdxBO = Mathf.Max(newIdxBO, configByDifficulty.minIndexBO);
            // return (newIdxBO, idxSO);
            return (idxBO, idxSO);
        }

        public LogicOrderData GetDynamicDifficultyAndValue(LogicOrderData logicOrderData)
        {
            // thay đổi diff theo profile
            var profileInGameService = MySonatFramework.GetService<ProfileInGameService>();
            var (diff, value) = profileInGameService.GetDifficultyValue(logicOrderData.difficulty, logicOrderData.difficultyValue);

            logicOrderData.difficulty = diff;
            logicOrderData.difficultyValue = value;

            // thay đổi diff theo count lose
            Debug.Log("<color=purple>DynamicLogicOrder:</color> count lose: " + countLose);
            var config = dynamicLogicOrderConfigSO.GetConfigByDifficulty(logicOrderData.level, logicOrderData.difficulty, logicOrderData.difficultyValue);
            if (countLose >= config.thresholdLoseCount)
            {
                var delta = config.deltaDiffValue * (countLose - config.thresholdLoseCount + 1);
                var (difficulty, difficultyValue) = LogicOrderHelper.GetDynamicDifficultyAndValue(logicOrderData.difficulty, logicOrderData.difficultyValue, delta);
                logicOrderData.difficulty = difficulty;
                logicOrderData.difficultyValue = difficultyValue;
                return logicOrderData;
            }

            return logicOrderData;
        }
    }
}