using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "SequenceConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/SequenceConfigSO")]
    public class SequenceConfigSO : ScriptableObject
    {
        public List<PhaseConfig> listPhaseConfigs;


#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var phaseConfig in listPhaseConfigs)
            {
                phaseConfig.index = listPhaseConfigs.IndexOf(phaseConfig);
            }
        }
#endif
    }

    [Serializable]
    public class PhaseConfig
    {
        [GUIColor(0f, 1f, 0f)]
        [ReadOnly]
        public int index;

        [Range(0, 1)]
        public float threshold;
        public int indexBO;
        public int indexSO;
    }
}