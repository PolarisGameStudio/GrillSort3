using System;
using System.Collections.Generic;
using Gameplay.LevelData;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Level
{
    public class LevelData_SkewerJam : LevelData
    {
        public int numberOfWaitingGrill;
        public int numberOfOrder;

        public List<WaitingGrillData> ListWaitingGrillData { get; set; } = new List<WaitingGrillData>();
        public List<OrderData_SkewerJam> ListOrderData { get; set; } = new List<OrderData_SkewerJam>();

        // logic order rescue
        public RescueCondition rescueCondition;
        public int sequenceLogicOrderIndex;

        // // logic order basic
        // public List<LogicOrderConfig> logicOrderConfigs;

        public LevelData_SkewerJam CloneSkewerJam()
        {
            var levelData = base.Clone();

            var levelDataSkewerJam = new LevelData_SkewerJam()
            {
                gameMode = this.gameMode,
                level = this.level,
                category = this.category,

                time = levelData.time,
                levelType = levelData.levelType,
                difficulty = levelData.difficulty,

                grillData = levelData.grillData,
                conveyorData = levelData.conveyorData,
                orderData = levelData.orderData,
                obstacleData = levelData.obstacleData,
                isDropMode = levelData.isDropMode,

                numberOfWaitingGrill = this.numberOfWaitingGrill,
                numberOfOrder = this.numberOfOrder,

                rescueCondition = this.rescueCondition,
                sequenceLogicOrderIndex = this.sequenceLogicOrderIndex,
                // logicOrderConfigs = this.logicOrderConfigs,
            };
            return levelDataSkewerJam;
        }
    }


    [Serializable]
    public class WaitingGrillData
    {
        public int id;
        public int active;
    }

    [Serializable]
    public class OrderData_SkewerJam
    {
        public int id;
        public int active;
    }

    [Serializable]
    public class RescueCondition
    {
        public int maxNumberRescues = 5;
        public int maxRescueGap = 2;
        public int remainingWaitingGrillCondition = 2;
        public float rateRescue = 0.75f;

        public RescueCondition(int level, LevelDifficulty difficulty)
        {
            maxRescueGap = 2;
            remainingWaitingGrillCondition = 2;

            if ((level - 1) % 100 < 50)
            {
                switch (difficulty)
                {
                    case LevelDifficulty.Easy1:
                        maxNumberRescues = 5;
                        rateRescue = 0.75f;
                        break;
                    case LevelDifficulty.Easy2:
                        maxNumberRescues = 4;
                        rateRescue = 0.7f;
                        break;
                    case LevelDifficulty.Medium1:
                        maxNumberRescues = 3;
                        rateRescue = 0.65f;
                        break;
                    case LevelDifficulty.Medium2:
                        maxNumberRescues = 3;
                        rateRescue = 0.5f;
                        break;
                    case LevelDifficulty.Hard1:
                        maxNumberRescues = 3;
                        rateRescue = 0.3f;
                        break;
                    case LevelDifficulty.Hard2:
                        maxNumberRescues = 2;
                        rateRescue = 0.2f;
                        break;
                }
            }
            else
            {
                switch (difficulty)
                {
                    case LevelDifficulty.Easy1:
                        maxNumberRescues = 4;
                        rateRescue = 0.75f;
                        break;
                    case LevelDifficulty.Easy2:
                        maxNumberRescues = 3;
                        rateRescue = 0.7f;
                        break;
                    case LevelDifficulty.Medium1:
                        maxNumberRescues = 2;
                        rateRescue = 0.65f;
                        break;
                    case LevelDifficulty.Medium2:
                        maxNumberRescues = 2;
                        rateRescue = 0.5f;
                        break;
                    case LevelDifficulty.Hard1:
                        maxNumberRescues = 2;
                        rateRescue = 0.3f;
                        break;
                    case LevelDifficulty.Hard2:
                        maxNumberRescues = 1;
                        rateRescue = 0.2f;
                        break;
                }
            }

            switch (difficulty)
            {
                case LevelDifficulty.Hard2:
                    remainingWaitingGrillCondition = 1;
                    break;
            }
        }
    }

    [Serializable]
    public class LogicOrderConfig
    {
        [Range(0, 1)]
        public float threshold;
        public int indexBO;
        public int indexSO;
    }
}