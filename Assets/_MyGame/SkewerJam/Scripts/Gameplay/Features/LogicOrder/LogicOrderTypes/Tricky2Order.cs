using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "Tricky2OrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Tricky2OrderSO")]
    public class Tricky2OrderSO : BaseOrderSO
    {
        public override LogicOrderType LogicOrderType => LogicOrderType.Tricky2;
        public override void Init()
        {

        }

        public override void SetLevelData(LevelData_SkewerJam levelData)
        {
        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            // gọi toàn item ở layer 2
            var dictItemIdsIn2Layer = ItemHelper.GetItemIdDictInGameplay(1, true);
            var grills = GameController.Instance.GameLogicHandler.GrillManager.ListGrills;

            var listItemOnlyLayer2 = new List<ItemId>();
            foreach (var grill in grills)
            {
                if (grill is PrimaryVendingGrill) continue; // tricky order né vending
                var layer2Data = grill.GetLayerData(1);
                if (layer2Data != null && layer2Data.itemData != null && layer2Data.itemData.Length > 0)
                {
                    foreach (var itemData in layer2Data.itemData)
                    {
                        if (itemData != null && itemData.id > 0 && dictItemIdsIn2Layer.ContainsKey((ItemId)itemData.id) == false)
                        {
                            listItemOnlyLayer2.Add((ItemId)itemData.id);
                        }
                    }
                }
            }

            // loại các item đã order
            if (listItemOnlyLayer2.Count == 0) return (ItemId.None, 0);

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrder = orderManager.GetOrderItemsDict();

            var dict = listItemOnlyLayer2.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());
            foreach (var itemId in dictOrder.Keys)
            {
                if (dict.ContainsKey(itemId))
                {
                    dict[itemId] -= (dictOrder[itemId].maxItems - dictOrder[itemId].num);
                    if (dict[itemId] <= 0)
                    {
                        dict.Remove(itemId);
                    }
                }
            }
            if (dict.Count == 0) return (ItemId.None, 0);

            // random item có số lượng lớn nhất
            var maxNum = dict.Values.Max();
            var listFilteredItemIds = dict.Keys.Where(e => dict[e] == maxNum).ToList();
            return (listFilteredItemIds[Random.Range(0, listFilteredItemIds.Count)], maxNum);
        }
    }
}