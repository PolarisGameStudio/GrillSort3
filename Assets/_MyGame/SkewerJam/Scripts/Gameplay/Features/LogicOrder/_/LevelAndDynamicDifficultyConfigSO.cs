using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "LevelAndDynamicDifficultyConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/LevelAndDynamicDifficultyConfigSO")]
    public class LevelAndDynamicDifficultyConfigSO : ScriptableObject, IComparable<LevelAndDynamicDifficultyConfigSO>
    {
        public int levelStart;
        public List<ConfigByDifficulty> listConfigByDifficulty;

        public int CompareTo(LevelAndDynamicDifficultyConfigSO other)
        {
            return levelStart.CompareTo(other.levelStart);
        }
    }

    [Serializable]
    public class ConfigByDifficulty : IComparable<ConfigByDifficulty>
    {
        public LevelDifficulty difficulty;
        public int difficultyValue;
        // public int gapCountLose; // Gap
        // public int thresholdBO; // ngưỡng indexBO bắt đầu giảm
        // public int minIndexBO; // lượng giảm

        [Header("Config")]
        public int thresholdLoseCount;
        [GUIColor(0f, 1f, 0f)]
        public int deltaDiffValue = -1;

        public int CompareTo(ConfigByDifficulty other)
        {
            if (difficulty.CompareTo(other.difficulty) != 0)
            {
                return difficulty.CompareTo(other.difficulty);
            }
            return difficultyValue.CompareTo(other.difficultyValue);
        }
    }

}