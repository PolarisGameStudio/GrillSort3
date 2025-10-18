using Manager;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public abstract class BaseOrderSO : ScriptableObject
    {
        public abstract (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null);

        public virtual bool CanUse(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            return true;
        }

        public virtual bool ForceUse(bool isRescue = false)
        {
            return false;
        }
    }
}