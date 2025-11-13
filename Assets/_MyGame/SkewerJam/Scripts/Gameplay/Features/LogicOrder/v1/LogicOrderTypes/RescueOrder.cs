using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Features.LogicOrder;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "RescueOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/RescueOrderSO")]
    public class RescueOrderSO : BaseOrderSO
    {
        [SerializeField, ReadOnly] private RescueCondition rescueCondition;
        private int gap = 0;
        private int numberRescues = 0;


        public override LogicOrderType LogicOrderType => LogicOrderType.Rescue;
        public override void Init()
        {
            gap = 0;
            numberRescues = 0;
        }

        public void SetRescueCondition(RescueCondition rescueCondition)
        {
            this.rescueCondition = rescueCondition;
        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            // Sửa lại: Ưu tiên Rescue (-1) slot. Nếu không có mới rescue (-2) và (-3) Slot
            // Rescue nên là Order từ 2 item trở lên, tránh rescue ra 1 item clear đi luôn. Nếu quét không có Order 2 item thì mới ra Order 1 item

            // deltaSlot < 0 ==> rescus
            // numItems > 2 ==> 

            var dictDeltaSlots = gameplayInfo.DictDeltaSlots;
            var listAllRescues = new List<(ItemId itemId, int numItems, int deltaSlot)>();
            // duyệt qua numItems >= 2 trước
            foreach (var itemId in dictDeltaSlots.Keys)
            {
                foreach (var numItems in dictDeltaSlots[itemId].Keys)
                {
                    if (dictDeltaSlots[itemId][numItems] < 0)
                    {
                        listAllRescues.Add((itemId, numItems, dictDeltaSlots[itemId][numItems]));
                    }
                }
            }

            List<(ItemId itemId, int numItems, int deltaSlot)> filteredRescues = new List<(ItemId itemId, int numItems, int deltaSlot)>();

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;

            var start = -1;
            var logicOrderHandler = orderManager.LogicOrderHandler;
            if (logicOrderHandler.IsForceRescue == true)
            {
                start = logicOrderHandler.ForceRescueData.deltaSlot;
            }
            for (int i = start; i >= -3; i--)
            {
                filteredRescues = listAllRescues.Where(e => e.deltaSlot == i && e.numItems >= 2).ToList();
                if (filteredRescues.Count() > 0)
                {
                    var random = UnityEngine.Random.Range(0, filteredRescues.Count());
                    var rescue = filteredRescues.ElementAt(random);
                    gap = 1;
                    return (rescue.itemId, rescue.numItems);
                }
            }

            if (listAllRescues.Count() > 0)
            {
                var rand = UnityEngine.Random.Range(0, listAllRescues.Count());
                var rc = listAllRescues.ElementAt(rand);
                gap = 1;
                return (rc.itemId, rc.numItems);
            }
            else
            {
                return (ItemId.None, 0);
            }
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
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var logicOrderHandler = orderManager.LogicOrderHandler;
            if (logicOrderHandler.IsForceRescue == true)
            {
                return true;
            }


            if (gap != 0)
            {
                gap += 1;
                if (gap > rescueCondition.maxRescueGap)
                {
                    gap = 0;
                }
            }

            if (CanUse())
            {
                if (numberRescues >= rescueCondition.maxNumberRescues)
                {
                    return false;
                }
                var numWaitingGrill = GameController.Instance.GameLogicHandler.WaitingGrillManager.ListWaitingGrills.Count(e => e.IsActive);
                var itemsInWaitingGrill = OrderHelper.GetItemsInWaitingGrill();

                if (isRescue == true || (gap == 0 && itemsInWaitingGrill.Count >= (numWaitingGrill - rescueCondition.remainingWaitingGrillCondition)))
                {
                    var random = UnityEngine.Random.Range(0, 1.0f);
                    Debug.Log("<color=blue>OrderHelper:</color> Random to rescue: " + Mathf.Round(random * 100f) * 0.01f + " >>> " + rescueCondition.rateRescue);
                    if (random < rescueCondition.rateRescue)
                    {
                        numberRescues += 1;
                        gap += 1;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}