using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "FlowConfigSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/FlowConfigSO_v2")]
    public class FlowConfigSO_v2 : ScriptableObject
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