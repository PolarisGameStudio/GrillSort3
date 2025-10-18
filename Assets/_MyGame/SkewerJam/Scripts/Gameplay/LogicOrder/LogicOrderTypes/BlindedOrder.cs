using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "BlindedOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/BlindedOrderSO")]
    public class BlindedOrderSO : BaseOrderSO
    {
        public override void Init()
        {

        }

        public override void SetLevelData(LevelData_SkewerJam levelData)
        {

        }

        public override bool CanUse(GameplayInfoForLogicOrder gameplayInfo)
        {
            var blindedItems = ItemHelper.GetItemIdsInBlindedGrill();
            return blindedItems.Count > 0;
        }
        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo)
        {
            if (CanUse(gameplayInfo) == false)
            {
                return (ItemId.None, 0);
            }

            // lấy ra item Id giống với item bị lock mà có số step nhỏ nhất và tối ưu số num
            var blindedItems = ItemHelper.GetItemIdsInBlindedGrill();
            var (itemId, num, step) = OrderHelper.GetOptimizedRandomSpecialItem(blindedItems, gameplayInfo);
            Debug.Log("<color=orange>OrderHelper:</color> GetOrder Blinded: " + itemId + " " + num + " step: " + step);
            return (itemId, num);
        }
    }
}