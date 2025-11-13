using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public abstract class BaseOrderSO : ScriptableObject
    {
        public abstract LogicOrderType LogicOrderType { get; }
        public abstract void Init();
        public virtual void SetLevelData(LevelData_SkewerJam levelData){

        }

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