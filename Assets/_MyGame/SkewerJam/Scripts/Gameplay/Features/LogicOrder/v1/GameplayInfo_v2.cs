using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public class GameplayInfoForLogicOrder
    {
        public int phase;

        private Dictionary<ItemId, Dictionary<int, int>> _dictDeltaSlots;

        public Dictionary<ItemId, Dictionary<int, int>> DictDeltaSlots => _dictDeltaSlots;

        public void UpdateState()
        {
            // đã loại các item và grill bị lock
            _dictDeltaSlots = GetDictDeltaSlots();
        }

        private Dictionary<ItemId, Dictionary<int, int>> GetDictDeltaSlots()
        {
            // dynamic programming:
            // prepare chuẩn bị dữ liệu:
            // tính xem mỗi node: (score/ cost)
            var targetDp = new Dictionary<ItemId, Dictionary<int, int>>(); // itemId/numItems/deltaSlot (cost)

            var neededItemsForCurrentOrder = OrderHelper.GetNeededItemsForCurrentOrder();
            var dictPrepareData = GetPrepareData(neededItemsForCurrentOrder);

            var listKeys = dictPrepareData.Keys.ToList();
            foreach (var itemId in listKeys)
            {
                var listNodes = dictPrepareData[itemId];

                var subDp = new Dictionary<int, Node>(); // score/(cost, costOrder): tối ưu min cost và costOrder thỏa mãn điều kiện
                var maxScore = 3 + (neededItemsForCurrentOrder.ContainsKey(itemId) ? neededItemsForCurrentOrder[itemId] : 0);

                // init score 0:
                subDp[0] = new Node() { totalCost = 0, listRootIds = new List<int>() };

                for (int curScore = 1; curScore <= maxScore; curScore++)
                {
                    var checkCreate = false;
                    subDp[curScore] = new Node() { totalCost = int.MaxValue, listRootIds = new List<int>() };
                    foreach (var curNode in listNodes)
                    {
                        var score = curNode.score;
                        var cost = curNode.cost;

                        for (int s = 1; s <= score; s++)
                        {
                            if (subDp.ContainsKey(curScore - s) == false) continue;
                            if (subDp[curScore - s].listRootIds.Contains(curNode.rootId) == true) continue;

                            // không chọn những node ở cùng 1 nhánh
                            // kiểm tra xem node hiện tại (curNode) không đc nằm trong nhánh vào trong listSubNodes
                            var listSubNodes = subDp[curScore].listRootIds;

                            var newNode = new Node()
                            {
                                totalCost = cost,
                                listRootIds = new List<int>() { curNode.rootId },
                                dictCostOrder = new Dictionary<int, List<int>>() { { curNode.rootId, curNode.listCostOrder } }
                            };

                            if (curScore - s >= 1)
                            {
                                newNode.listRootIds.AddRange(subDp[curScore - s].listRootIds);

                                // update dictCostOrder:
                                foreach (var rootId in subDp[curScore - s].listRootIds)
                                {
                                    newNode.dictCostOrder[rootId] = subDp[curScore - s].dictCostOrder[rootId];
                                }

                                // update totalCost:
                                var listCostOrder = newNode.dictCostOrder.Values.SelectMany(e => e).ToList();
                                var dictCostOrder = listCostOrder.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());
                                var diffCostOrder = 0;
                                foreach (var (i, numItems) in neededItemsForCurrentOrder)
                                {
                                    if (dictCostOrder.ContainsKey((int)i) == false) continue;
                                    diffCostOrder += Mathf.Max(0, dictCostOrder[(int)i] - numItems);
                                }
                                newNode.totalCost = subDp[curScore - s].totalCost + cost + diffCostOrder;
                            }

                            if (subDp[curScore] > newNode)
                            {
                                checkCreate = true;
                                subDp[curScore] = newNode;
                            }
                        }
                    }

                    if (checkCreate == false)
                    {
                        subDp.Remove(curScore);
                    }
                    else
                    {
                        var realScore = CheckAddToTargetDp(itemId, neededItemsForCurrentOrder, curScore);
                        if (realScore <= 0) continue;
                        if (targetDp.ContainsKey(itemId) == false)
                        {
                            targetDp[itemId] = new Dictionary<int, int>();
                        }


                        targetDp[itemId][realScore] = subDp[curScore].totalCost;
                    }
                }
            }
            return targetDp;

        }

        private int CheckAddToTargetDp(ItemId itemId, Dictionary<ItemId, int> neededItemsForCurrentOrder, int score)
        {
            if (neededItemsForCurrentOrder.ContainsKey(itemId) == false) return score;
            return score - neededItemsForCurrentOrder[itemId];
        }

        private Dictionary<ItemId, List<NodeAsOneLayerInGrill>> GetPrepareData(Dictionary<ItemId, int> neededItemsForCurrentOrder)
        {
            var dictNode = new Dictionary<ItemId, List<NodeAsOneLayerInGrill>>(); // itemId/listNode

            var countId = 0;
            // duyệt qua waiting grill;
            var listWaitingItems = OrderHelper.GetItemsInWaitingGrill();
            foreach (var item in listWaitingItems)
            {
                if (dictNode.ContainsKey((ItemId)item.id) == false)
                {
                    dictNode[(ItemId)item.id] = new List<NodeAsOneLayerInGrill>();
                }

                var nodeId = countId++;
                dictNode[(ItemId)item.id].Add(new NodeAsOneLayerInGrill() { id = nodeId, rootId = nodeId, score = 1, cost = -1, listCostOrder = new List<int>() });

            }

            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            foreach (var primaryGrill in grillManager.ListGrills)
            {
                if (primaryGrill.IsLock) continue;

                // duyệt qua layer 0:
                var slots = primaryGrill.GetSlots();
                if (slots == null) continue;

                var currentRootId = countId;
                var listItemsInLayer0 = slots.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
                var dictItemsInLayer0 = listItemsInLayer0.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());
                var listKeysInLayer0 = dictItemsInLayer0.Keys.ToList();
                foreach (var itemId in listKeysInLayer0)
                {
                    if (ItemHelper.IsItemSpecial(itemId)) continue;
                    if (dictNode.ContainsKey(itemId) == false)
                    {
                        dictNode[itemId] = new List<NodeAsOneLayerInGrill>();
                    }

                    var numItems = dictItemsInLayer0[itemId];
                    var nodeId = countId++;
                    dictNode[itemId].Add(new NodeAsOneLayerInGrill() { id = nodeId, rootId = currentRootId, score = numItems, cost = 0, listCostOrder = new List<int>() });
                }

                // duyệt qua layer 1:
                var subGrills = primaryGrill.GetSubGrills();
                if (subGrills == null || subGrills.Count == 0) continue;
                var slotsInLayer1 = subGrills[0].GetSlots();
                if (slotsInLayer1 == null) continue;

                var listItemsInLayer1 = slotsInLayer1.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
                var dictItemsInLayer1 = listItemsInLayer1.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());
                var listKeysInLayer1 = dictItemsInLayer1.Keys.ToList();
                foreach (var itemId in listKeysInLayer1)
                {
                    if (ItemHelper.IsItemSpecial(itemId)) continue;
                    if (dictNode.ContainsKey(itemId) == false)
                    {
                        dictNode[itemId] = new List<NodeAsOneLayerInGrill>();
                    }

                    // tính score:
                    var score = dictItemsInLayer1[itemId];
                    var cost = 0;
                    var listCostOrder = new List<int>();
                    foreach (var it in listItemsInLayer0)
                    {
                        if (ItemHelper.IsItemSpecial((ItemId)it.id)) continue;
                        if ((ItemId)it.id == itemId)
                        {
                            score += 1;
                        }
                        else
                        {
                            if (neededItemsForCurrentOrder.ContainsKey((ItemId)it.id) && neededItemsForCurrentOrder[(ItemId)it.id] > listCostOrder.Count)
                            {
                                listCostOrder.Add(it.id);
                            }
                            else
                            {
                                cost += 1;
                            }
                        }
                    }
                    var nodeId = countId++;
                    dictNode[itemId].Add(new NodeAsOneLayerInGrill() { id = nodeId, rootId = currentRootId, score = score, cost = cost, listCostOrder = listCostOrder });
                }
            }

            return dictNode;
        }
    }
}