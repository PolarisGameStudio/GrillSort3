using System;
using System.Linq;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "DynamicLogicOrderConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/DynamicLogicOrderConfigSO")]
    public class DynamicLogicOrderConfigSO : ScriptableObject
    {
        [Range(0, 1)]
        public float completionRate = 0.1f;

        public ConfigByDifficulty[] configs;

        public ConfigByDifficulty GetConfigByDifficulty(LevelDifficulty difficulty)
        {
            return configs.FirstOrDefault(c => c.difficulty == difficulty);
        }
    }

    [Serializable]
    public class ConfigByDifficulty
    {
        public LevelDifficulty difficulty;
        public int gapCountLose; // Gap
        public int thresholdBO; // ngưỡng indexBO bắt đầu giảm
        public int minIndexBO; // lượng giảm
    }
}