using System.Linq;
using Manager;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "RescueOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/RescueOrderSO")]
    public class RescueOrderSO : BaseOrderSO
    {
        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            // ---Tạo order để giải cứu---
            // Item được lấy từ hàng chờ --> Làm giảm số lượng khay trống nhiều nhất
            // check lại số lượng order nữa

            var items = OrderHelper.GetItemsInWaitingGrill();
            if (items.Count == 0)
            {
                Debug.Log("<color=yellow>OrderHelper:</color> GetItemOrderToRescue: No items in waiting grill");
                return (ItemId.None, 0);
            }

            var dictItems = items.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());
            var dictNeededItems = OrderHelper.GetNeededItemsForCurrentOrder();
            foreach (var id in dictNeededItems.Keys)
            {
                if (dictItems.ContainsKey((int)id))
                {
                    dictItems[(int)id] -= dictNeededItems[id];
                    if (dictItems[(int)id] <= 0)
                    {
                        dictItems.Remove((int)id);
                    }
                }
            }

            if (dictItems.Count == 0)
            {
                Debug.Log("<color=yellow>OrderHelper:</color> GetItemOrderToRescue: No items to rescue");
                return (ItemId.None, 0);
            }

            var itemId = dictItems.OrderByDescending(e => e.Value).First().Key;
            var num = dictItems[itemId] > 3 ? 3 : dictItems[itemId];

            Debug.Log("<color=blue>OrderHelper:</color> GetItemOrderToRescue: " + itemId + " " + num);
            return ((ItemId)itemId, num);
        }

        public override bool CanUse(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            var items = OrderHelper.GetItemsInWaitingGrill();
            if (items.Count == 0)
            {
                // Debug.Log("<color=yellow>OrderHelper:</color> CanUseRescueOrder: No items in waiting grill");
                return false;
            }

            var dictItems = items.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());
            var dictNeededItems = OrderHelper.GetNeededItemsForCurrentOrder();
            foreach (var id in dictNeededItems.Keys)
            {
                if (dictItems.ContainsKey((int)id))
                {
                    dictItems[(int)id] -= dictNeededItems[id];
                    if (dictItems[(int)id] <= 0)
                    {
                        dictItems.Remove((int)id);
                    }
                }
            }

            if (dictItems.Count == 0)
            {
                // Debug.Log("<color=yellow>OrderHelper:</color> CanUseRescueOrder: No items to rescue");
                return false;
            }
            return true;
        }

        public override bool ForceUse(bool isRescue = false)
        {
            if (CanUse())
            {
                var itemsInWaitingGrill = OrderHelper.GetItemsInWaitingGrill();
                if (itemsInWaitingGrill.Count >= 3)
                {
                    return true;
                }
                return isRescue;
            }
            return false;
        }
    }
}