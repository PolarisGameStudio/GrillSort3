using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Manager;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public static class OrderHelper
    {
        public static List<Item> GetItemsInWaitingGrill()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            var itemsInWaiting = waitingGrillManager.ListWaitingGrills.Select(e => e.GetSlots()[0].GetItem());
            return itemsInWaiting.Where(e => e != null).ToList();
        }

        public static Dictionary<ItemId, int> GetNeededItemsForCurrentOrder()
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var orderItemsDict = orderManager.GetOrderItemsDict();
            return orderItemsDict.ToDictionary(e => e.Key, e => e.Value.maxItems - e.Value.num);
        }

        public static (ItemId itemId, int num, int step) GetOptimizedRandomItem(List<ItemId> randomItemIds, int step, GameplayInfoForLogicOrder info)
        {
            var dictNeededSlots = info.DictNeededSlots;
            var maxNum = 0;
            var itemIdsList = new List<ItemId>();
            foreach (var itemId in randomItemIds)
            {
                foreach (var num in dictNeededSlots[itemId].Keys.Where(e => dictNeededSlots[itemId][e] == step))
                {
                    if (num > maxNum)
                    {
                        maxNum = num;
                        itemIdsList.Clear();
                        itemIdsList.Add(itemId);
                    }
                    else if (num == maxNum)
                    {
                        itemIdsList.Add(itemId);
                    }
                }
            }
            var randomId = itemIdsList[UnityEngine.Random.Range(0, itemIdsList.Count)];
            return ((ItemId)randomId, maxNum, step);
        }
    }
}