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
        public List<LevelAndStartCountPRConfigSO> listLRASCs;

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
            var temp = new LevelAndStartCountPRConfigSO() { levelStart = level };
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
            var difficultyAndPR = startCountPR.listDifficultyAndPR.GetFirstGreaterThan(temp3);
            if (difficultyAndPR == null)
            {
                return 0;
            }
            return difficultyAndPR.PR;

        }
    }
}