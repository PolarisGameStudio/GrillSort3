// using System.Collections.Generic;
// using System.Linq;
// using Manager;

// namespace MyGame.SkewerJam.Gameplay.LogicOrder
// {
//     public class GameplayInfoForLogicOrder
//     {
//         public int phase;

//         private Dictionary<ItemId, Dictionary<int, int>> _dictDeltaSlots;

//         public Dictionary<ItemId, Dictionary<int, int>> DictDeltaSlots => _dictDeltaSlots;

//         public void UpdateState()
//         {
//             // đã loại các item và grill bị lock
//             _dictDeltaSlots = GetDictDeltaSlots();
//         }

//         private Dictionary<ItemId, Dictionary<int, int>> GetDictDeltaSlots()
//         {
//             // // dynamic programming
//             // // neededSlot = số slot cần để ăn được order (numItems của itemId)
//             // // deltaSlot = lượng slot thay đổi để có thể ăn được order (numItems của itemId)
//             // var dp = new Dictionary<ItemId, Dictionary<int, int>>(); // itemId/numItems/neededSlot

//             // // Tính toán order: Sẽ luôn chọn item tối ưu giúp clear order
//             // var neededItemsForCurrentOrder = OrderHelper.GetNeededItemsForCurrentOrder();
//             // CalculateOnWaitingGrill(ref dp, ref neededItemsForCurrentOrder);
//             // CalculateOnGrill(ref dp, ref neededItemsForCurrentOrder);
//             // return dp;

//             // prepare chuẩn bị dữ liệu:
//             // tính xem mỗi node: (score/ cost)
//             var targetDp = new Dictionary<ItemId, Dictionary<int, int>>(); // itemId/numItems/deltaSlot (cost)

//             var neededItemsForCurrentOrder = OrderHelper.GetNeededItemsForCurrentOrder();
//             var dictPrepareData = GetPrepareData(neededItemsForCurrentOrder);

//             var listKeys = dictPrepareData.Keys.ToList();
//             foreach (var itemId in listKeys)
//             {
//                 var listNodes = dictPrepareData[itemId];

//                 var subDp = new Dictionary<int, Node>(); // score/(cost, costOrder): tối ưu min cost và costOrder thỏa mãn điều kiện
//                 for (int i = 1; i <= 3; i++)
//                 {
//                     var checkCreate = false;
//                     subDp[i] = new Node() { cost = int.MaxValue, costOrder = 0, listSubNodes = new List<int>() };
//                     foreach (var node in listNodes)
//                     {
//                         var score = node.score;
//                         var cost = node.cost;
//                         var costOrder = node.costOrder;

//                         // var newCostOrder = 0;
//                         for (int s = 1; s <= score; s++)
//                         {
//                             if (i - s >= 1 && subDp[i - s].listSubNodes.Contains(node.id) == true) continue;

//                             var newCost = cost;
//                             var newCostOrder = costOrder;
//                             var newListSubNodes = new List<int>() { node.id };
//                             if (i - s >= 1)
//                             {
//                                 newCost = subDp[i - s].cost + cost;
//                                 newCostOrder = subDp[i - s].costOrder + costOrder;
//                                 newListSubNodes.AddRange(subDp[i - s].listSubNodes);
//                             }

//                             if (newCostOrder > (neededItemsForCurrentOrder.ContainsKey(itemId) ? neededItemsForCurrentOrder[itemId] : 0)) continue;

//                             var newNode = new Node() { cost = newCost, costOrder = newCostOrder, listSubNodes = newListSubNodes };
//                             if (subDp[i] > newNode)
//                             {
//                                 checkCreate = true;
//                                 subDp[i] = newNode;
//                             }
//                         }
//                     }

//                     if (checkCreate)
//                     {
//                         if (targetDp.ContainsKey(itemId) == false)
//                         {
//                             targetDp[itemId] = new Dictionary<int, int>();
//                         }
//                         targetDp[itemId][i] = subDp[i].cost;
//                     }
//                 }
//             }

//             return targetDp;

//         }

//         private Dictionary<ItemId, List<SubNode>> GetPrepareData(Dictionary<ItemId, int> neededItemsForCurrentOrder)
//         {
//             var dictNode = new Dictionary<ItemId, List<SubNode>>(); // itemId/listNode

//             // duyệt qua waiting grill;
//             var listWaitingItems = OrderHelper.GetItemsInWaitingGrill();
//             foreach (var item in listWaitingItems)
//             {
//                 if (dictNode.ContainsKey((ItemId)item.id) == false)
//                 {
//                     dictNode[(ItemId)item.id] = new List<SubNode>();
//                 }

//                 var subNodeId = dictNode[(ItemId)item.id].Count;
//                 dictNode[(ItemId)item.id].Add(new SubNode() { id = subNodeId, score = 1, cost = -1, costOrder = 0 });
//             }

//             var grillManager = GameController.Instance.GameLogicHandler.GrillManager;

//             foreach (var primaryGrill in grillManager.ListGrills)
//             {
//                 if (primaryGrill.IsLock) continue;

//                 // duyệt qua layer 0:
//                 var slots = primaryGrill.GetSlots();
//                 if (slots == null) continue;
//                 var listItems = slots.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
//                 var dictItems = listItems.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());
//                 var listKeys = dictItems.Keys.ToList();
//                 foreach (var item in listKeys)
//                 {
//                     if (dictNode.ContainsKey(item) == false)
//                     {
//                         dictNode[item] = new List<SubNode>();
//                     }

//                     var numItems = dictItems[item];
//                     var subNodeId = dictNode[item].Count;
//                     dictNode[item].Add(new SubNode() { id = subNodeId, score = numItems, cost = 0, costOrder = 0 });
//                 }

//                 // duyệt qua layer 1:
//                 var subGrills = primaryGrill.GetSubGrills();
//                 if (subGrills == null || subGrills.Count == 0) continue;
//                 var slotsInLayer1 = subGrills[0].GetSlots();
//                 if (slotsInLayer1 == null) continue;

//                 var listItemsInLayer1 = slotsInLayer1.Select(e => e.GetItem()).Where(e => e != null && e.IsLocked == false).ToList();
//                 var dictItemsInLayer1 = listItemsInLayer1.GroupBy(e => (ItemId)e.id).ToDictionary(e => e.Key, e => e.Count());
//                 var listKeysInLayer1 = dictItemsInLayer1.Keys.ToList();
//                 foreach (var item in listKeysInLayer1)
//                 {
//                     if (dictNode.ContainsKey(item) == false)
//                     {
//                         dictNode[item] = new List<SubNode>();
//                     }

//                     // tính score:
//                     var score = dictItemsInLayer1[item];
//                     var cost = 0;
//                     var scoreOrder = 0;
//                     foreach (var it in listItems)
//                     {
//                         if (neededItemsForCurrentOrder.ContainsKey((ItemId)it.id) && neededItemsForCurrentOrder[(ItemId)it.id] > scoreOrder)
//                         {
//                             scoreOrder += 1;
//                         }
//                         else
//                         {
//                             if ((ItemId)it.id == item)
//                             {
//                                 score += 1;
//                             }
//                             else
//                             {
//                                 cost += 1;
//                             }
//                         }
//                     }
//                     var subNodeId = dictNode[item].Count;
//                     dictNode[item].Add(new SubNode() { id = subNodeId, score = score, cost = cost, costOrder = scoreOrder });
//                 }
//             }
//             return dictNode;
//         }
//     }
// }