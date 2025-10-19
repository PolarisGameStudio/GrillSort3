using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;

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

        public static (ItemId itemId, int num, int step) GetOptimizedRandomItem(List<ItemId> randomItemIds, int step, GameplayInfoForLogicOrder info, int minNum = 1)
        {
            var dictNeededSlots = info.DictNeededSlots;
            var maxNum = 0;
            var itemIdsList = new List<ItemId>();
            foreach (var itemId in randomItemIds)
            {
                foreach (var num in dictNeededSlots[itemId].Keys.Where(e => dictNeededSlots[itemId][e] == step))
                {
                    if (num < minNum) continue;
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

        public static (ItemId itemId, int num, int step) GetOptimizedRandomSpecialItem(List<ItemId> randomItemIds, GameplayInfoForLogicOrder info)
        {
            var dictNeededSlots = info.DictNeededSlots;
            var listRandomItemIds = new List<ItemId>();
            var minStep = int.MaxValue;

            foreach (var itemId in randomItemIds)
            {
                if (dictNeededSlots.ContainsKey(itemId))
                {
                    var step = dictNeededSlots[itemId].Values.Min();
                    if (step < minStep)
                    {
                        minStep = step;
                        listRandomItemIds.Clear();
                        listRandomItemIds.Add(itemId);
                    }
                    else if (step == minStep)
                    {
                        listRandomItemIds.Add(itemId);
                    }
                }
            }
            var randomItemId = listRandomItemIds[UnityEngine.Random.Range(0, listRandomItemIds.Count)];
            var dictNumAndStep = dictNeededSlots[randomItemId];
            var num = dictNumAndStep.Keys.Where(e => dictNumAndStep[e] == minStep).OrderBy(e => -e).FirstOrDefault();

            // BUG:Cần phòng trường hợp cuối ván thừa/ thiếu item trong order
            num = num + 1 > 3 ? 3 : num + 1;
            return (randomItemId, num, minStep);
        }
    }
}