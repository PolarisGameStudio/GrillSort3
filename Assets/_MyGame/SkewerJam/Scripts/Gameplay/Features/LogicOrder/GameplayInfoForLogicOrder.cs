using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Gameplay.LevelData;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public class GameplayInfoForLogicOrder
    {
        private int _maxDepth;
        private Dictionary<ItemId, Dictionary<int, int>> _dictDeltaSlots;

        public Dictionary<ItemId, Dictionary<int, int>> DictDeltaSlots => _dictDeltaSlots;


        private Dictionary<string, int> _dictTempData = new Dictionary<string, int>();
        public Dictionary<string, int> DictTempData => _dictTempData;

        public int GetTempData(string key)
        {
            return _dictTempData[key];
        }

        public void SetTempData(string key, int value)
        {
            _dictTempData[key] = value;
        }

        public void UpdateState(int maxDepth)
        {
            // đã loại các item và grill bị lock
            _maxDepth = maxDepth;
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

            // duyệt qua grill;
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            foreach (var primaryGrill in grillManager.ListGrills)
            {
                CalculateInGrill(primaryGrill, neededItemsForCurrentOrder, ref dictNode, ref countId);
            }

            return dictNode;
        }

        private void CalculateInGrill(PrimaryGrill primaryGrill, Dictionary<ItemId, int> neededItemsForCurrentOrder, ref Dictionary<ItemId, List<NodeAsOneLayerInGrill>> dictNode, ref int countId)
        {
            if (primaryGrill.IsLock) return;

            var currentRootId = countId;
            var listItemInUpperLayer = new List<ItemId>();
            for (int i = 0; i < _maxDepth; i++)
            {
                var layerData = GetLayerData(primaryGrill, i);
                if (layerData == null) return;

                var filteredLayerData = layerData.itemData.Where(e => e != null && e.id != 0 && ItemHelper.IsLockType(e.itemType) == false && ItemHelper.IsItemSpecial((ItemId)e.id) == false).ToList();
                var dictItemsInLayer = filteredLayerData.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());
                var listKeysInLayer = dictItemsInLayer.Keys.ToList();
                foreach (var itemId in listKeysInLayer)
                {
                    if (dictNode.ContainsKey(itemId) == false)
                    {
                        dictNode[itemId] = new List<NodeAsOneLayerInGrill>();
                    }

                    // tính score:
                    var score = dictItemsInLayer[itemId];
                    var cost = 0;
                    var listCostOrder = new List<int>();
                    foreach (var it in listItemInUpperLayer)
                    {
                        if (it == itemId)
                        {
                            score += 1;
                        }
                        else
                        {
                            if (neededItemsForCurrentOrder.ContainsKey(it) && neededItemsForCurrentOrder[it] > listCostOrder.Count)
                            {
                                listCostOrder.Add((int)it);
                            }
                            else
                            {
                                cost += 1;
                            }
                        }
                    }
                    var nodeId = countId++;
                    dictNode[itemId].Add(new NodeAsOneLayerInGrill()
                    {
                        id = nodeId,
                        rootId = currentRootId,
                        score = score,
                        cost = cost,
                        listCostOrder = listCostOrder
                    });

                }
                // prepare data for next layer:
                listItemInUpperLayer.AddRange(filteredLayerData.Select(e => (ItemId)e.id));
            }

        }

        private LayerData GetLayerData(PrimaryGrill primaryGrill, int layer)
        {
            if (layer == 0)
            {
                return primaryGrill.GetCurrentData();
            }
            else
            {
                var subGrills = primaryGrill.GetSubGrills();
                if (subGrills == null || subGrills.Count <= layer - 1) return null;
                var selectedSubGrill = subGrills[layer - 1];
                return selectedSubGrill.GetCurrentData();
            }
        }
    }
}