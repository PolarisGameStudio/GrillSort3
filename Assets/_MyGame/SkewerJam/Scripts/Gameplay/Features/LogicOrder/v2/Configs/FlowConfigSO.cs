using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "FlowConfigSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/FlowConfigSO_v2")]
    public class FlowConfigSO_v2 : ScriptableObject
    {
        public List<int> listCurveIndices;
    }
}