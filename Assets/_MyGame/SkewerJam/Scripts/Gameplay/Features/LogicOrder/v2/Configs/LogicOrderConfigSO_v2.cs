using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "LogicOrderConfigSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/LogicOrderConfigSO_v2")]
    public class LogicOrderConfigSO_v2 : ScriptableObject
    {
        public List<DifficultyAndFlowConfig> difficultyAndFlowConfigs;

        public FlowConfigSO_v2 GetFlowConfigSO(LevelDifficulty difficulty, int difficultyValue)
        {
            var difficultyAndFlowConfig = difficultyAndFlowConfigs.FirstOrDefault(e => e.difficulty == difficulty);
            if (difficultyAndFlowConfig == null)
            {
                difficultyAndFlowConfig = difficultyAndFlowConfigs.LastOrDefault();
            }
            return difficultyAndFlowConfig.GetFlowConfigSO(difficultyValue);
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var difficultyAndFlowConfig in difficultyAndFlowConfigs)
            {
                foreach (var difficultyValueAndFlowConfigSO in difficultyAndFlowConfig.listFlowConfigSOs)
                {
                    difficultyValueAndFlowConfigSO.index = difficultyAndFlowConfig.listFlowConfigSOs.IndexOf(difficultyValueAndFlowConfigSO);
                }
            }
        }
#endif
    }

    [Serializable]
    public class DifficultyAndFlowConfig
    {
        [GUIColor(0f, 1f, 0f)]
        public LevelDifficulty difficulty;
        public List<DifficultyValueAndFlowConfigSO_v2> listFlowConfigSOs;

        public FlowConfigSO_v2 GetFlowConfigSO(int difficultyValue)
        {
            if (difficultyValue < 0)
            {
                difficultyValue = 0;
            }
            if (difficultyValue >= listFlowConfigSOs.Count)
            {
                difficultyValue = listFlowConfigSOs.Count - 1;
            }

            var result = listFlowConfigSOs[difficultyValue];
            return result.GetRandomFlowConfigSO();
        }
    }

    [Serializable]
    public class DifficultyValueAndFlowConfigSO_v2
    {
        [GUIColor(1f, 1f, 1f)]
        [ReadOnly]
        public int index;
        public List<FlowConfigSO_v2> listFlowConfigSOs;

        public FlowConfigSO_v2 GetRandomFlowConfigSO()
        {
            var random = UnityEngine.Random.Range(0, listFlowConfigSOs.Count);
            return listFlowConfigSOs[random];
        }
    }
}