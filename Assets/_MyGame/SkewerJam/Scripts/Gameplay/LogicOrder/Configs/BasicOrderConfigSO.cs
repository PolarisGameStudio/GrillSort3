using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "BasicOrderConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/BasicOrderConfigSO")]
    public class BasicOrderConfigSO : ScriptableObject
    {
        public List<BO> listBasicOrderConfigs;
        public float rateIndependencyOrder = 0.75f;

#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var config in listBasicOrderConfigs)
            {
                config.index = listBasicOrderConfigs.IndexOf(config);
            }
        }
#endif
    }

    [Serializable]
    public class BO
    {
        [GUIColor(0f, 1f, 0f)]
        [ReadOnly] public int index;

        [Range(0, 1)]
        public float rateWith0Step = 0;
        [Range(0, 1)]
        public float rateWith1Step = 0;
        [Range(0, 1)]
        public float rateWith2Step = 0;
        [Range(0, 1)]
        public float rateWith3Step = 0;

#if UNITY_EDITOR
        public void OnValidate()
        {
            rateWith3Step = 1 - rateWith0Step - rateWith1Step - rateWith2Step;
        }
#endif
    }
}