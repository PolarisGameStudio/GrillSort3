using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "SpecialOrderConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/SpecialOrderConfigSO")]
    public class SpecialOrderConfigSO : ScriptableObject
    {
        public List<SO> listSpecialOrderConfigs;
#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var config in listSpecialOrderConfigs)
            {
                config.index = listSpecialOrderConfigs.IndexOf(config);
            }
        }
#endif
    }

    [Serializable]
    public class SO
    {
        [GUIColor(0f, 1f, 0f)]
        [ReadOnly] public int index;

        [Range(0, 1)]
        public float rateBasicOrder;
        [Range(0, 1)]
        public float rateLockedOrder;
        [Range(0, 1)]
        public float rateBlindedOrder;

#if UNITY_EDITOR
        public void OnValidate()
        {
            rateBlindedOrder = 1 - rateBasicOrder - rateLockedOrder;
        }
#endif
    }
}