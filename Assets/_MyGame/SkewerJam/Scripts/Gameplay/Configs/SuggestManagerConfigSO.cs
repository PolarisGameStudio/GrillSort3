using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "SuggestManagerConfigSO", menuName = "MyGame/SkewerJam/Configs/SuggestManagerConfigSO")]
    public class SuggestManagerConfigSO : ScriptableObject
    {
        public float waitSuggestItemsTime = 15f;
        public float waitSuggestBoostersTime = 15f;
    }
}