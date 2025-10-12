using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    [CreateAssetMenu(fileName = "OrderManagerSO", menuName = "MyGame/SkewerJam/OrderConfigs/OrderManagerSO")]
    public class OrderManagerSO : ScriptableObject
    {
        public int MaxOrder;
        public int DefaultNumberOfReadyOrder;
    }
}