using System.Linq;
using MyGame.Modules.Utils;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.TimeManagement;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameService", menuName = "MyGame/Features/ProfileInGame/ProfileInGameService")]
    public class ProfileInGameService : MyService<ProfileInGameConfigSO>
    {
        public override string DATA_KEY => "PROFILE_IN_GAME";
        public ProfileInGameInventoryModule inventoryModule;

        private LongDataPref _lastPlayTime;
        private int _pr => inventoryModule.GetPR();

        #region Init
        protected override void Init()
        {
            new EventBinding<LevelStartedEvent>(OnLevelStarted);
            new EventBinding<LevelEndedEvent>(OnLevelEnded);
            new EventBinding<LevelContinueEvent>(OnLevelContinue);

            new EventBinding<UseBoosterEvent>(OnUseBooster);

            if (CheckLastPlayTime())
            {
                ResetData();
            }
        }

        protected override void LoadConfig()
        {

        }

        protected override void LoadData()
        {
            _lastPlayTime = new LongDataPref($"{DATA_KEY}_LastPlayTime");
            inventoryModule.LoadData();
        }

        protected override void ResetData()
        {
            inventoryModule.ResetData();
            _lastPlayTime.Value = 0;
        }

        private bool CheckLastPlayTime()
        {
            var now = MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
            long daysToResetSeconds = config.daysToReset * 24 * 60 * 60;
            return now - _lastPlayTime.Value > daysToResetSeconds;
        }

        #endregion

        #region Event Handlers
        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            inventoryModule.UpdateLevelStarted(eventData.level);
            _lastPlayTime.Value = MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
        }

        private void OnLevelEnded(LevelEndedEvent eventData)
        {
            inventoryModule.UpdateLevelEnded(eventData.success);
        }

        private void OnLevelContinue(LevelContinueEvent eventData)
        {
            inventoryModule.UpdateReviveUsed();
        }

        private void OnUseBooster(UseBoosterEvent eventData)
        {
            inventoryModule.UpdateBoosterUsed(eventData.booster);
        }
        #endregion

        public (LevelDifficulty difficulty, int difficultyValue) GetDifficultyValue(LevelDifficulty difficulty, int difficultyValue)
        {
            var playerRankConfig = GetPlayerRankConfig();
            var changeDifficultyValue = playerRankConfig.changeDifficultyValue;


            Debug.Log("ProfileInGame:" + _pr + " diff: " + difficulty + " value: " + difficultyValue + " change: " + changeDifficultyValue);
            var maxValue = config.GetMaxValue(difficulty);
            var newValue = difficultyValue + changeDifficultyValue;

            while (newValue > maxValue)
            {
                newValue = newValue - maxValue;
                difficulty += 1;
                if (difficulty >= LevelDifficulty.MAX)
                {
                    newValue = maxValue;
                    difficulty = LevelDifficulty.MAX - 1;
                    break;
                }

                maxValue = config.GetMaxValue(difficulty);
            }

            while (newValue < 0)
            {
                var preMaxValue = config.GetMaxValue(difficulty - 1);
                newValue = preMaxValue + newValue;
                difficulty -= 1;

                if (difficulty < LevelDifficulty.Easy)
                {
                    newValue = 0;
                    difficulty = LevelDifficulty.Easy;
                    break;
                }

                maxValue = config.GetMaxValue(difficulty);
            }

            return (difficulty, newValue);
        }

        private PlayerRankConfig GetPlayerRankConfig()
        {
            var temp = new PlayerRankConfig() { prMileStone = _pr };
            var result = config.listPlayerRankConfigs.GetFirstGreaterThan(temp);
            return result;
        }
    }
}
