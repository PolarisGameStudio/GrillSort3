using System;
using System.Collections.Generic;
using Gameplay.LevelData;
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

        // logic order basic
        public List<LogicOrderConfig> logicOrderConfigs;

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
                logicOrderConfigs = this.logicOrderConfigs,
            };
            return levelDataSkewerJam;
        }
    }


    [Serializable]
    public class WaitingGrillData
    {
        public List<GrillData> grillData;
    }

    [Serializable]
    public class OrderData_SkewerJam
    {
        public int id;
        public int active;
        public int currentNumber = 0;
        public int maxNumber = 0;
    }

    [Serializable]
    public class RescueCondition
    {
        public int maxNumberRescues;
        public int maxRescueGap;
        public int remainingWaitingGrill;
    }

    [Serializable]
    public class LogicOrderConfig
    {
        [Range(0, 1)]
        public float region;
        public int minNumberSteps;
    }
}