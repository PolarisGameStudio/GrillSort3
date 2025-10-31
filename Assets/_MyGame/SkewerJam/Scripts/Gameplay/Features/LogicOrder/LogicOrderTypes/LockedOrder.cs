using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "LockedOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/LockedOrderSO")]
    public class LockedOrderSO : BaseOrderSO
    {
        public override LogicOrderType LogicOrderType => LogicOrderType.Locked;
        public override void Init()
        {

        }

        public override void SetLevelData(LevelData_SkewerJam levelData)
        {

        }

        public override bool CanUse(GameplayInfoForLogicOrder gameplayInfo)
        {
            var lockedItems = ItemHelper.GetItemIdsInLockedGrillInLayer1();
            return lockedItems.Count > 0;
        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo)
        {
            if (CanUse(gameplayInfo) == false)
            {
                return (ItemId.None, 0);
            }

            // lấy ra item Id giống với item bị lock mà có số step nhỏ nhất và tối ưu số num

            var lockedItems = ItemHelper.GetItemIdsInLockedGrillInLayer1();
            if(lockedItems.Count == 0) return (ItemId.None, 0);
            var (itemId, num, step) = OrderHelper.GetOptimizedRandomLockedOrder(lockedItems, gameplayInfo);
            Debug.Log("<color=magenta>OrderHelper:</color> GetOrder Locked: " + itemId + " " + num + " step: " + step);
            return (itemId, num);
        }
    }
}