using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "HardFlowConfigSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/HardFlowConfigSO_v2")]
    public class HardFlowConfigSO_v2 : ScriptableObject
    {
        public List<int> listCurveIndices;

        public int GetCurveIndex(int phase)
        {
            if (phase < 0)
            {
                return 0;
            }

            if (phase >= listCurveIndices.Count)
            {
                return listCurveIndices[^1];
            }

            return listCurveIndices[phase];
        }
    }
}