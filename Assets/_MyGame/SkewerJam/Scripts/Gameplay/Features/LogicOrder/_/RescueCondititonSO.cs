using System;
using System.Collections.Generic;
using MyGame.Modules.Utils;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Features.LogicOrder
{
    [CreateAssetMenu(fileName = "RescueCondititonSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/RescueCondititonSO")]
    public class RescueCondititonSO : ScriptableObject
    {
        public List<RescueCondition> listRescueConditions;

        public RescueCondition GetRescueCondition(int level, LevelDifficulty difficulty, int difficultyValue)
        {
            var temp = new RescueCondition() { level = level, difficulty = difficulty, difficultyValue = difficultyValue };
            var result = listRescueConditions.GetFirstGreaterThan(temp);
            return result;
        }
    }

    [Serializable]
    public class RescueCondition : IComparable<RescueCondition>
    {
        [Header("Level Condition")]
        public int level;
        public LevelDifficulty difficulty;
        public int difficultyValue;

        [Header("Rescue Condition")]
        public int maxNumberRescues = 5;
        public int maxRescueGap = 2;
        public int remainingWaitingGrillCondition = 2;
        public float rateRescue = 0.75f;

        public int CompareTo(RescueCondition other)
        {
            if (level.CompareTo(other.level) != 0)
            {
                return level.CompareTo(other.level);
            }
            if (difficulty.CompareTo(other.difficulty) != 0)
            {
                return difficulty.CompareTo(other.difficulty);
            }
            return difficultyValue.CompareTo(other.difficultyValue);
        }

        // public RescueCondition(int level, LevelDifficulty difficulty)
        // {
        //     maxRescueGap = 2;
        //     remainingWaitingGrillCondition = 2;

        //     if ((level - 1) % 100 < 50)
        //     {
        //         switch (difficulty)
        //         {
        //             case LevelDifficulty.Easy:
        //                 maxNumberRescues = 5;
        //                 rateRescue = 0.75f;
        //                 break;
        //             case LevelDifficulty.Medium:
        //                 maxNumberRescues = 4;
        //                 rateRescue = 0.7f;
        //                 break;
        //             case LevelDifficulty.Hard:
        //                 maxNumberRescues = 3;
        //                 rateRescue = 0.65f;
        //                 break;
        //             case LevelDifficulty.SuperHard:
        //                 maxNumberRescues = 2;
        //                 rateRescue = 0.5f;
        //                 break;
        //                 // case LevelDifficulty.Hard1:
        //                 //     maxNumberRescues = 3;
        //                 //     rateRescue = 0.3f;
        //                 //     break;
        //                 // case LevelDifficulty.Hard2:
        //                 //     maxNumberRescues = 2;
        //                 //     rateRescue = 0.2f;
        //                 //     break;
        //         }
        //     }
        //     else
        //     {
        //         switch (difficulty)
        //         {
        //             case LevelDifficulty.Easy:
        //                 maxNumberRescues = 4;
        //                 rateRescue = 0.75f;
        //                 break;
        //             case LevelDifficulty.Medium:
        //                 maxNumberRescues = 3;
        //                 rateRescue = 0.7f;
        //                 break;
        //             case LevelDifficulty.Hard:
        //                 maxNumberRescues = 2;
        //                 rateRescue = 0.65f;
        //                 break;
        //             case LevelDifficulty.SuperHard:
        //                 maxNumberRescues = 2;
        //                 rateRescue = 0.5f;
        //                 break;
        //                 // case LevelDifficulty.Hard1:
        //                 //     maxNumberRescues = 2;
        //                 //     rateRescue = 0.3f;
        //                 //     break;
        //                 // case LevelDifficulty.Hard2:
        //                 //     maxNumberRescues = 1;
        //                 //     rateRescue = 0.2f;
        //                 //     break;
        //         }
        //     }

        //     switch (difficulty)
        //     {
        //         case LevelDifficulty.Hard:
        //             remainingWaitingGrillCondition = 1;
        //             break;
        //     }
    }
}