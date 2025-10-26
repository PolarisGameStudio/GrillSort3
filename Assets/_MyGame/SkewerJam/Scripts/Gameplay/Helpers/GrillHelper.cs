using System;
using System.Collections.Generic;
using Gameplay.Entities;
using Gameplay.LevelData;
using Manager;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public static class GrillHelper
    {
        public static GrillType ValidateGrillType(this GrillType grillType)
        {
            if (Enum.IsDefined(typeof(GrillType), (byte)grillType)) return grillType;
            switch ((int)grillType)
            {
                case 11:
                    return GrillType.Lock;
                case 12:
                    return GrillType.LockAds;
                case 13:
                    return GrillType.LockAndKey;
                case 14:
                    return GrillType.LockAndKey2;
                case 15:
                    return GrillType.Ice;
                case 16:
                    return GrillType.Lid;
                default:
                    return GrillType.Normal;

            }
        }

        public static List<ItemId> GetItemIdListWithLayer(int numLayer, bool ignoreLock = false)
        {
            var listGrills = GameController.Instance.GameLogicHandler.GrillManager.ListGrills;
            var listItemIds = new List<ItemId>();

            foreach (var primaryGrill in listGrills)
            {
                if (ignoreLock == true && primaryGrill.IsLock) continue;

                var shuffleLayerData = primaryGrill.GetShuffleLayerData();
                if (shuffleLayerData?.layerData?.itemData == null) continue;
                foreach (var itemData in shuffleLayerData.layerData.itemData)
                {
                    if (itemData is not { id: > 0 }) continue;
                    listItemIds.Add((ItemId)itemData.id);
                }

                var subLayerData = primaryGrill.GetSubsShuffleLayerData();
                if (subLayerData?.Count == 0) continue;

                int currentLayer = 2;
                foreach (var subLayer in subLayerData)
                {
                    if (numLayer != -1 && currentLayer > numLayer) break;

                    currentLayer++;
                    if (subLayer?.layerData?.itemData == null) continue;
                    foreach (var itemData in subLayer.layerData.itemData)
                    {
                        if (itemData is not { id: > 0 }) continue;
                        listItemIds.Add((ItemId)itemData.id);
                    }
                }
            }
            return listItemIds;
        }

        public static Dictionary<int, int> GetGrillCountOrderItems()
        {
            // Đếm số order item trong các grill
            var listGrills = GameController.Instance.GameLogicHandler.GrillManager.ListGrills;
            var targetItemIds = GameController.Instance.GameLogicHandler.OrderManager.GetTargetItemIds();

            var dict = new Dictionary<int, int>();
            foreach (var primaryGrill in listGrills)
            {
                dict.TryAdd(primaryGrill.id, 0);
                foreach (var slot in primaryGrill.GetSlots())
                {
                    var item = slot.GetItem();
                    if (item != null && targetItemIds.Contains((ItemId)item.id))
                    {
                        dict[primaryGrill.id]++;
                    }
                }

                var subGrills = primaryGrill.GetSubGrills();
                if (subGrills == null || subGrills.Count == 0) continue;
                foreach (var slotInSubGrill in subGrills[0].GetSlots())
                {
                    var item = slotInSubGrill.GetItem();
                    if (item != null && targetItemIds.Contains((ItemId)item.id))
                    {
                        dict[primaryGrill.id]++;
                    }
                }
            }

            return dict;
        }

        public static bool CanShuffle(PrimaryGrill primaryGrill)
        {
            return primaryGrill.CanShuffle();
        }
    }
}