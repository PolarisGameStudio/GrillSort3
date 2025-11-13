using System;
using System.Collections.Generic;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Config
{
    [CreateAssetMenu(fileName = "LoseStreakPRConfigSO", menuName = "MyGame/Features/ProfileInGame/LoseStreakPRConfigSO")]
    public class LoseStreakPRConfigSO : ScriptableObject
    {
        public List<LevelRangeAndLoseStreakPRConfig> listLevelRangeAndLoseStreakPRConfigs;

        #region OnValidate
        public void OnValidate()
        {
            for (int i = 0; i < listLevelRangeAndLoseStreakPRConfigs.Count - 1; i++)
            {
                var currentLevelRangeAndLoseStreakPRConfig = listLevelRangeAndLoseStreakPRConfigs[i];
                if (currentLevelRangeAndLoseStreakPRConfig.levelStart < listLevelRangeAndLoseStreakPRConfigs[i + 1].levelStart)
                {
                    Debug.LogError($"LoseStreakPRConfig: levelStart {currentLevelRangeAndLoseStreakPRConfig.levelStart} is less than levelStart {listLevelRangeAndLoseStreakPRConfigs[i + 1].levelStart}");
                }

                currentLevelRangeAndLoseStreakPRConfig.listLoseStreakPRs.Sort((a, b) => a.loseStreak.CompareTo(b.loseStreak));
            }
        }
        #endregion
    }

    [Serializable]
    public class LevelRangeAndLoseStreakPRConfig
    {
        public int levelStart;
        public List<LoseStreakPR> listLoseStreakPRs;
    }

    [Serializable]
    public class LoseStreakPR
    {
        public int loseStreak;
        public List<DifficultyAndPR> listDifficultyAndPR;
    }

    [Serializable]
    public class DifficultyAndPR
    {
        public LevelDifficulty difficulty;
        public int difficultyValue;
        public int PR;
    }
}