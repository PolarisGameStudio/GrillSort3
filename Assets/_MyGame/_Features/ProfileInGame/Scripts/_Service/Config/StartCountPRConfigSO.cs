using System;
using System.Collections.Generic;
using MyGame.Modules.Utils;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Config
{
    [CreateAssetMenu(fileName = "LoseStreakPRConfigSO", menuName = "MyGame/Features/ProfileInGame/LoseStreakPRConfigSO")]
    public class StartCountPRConfigSO : ScriptableObject
    {
        public List<LevelRangeAndStartCountPRConfig> listLRASCs;

        #region OnValidate
        public void OnValidate()
        {
            for (int i = 0; i < listLRASCs.Count - 1; i++)
            {
                var currentLevelRangeAndLoseStreakPRConfig = listLRASCs[i];
                if (currentLevelRangeAndLoseStreakPRConfig.levelStart >= listLRASCs[i + 1].levelStart)
                {
                    Debug.LogError($"LoseStreakPRConfig: levelStart {currentLevelRangeAndLoseStreakPRConfig.levelStart} is greater than or equal to levelStart {listLRASCs[i + 1].levelStart}");
                }
            }
        }
        #endregion

        public int GetPRByStartCount(int level, LevelDifficulty difficulty, int difficultyValue, int startCount)
        {
            var temp = new LevelRangeAndStartCountPRConfig() { levelStart = level };
            var levelRangeAndStartCountPRConfig = listLRASCs.GetFirstGreaterThan(temp);
            if (levelRangeAndStartCountPRConfig == null)
            {
                return 0;
            }

            var temp2 = new StartCountPR() { startCountMilestone = startCount };
            var startCountPR = levelRangeAndStartCountPRConfig.listStartCountPRs.GetFirstGreaterThan(temp2);
            if (startCountPR == null)
            {
                return 0;
            }
            var temp3 = new DifficultyAndPR() { difficulty = difficulty, difficultyValue = difficultyValue };
            return startCountPR.listDifficultyAndPR.GetFirstGreaterThan(temp3).PR;

        }
    }

    [Serializable]
    public class LevelRangeAndStartCountPRConfig : IComparable<LevelRangeAndStartCountPRConfig>
    {
        [GUIColor(1f, 1f, 0f)]
        public int levelStart;
        public List<StartCountPR> listStartCountPRs;

        public int CompareTo(LevelRangeAndStartCountPRConfig other)
        {
            return levelStart.CompareTo(other.levelStart);
        }
    }

    [Serializable]
    public class StartCountPR : IComparable<StartCountPR>
    {
        public int startCountMilestone;
        public List<DifficultyAndPR> listDifficultyAndPR;

        public int CompareTo(StartCountPR other)
        {
            return startCountMilestone.CompareTo(other.startCountMilestone);
        }
    }

    [Serializable]
    public class DifficultyAndPR : IComparable<DifficultyAndPR>
    {
        public LevelDifficulty difficulty;
        public int difficultyValue;
        [GUIColor(0f, 1f, 0f)]
        public int PR;

        public int CompareTo(DifficultyAndPR other)
        {
            if (difficulty == other.difficulty)
            {
                return difficultyValue.CompareTo(other.difficultyValue);
            }
            return difficulty.CompareTo(other.difficulty);
        }
    }
}