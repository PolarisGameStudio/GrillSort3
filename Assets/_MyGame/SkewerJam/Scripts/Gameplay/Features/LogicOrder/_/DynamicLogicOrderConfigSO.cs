using System;
using System.Collections.Generic;
using MyGame.Modules.Utils;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "DynamicLogicOrderConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/DynamicLogicOrderConfigSO")]
    public class DynamicLogicOrderConfigSO : ScriptableObject
    {
        [Range(0, 1)]
        public float completionRate = 0.1f;

        public List<LevelAndDynamicDifficultyConfigSO> listLevelAndDynamicDifficultyConfigs;

        public ConfigByDifficulty GetConfigByDifficulty(int level, LevelDifficulty difficulty, int difficultyValue)
        {
            var temp = new LevelAndDynamicDifficultyConfigSO() { levelStart = level };
            var levelAndDynamicDifficultyConfig = listLevelAndDynamicDifficultyConfigs.GetFirstGreaterThan(temp);
            if (levelAndDynamicDifficultyConfig == null)
            {
                return null;
            }

            var temp1 = new ConfigByDifficulty() { difficulty = difficulty, difficultyValue = difficultyValue };
            var config = levelAndDynamicDifficultyConfig.listConfigByDifficulty.GetFirstGreaterThan(temp1);
            if (config == null)
            {
                return null;
            }
            return config;
        }
    }

}