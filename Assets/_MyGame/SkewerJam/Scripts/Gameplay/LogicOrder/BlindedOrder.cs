using Manager;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "BlindedOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/BlindedOrderSO")]
    public class BlindedOrderSO : BaseOrderSO
    {
        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo)
        {
            return (ItemId.None, 0);
        }
    }
}