using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public class GameplayInfoForLogicOrder
    {
        public int phase;

        private Dictionary<ItemId, Dictionary<int, int>> _dictNeededSlots;
        private Dictionary<ItemId, Dictionary<int, int>> _dictDeltaSlots;

        public Dictionary<ItemId, Dictionary<int, int>> DictNeededSlots => _dictNeededSlots;
        public Dictionary<ItemId, Dictionary<int, int>> DictDeltaSlots => _dictDeltaSlots;

        public void UpdateState()
        {
            // đã loại các item và grill bị lock
            _dictNeededSlots = GetDictNeededSlots();
            _dictDeltaSlots = GetDictDeltaSlots(_dictNeededSlots);
        }

        private Dictionary<ItemId, Dictionary<int, int>> GetDictNeededSlots()
        {
            // dynamic programming
            // neededSlot = số slot cần để ăn được order (numItems của itemId)
            // deltaSlot = lượng slot thay đổi để có thể ăn được order (numItems của itemId)
            var dp = new Dictionary<ItemId, Dictionary<int, int>>(); // itemId/numItems/neededSlot

            // Tính toán order: Sẽ luôn chọn item tối ưu giúp clear order
            var neededItemsForCurrentOrder = OrderHelper.GetNeededItemsForCurrentOrder();
            CalculateOnWaitingGrill(ref dp, ref neededItemsForCurrentOrder);
            CalculateOnGrill(ref dp, ref neededItemsForCurrentOrder);
            return dp;
        }

        private Dictionary<ItemId, Dictionary<int, int>> GetDictDeltaSlots(Dictionary<ItemId, Dictionary<int, int>> dictNeededSlots)
        {
            var dp = new Dictionary<ItemId, Dictionary<int, int>>(dictNeededSlots); // itemId/numItems/deltaSlot

            var listWaitingItems = OrderHelper.GetItemsInWaitingGrill();
            var dictWaitingItems = listWaitingItems.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());
            foreach (var itemId in dictWaitingItems.Keys)
            {
                if (dp.ContainsKey((ItemId)itemId))
                {
                    var listNumItems = dictNeededSlots[(ItemId)itemId].Keys.ToList();
                    foreach (var numItems in listNumItems)
                    {
                        var neededSlot = dp[(ItemId)itemId][numItems];
                        dp[(ItemId)itemId][numItems] = neededSlot - dictWaitingItems[itemId];
                    }
                }
            }
            return dp;
        }

        private void CalculateOnWaitingGrill(ref Dictionary<ItemId, Dictionary<int, int>> dp, ref Dictionary<ItemId, int> neededItemsForCurrentOrder)
        {
            var listWaitingItems = OrderHelper.GetItemsInWaitingGrill();
            foreach (var item in listWaitingItems)
            {
                if (neededItemsForCurrentOrder.ContainsKey((ItemId)item.id))
                {
                    neededItemsForCurrentOrder[(ItemId)item.id]--;
                    if (neededItemsForCurrentOrder[(ItemId)item.id] <= 0)
                    {
                        neededItemsForCurrentOrder.Remove((ItemId)item.id);
                    }
                    continue;
                }

                if (dp.ContainsKey((ItemId)item.id) == false)
                {
                    dp[(ItemId)item.id] = new Dictionary<int, int>() { { 1, 0 } };
                }
                else
                {
                    var numInDp = dp[(ItemId)item.id].Count;
                    if (numInDp < 3)
                    {
                        dp[(ItemId)item.id][numInDp + 1] = 0;
                    }
                }
            }
        }

        private void CalculateOnGrill(ref Dictionary<ItemId, Dictionary<int, int>> dp, ref Dictionary<ItemId, int> neededItemsForCurrentOrder)
        {
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var dictCountSlotByGrill = new Dictionary<int, int>(); // số slot mỗi grill
            var dictNumItemsInUpperLayer = new Dictionary<int, List<Item>>(); // số lượng item ở layer trên đó

            // duyệt qua layer 0
            CalculateOnLayer0(ref dp, ref neededItemsForCurrentOrder, ref dictCountSlotByGrill, ref dictNumItemsInUpperLayer);

            // duyệt qua layer 1
            CalculateOnLayer1(ref dp, ref neededItemsForCurrentOrder, ref dictCountSlotByGrill, ref dictNumItemsInUpperLayer);
        }

        private void CalculateOnLayer0(ref Dictionary<ItemId, Dictionary<int, int>> dp, ref Dictionary<ItemId, int> neededItemsForCurrentOrder, ref Dictionary<int, int> dictCountSlotByGrill, ref Dictionary<int, List<Item>> dictNumItemsInUpperLayer)
        {
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            foreach (var primaryGrill in grillManager.ListGrills)
            {
                if (primaryGrill.IsLock) continue;
                var slots = primaryGrill.GetSlots();
                if (slots == null) continue;

                var listCurrentItems = slots.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
                var dictItems = listCurrentItems.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());

                foreach (var itemId in dictItems.Keys)
                {
                    if (ItemHelper.IsItemSpecial(itemId)) continue;
                    var numItems = dictItems[itemId];
                    // cần chọn item tối ưu cho order
                    if (neededItemsForCurrentOrder.ContainsKey(itemId))
                    {
                        var neededItems = neededItemsForCurrentOrder[itemId];
                        neededItemsForCurrentOrder[itemId] -= numItems;
                        if (neededItemsForCurrentOrder[itemId] <= 0)
                        {
                            neededItemsForCurrentOrder.Remove(itemId);
                        }

                        numItems = numItems - neededItems;
                    }
                    if (numItems <= 0) continue;

                    var numInDp = dp.ContainsKey(itemId) ? dp[itemId].Count : 0;
                    dp.TryAdd(itemId, new Dictionary<int, int>());
                    for (int j = 1; j <= numItems; j++)
                    {
                        if (numInDp + j > 3) break;
                        dp[itemId].TryAdd(numInDp + j, 0);
                    }
                }
                dictNumItemsInUpperLayer[primaryGrill.id] = listCurrentItems;
                dictCountSlotByGrill[primaryGrill.id] = listCurrentItems.Count;
            }
        }

        private void CalculateOnLayer1(ref Dictionary<ItemId, Dictionary<int, int>> dp, ref Dictionary<ItemId, int> neededItemsForCurrentOrder, ref Dictionary<int, int> dictCountSlotByGrill, ref Dictionary<int, List<Item>> dictNumItemsInUpperLayer)
        {
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;

            // Duyệt lần 1 để lấy item tối ưu còn lại cho order
            var dictListItemStepInfo = new Dictionary<ItemId, List<(List<Item>, int)>>(); // Danh sách các loại item và step cần cửa nó: itemId/(Item, số step cần cửa nó)
            foreach (var primaryGrill in grillManager.ListGrills)
            {
                if (primaryGrill.IsLock) continue;
                var subGrills = primaryGrill.GetSubGrills();
                if (subGrills == null || subGrills.Count == 0) continue;
                var slots = subGrills[0].GetSlots();
                if (slots == null) continue;

                var listCurrentItems = slots.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
                var dictItems = listCurrentItems.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());
                foreach (var itemId in dictItems.Keys)
                {
                    if (ItemHelper.IsItemSpecial(itemId)) continue;
                    if (neededItemsForCurrentOrder.ContainsKey(itemId))
                    {
                        dictListItemStepInfo.TryAdd(itemId, new List<(List<Item>, int)>());

                        var items = listCurrentItems.Where(e => (ItemId)e.id == itemId).ToList();
                        var neededSlot = dictCountSlotByGrill.GetValueOrDefault(primaryGrill.id, 0);
                        dictListItemStepInfo[itemId].Add((items, neededSlot));
                    }
                }
            }

            var listIgnoreItems = new List<Item>();
            foreach (var itemId in neededItemsForCurrentOrder.Keys)
            {
                if (dictListItemStepInfo.ContainsKey(itemId))
                {
                    var neededItems = neededItemsForCurrentOrder[itemId];
                    listIgnoreItems.AddRange(ChooseItemToIgnore(dictListItemStepInfo[itemId], neededItems));
                }
            }

            // duyệt lần 2 để tính toán
            foreach (var primaryGrill in grillManager.ListGrills)
            {
                if (primaryGrill.IsLock) continue;
                var subGrills = primaryGrill.GetSubGrills();
                if (subGrills == null || subGrills.Count == 0) continue;
                var slots = subGrills[0].GetSlots();
                if (slots == null) continue;

                var listCurrentItems = slots.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
                var remainItems = listCurrentItems.Where(e => listIgnoreItems.Contains(e) == false).ToList();
                var dictItems = remainItems.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());

                foreach (var itemId in dictItems.Keys)
                {
                    if (ItemHelper.IsItemSpecial(itemId)) continue;
                    var numItems = dictItems[itemId];
                    var currentNeededSlot = dictCountSlotByGrill.GetValueOrDefault(primaryGrill.id, 0);

                    // nếu ở layer bên trên có item này thì trừ đi số lượng itme ở layer trên đó
                    var numItemsInUpperLayer = dictNumItemsInUpperLayer.GetValueOrDefault(primaryGrill.id, new List<Item>()).Where(e => (ItemId)e.id == itemId).Count();
                    currentNeededSlot = currentNeededSlot - numItemsInUpperLayer;

                    if (dp.ContainsKey(itemId) == false)
                    {
                        dp[itemId] = new Dictionary<int, int>();
                        for (int j = 1; j <= Mathf.Min(numItems, 3); j++)
                        {
                            dp[itemId].TryAdd(j, currentNeededSlot);
                        }
                    }
                    else
                    {
                        var numInDp = dp[itemId].Count;
                        for (int j = numInDp; j >= 1; j--)
                        {
                            var currentValue = dp[itemId][j] + currentNeededSlot;
                            for (int k = 1; k <= numItems; k++)
                            {
                                if (j + k > 3) continue;
                                dp[itemId].TryAdd(j + k, currentValue);
                                dp[itemId][j + k] = Mathf.Min(dp[itemId][j + k], currentValue);
                            }
                        }
                    }
                }
            }
        }


        private List<Item> ChooseItemToIgnore(List<(List<Item>, int)> list, int neededItems)
        {
            // dp[i] = (số item đạt được, chi phí, danh sách item đã chọn)
            var dp = new (int maxItems, int minCost, List<Item> chosen)[neededItems + 1];

            for (int i = 0; i <= neededItems; i++)
            {
                dp[i].maxItems = 0;
                dp[i].minCost = int.MaxValue;
                dp[i].chosen = new List<Item>();
            }

            dp[0].minCost = 0;

            foreach (var (items, cost) in list)
            {
                int itemCount = items.Count;

                // duyệt ngược để không overwrite trạng thái cũ
                for (int cap = neededItems; cap >= 0; cap--)
                {
                    if (dp[cap].minCost == int.MaxValue) continue;

                    int newCap = cap + itemCount;
                    if (newCap > neededItems) newCap = neededItems;

                    int newItems = dp[cap].maxItems + itemCount;
                    int newCost = dp[cap].minCost + cost;

                    bool shouldUpdate = false;

                    if (newItems > dp[newCap].maxItems) shouldUpdate = true;
                    else if (newItems == dp[newCap].maxItems && newCost < dp[newCap].minCost) shouldUpdate = true;

                    if (shouldUpdate)
                    {
                        dp[newCap].maxItems = newItems;
                        dp[newCap].minCost = newCost;
                        dp[newCap].chosen = dp[cap].chosen.Concat(items).ToList();
                    }
                }
            }

            // tìm trạng thái tốt nhất
            var best = dp[0];
            for (int i = 1; i <= neededItems; i++)
            {
                if (dp[i].maxItems > best.maxItems ||
                    (dp[i].maxItems == best.maxItems && dp[i].minCost < best.minCost))
                {
                    best = dp[i];
                }
            }

            return best.chosen;
        }
    }
}