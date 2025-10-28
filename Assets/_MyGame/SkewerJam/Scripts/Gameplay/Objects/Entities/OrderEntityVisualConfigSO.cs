using UnityEngine;

namespace MyGame.SkewerJam.Objects.Entities
{
    [CreateAssetMenu(fileName = "OrderEntityVisualConfigSO", menuName = "MyGame/SkewerJam/Configs/Animations/OrderEntityVisualConfigSO")]
    public class OrderEntityVisualConfigSO : ScriptableObject
    {
        [Header("Complete: Scale")]
        public Vector2 scaleValue = new Vector2(1.2f, 0.8f);
        public float scaleDuration = 0.3f;
    }
}