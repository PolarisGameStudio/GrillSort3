using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "LogicOrderConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/LogicOrderConfigSO")]
    public class LogicOrderConfigSO : ScriptableObject
    {
        public List<SequenceConfigSO> listSequenceConfigs;
    }
}