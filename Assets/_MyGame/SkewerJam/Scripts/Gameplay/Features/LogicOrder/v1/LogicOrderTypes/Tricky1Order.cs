using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "Tricky1OrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Tricky1OrderSO")]
    public class Tricky1OrderSO : BaseOrderSO
    {
        public override LogicOrderType LogicOrderType => LogicOrderType.Tricky1;
        public override void Init()
        {

        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            // gọi 1 order chứa 1 item ở layer 3
            var dictItemIdsIn2Layer = ItemHelper.GetItemIdDictInGameplay(2, true);
            var grills = GameController.Instance.GameLogicHandler.GrillManager.ListGrills;

            var listItemOnlyLayer3 = new List<ItemId>();
            foreach (var grill in grills)
            {
                if (grill is PrimaryVendingGrill) continue; // tricky order né vending

                var layer3Data = grill.GetLayerData(2);
                if (layer3Data != null && layer3Data.itemData != null && layer3Data.itemData.Length > 0)
                {
                    foreach (var itemData in layer3Data.itemData)
                    {
                        if (itemData != null && itemData.id > 0 && dictItemIdsIn2Layer.ContainsKey((ItemId)itemData.id) == false)
                        {
                            listItemOnlyLayer3.Add((ItemId)itemData.id);
                        }
                    }
                }
            }

            // loại các item đã order
            var dict = listItemOnlyLayer3.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrder = orderManager.GetOrderItemsDict();
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
            var listFilteredItemIds = dict.Keys.ToList();
            return (listFilteredItemIds[Random.Range(0, listFilteredItemIds.Count)], 1);
        }
    }
}