using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Config
{
    [CreateAssetMenu(fileName = "LevelAndStartCountPRConfigSO", menuName = "MyGame/Features/ProfileInGame/LevelAndStartCountPRConfigSO")]
    public class LevelAndStartCountPRConfigSO : ScriptableObject, IComparable<LevelAndStartCountPRConfigSO>
    {
        [GUIColor(1f, 1f, 0f)]
        public int levelStart;
        public List<StartCountPR> listStartCountPRs;

        public int CompareTo(LevelAndStartCountPRConfigSO other)
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