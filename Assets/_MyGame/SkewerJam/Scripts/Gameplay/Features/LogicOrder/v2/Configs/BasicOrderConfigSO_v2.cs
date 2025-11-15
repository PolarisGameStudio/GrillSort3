using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "BasicOrderConfigSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/BasicOrderConfigSO_v2")]
    public class BasicOrderConfigSO_v2 : ScriptableObject
    {
        public List<CurveConfig> listCurveConfigs;

        public int GetRandomRemainingSlot(int curveIndex)
        {
            var curveConfig = listCurveConfigs[Mathf.Clamp(curveIndex, 0, listCurveConfigs.Count - 1)];
            var random = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("<color=purple>BasicOrderConfigSO_v2: GetRandomRemainingSlot: curveIndex: " + curveIndex + "</color> >>> random: " + random);
            var str = "";
            foreach (var rateConfig in curveConfig.listRateConfigs)
            {
                str += $"({rateConfig.remainingSlot}, {rateConfig.rate}) >> ";
            }
            Debug.Log("BasicOrderConfigSO_v2: curveConfig: >>> " + str);

            var accumulatedRate = 0f;
            int i = 0;
            foreach (var rateConfig in curveConfig.listRateConfigs)
            {
                accumulatedRate += rateConfig.rate;
                if (random < accumulatedRate)
                {
                    Debug.Log("BasicOrderConfigSO_v2: GetRandomRemainingSlot: rateConfig: " + i);
                    return rateConfig.remainingSlot;
                }
                i++;
            }
            return 0;
        }
#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var curveConfig in listCurveConfigs)
            {
                curveConfig.index = listCurveConfigs.IndexOf(curveConfig);
                foreach (var rateConfig in curveConfig.listRateConfigs)
                {
                    rateConfig.index = curveConfig.listRateConfigs.IndexOf(rateConfig);
                    rateConfig.remainingSlot = 5 - rateConfig.index;
                }
            }
        }

        public int GetMaxRemainingSlot()
        {
            return 5;
        }
#endif
    }

    [Serializable]
    public class CurveConfig
    {
        [GUIColor(0f, 1f, 0f)]
        [ReadOnly]
        public int index;
        public List<RateConfig> listRateConfigs;
    }


    [Serializable]
    public class RateConfig
    {
        [GUIColor(0f, 1f, 0f)]
        [ReadOnly]
        public int index;
        public int remainingSlot;

        [Range(0, 1)]
        public float rate;
    }
}