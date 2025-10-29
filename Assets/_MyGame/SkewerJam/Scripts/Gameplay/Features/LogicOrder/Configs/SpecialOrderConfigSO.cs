using System;
using System.Collections.Generic;
using MyGame.SkewerJam.Gameplay.Helpers;
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
        [Range(0, 1)]
        public float rateTricky1Order;
        [Range(0, 1)]
        public float rateTricky2Order;

        public LogicOrderType GetLogicOrderType()
        {
            var random = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("<color=white>LogicOrderHandler:</color> ChooseLogicOrder: " + Mathf.Round(random * 100f) * 0.01f + " >>> " + rateBasicOrder + " - " + rateLockedOrder + " - " + rateBlindedOrder + " - " + rateTricky1Order + " - " + rateTricky2Order);
            if (random < rateBasicOrder)
            {
                return LogicOrderType.Basic;
            }
            else if (random < rateBasicOrder + rateLockedOrder)
            {
                return LogicOrderType.Locked;
            }
            else if (random < rateBasicOrder + rateLockedOrder + rateBlindedOrder)
            {
                return LogicOrderType.Blinded;
            }
            else if (random < rateBasicOrder + rateLockedOrder + rateBlindedOrder + rateTricky1Order)
            {
                return LogicOrderType.Tricky1;
            }
            else
            {
                return LogicOrderType.Tricky2;
            }
        }
    }
}