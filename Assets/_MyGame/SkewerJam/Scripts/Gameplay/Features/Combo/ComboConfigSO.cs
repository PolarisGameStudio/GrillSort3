using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    [CreateAssetMenu(fileName = "ComboConfigSO", menuName = "MyGame/SkewerJam/ComboConfigSO")]
    public class ComboConfigSO : ScriptableObject
    {
        public List<int> comboTime;
    }
}