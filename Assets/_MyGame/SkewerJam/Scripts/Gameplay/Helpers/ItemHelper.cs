using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Gameplay.LevelData;
using Manager;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public static class ItemHelper
    {

        public static bool IsItemSpecial(ItemId itemId)
        {
            return itemId == ItemId.Item_Special_QuestEvent || itemId == ItemId.Item_special_2 || itemId == ItemId.Item_Special_Coin_1 || itemId == ItemId.Item_Special_Coin_2;
        }

        public static Dictionary<ItemId, int> GetItemIdDictInGameplay(int numLayer, bool ignoreLock = false)
        {
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;

            var listItemIds = GrillHelper.GetItemIdListWithLayer(numLayer, ignoreLock);
            foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
            {
                var slot = waitingGrill.GetSlots()[0];
                var item = slot.GetItem();
                if (item != null)
                {
                    if (IsItemSpecial((ItemId)item.id)) continue;
                    listItemIds.Add((ItemId)item.id);
                }
            }

            return listItemIds.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());
        }

        public static List<ItemId> GetItemIdsInLockedGrillInLayer1()
        {
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var listItemIds = new List<ItemId>();
            foreach (var grill in grillManager.ListGrills)
            {
                if (grill.IsLock)
                {
                    foreach (var slot in grill.GetSlots())
                    {
                        var item = slot.GetItem();
                        if (item != null)
                        {
                            if (IsItemSpecial((ItemId)item.id)) continue;
                            listItemIds.Add((ItemId)item.id);
                        }
                    }
                }
                else
                {
                    foreach (var slot in grill.GetSlots())
                    {
                        var item = slot.GetItem();
                        if (item != null && item.IsLocked)
                        {
                            if (IsItemSpecial((ItemId)item.id)) continue;
                            listItemIds.Add((ItemId)item.id);
                        }
                    }
                }
            }
            return listItemIds;
        }

        public static List<ItemId> GetItemIdsInBlindedGrill()
        {
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var listItemIds = new List<ItemId>();
            foreach (var grill in grillManager.ListGrills)
            {
                if (grill.grillType == GrillType.Vending)
                {
                    var subGrills = grill.GetSubGrills();
                    if (subGrills != null && subGrills.Count > 0)
                    {
                        var firstSubGrill = subGrills[0];
                        foreach (var slot in firstSubGrill.GetSlots())
                        {
                            var item = slot.GetItem();
                            if (item != null)
                            {
                                if (IsItemSpecial((ItemId)item.id)) continue;
                                listItemIds.Add((ItemId)item.id);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var slot in grill.GetSlots())
                    {
                        var item = slot.GetItem();
                        if (item != null && item.itemType == ItemType.Hidden)
                        {
                            if (IsItemSpecial((ItemId)item.id)) continue;
                            listItemIds.Add((ItemId)item.id);
                        }
                    }
                }
            }
            return listItemIds;
        }

        public static bool CheckSelectedItemOnOrder(Item item)
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var orderItemsDict = orderManager.GetOrderItemsDict();
            return orderItemsDict.ContainsKey((ItemId)item.id);
        }

        public static List<Item> SelectItemsForForceBooster(int numItems)
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrder = orderManager.GetOrderItemsDict();

            // lấy item từ theo layer từ 0 -> ...
            var listSelectedItems = new List<Item>();
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var listGrills = grillManager.ListGrills;

            foreach (var grill in listGrills)
            {
                if (grill.IsLock) continue;

                foreach (var slot in grill.GetSlots())
                {
                    var item = slot.GetItem();
                    if (item == null) continue;
                    if (item.IsLocked) continue;

                    if (dictOrder.ContainsKey((ItemId)item.id) || IsItemSpecial((ItemId)item.id)) continue;

                    listSelectedItems.Add(item);
                }
            }

            //lấy random 4 item
            var random = new System.Random();
            var randomItems = listSelectedItems.OrderBy(x => random.Next()).Take(numItems).ToList();
            return randomItems;
        }

        public static bool IsLockType(ItemType itemType)
        {
            return itemType == ItemType.Ice;
        }

        public static bool CanClearOnePlate(Item item)
        {
            return item.Data.itemType != ItemType.Key && item.Data.itemType != ItemType.Key2 && item.Data.itemType != ItemType.KeyArea;
        }
    }
}