using Manager;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "LockedOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/LockedOrderSO")]
    public class LockedOrderSO : BaseOrderSO
    {
        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo)
        {
            return (ItemId.None, 0);
        }
    }
}