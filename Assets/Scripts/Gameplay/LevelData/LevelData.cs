using System;
using System.Collections.Generic;
using Manager;
using Newtonsoft.Json;
using Sonat.Enums;
using UnityEngine;

namespace Gameplay.LevelData
{
    public class LevelData : SonatFramework.Systems.LevelManagement.LevelData
    {
        public LevelDifficulty difficulty = LevelDifficulty.Normal;
        public LevelType levelType = LevelType.Food;
        public ushort time;
        public List<GrillData> grillData;
        public List<ConveyorData> conveyorData;
        public List<OrderData> orderData;
        public bool isDropMode;
        public List<ObstacleData> obstacleData;

        public virtual LevelData Clone()
        {
            LevelData levelData = new LevelData();
            levelData.time = this.time;
            levelData.difficulty = this.difficulty;
            levelData.levelType = this.levelType;
            levelData.isDropMode = this.isDropMode;
            levelData.grillData = new List<GrillData>(grillData);
            if (conveyorData != null)
                levelData.conveyorData = new List<ConveyorData>(conveyorData);
            // if (orderData != null && !GameRemoteConfigValue.noOrder)
            //     levelData.orderData = new List<OrderData>(orderData);
            if (obstacleData != null)
                levelData.obstacleData = new List<ObstacleData>(obstacleData);
            return levelData;
        }

        public void LoopLayer(int multiple)
        {
            if (multiple <= 0) return;
            List<LayerData> layers = new List<LayerData>();

            for (int i = 0; i < multiple; i++)
            {
                foreach (var data in grillData)
                {
                    layers.AddRange(data.layer);
                }
            }

            layers.Shuffle();
            int grillIndex = 0;
            foreach (var layer in layers)
            {
                grillData[grillIndex].layer.Add(layer);
                grillIndex++;
                if (grillIndex >= grillData.Count) grillIndex = 0;
            }
        }
    }

    [Serializable]
    public class GrillData
    {
        public byte id;
        public Vector3Data position;
        public List<LayerData> layer;
        public GrillType grillType;
        public bool isLock;
        public int slotCount;

        [JsonIgnore]
        public int SlotCount
        {
            get
            {
                if (slotCount > 0) return slotCount;
                switch ((byte)grillType)
                {
                    case 0:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 10:
                        slotCount = 3;
                        return 3;
                    case 1:
                        grillType = GrillType.Normal;
                        slotCount = 1;
                        return 1;
                    case 9:
                    case 11:
                        slotCount = 1;
                        return 1;
                    case 3:
                        slotCount = 7;
                        grillType = GrillType.Normal;
                        return 7;
                    default:
                        slotCount = 3;
                        return 3;
                }
            }
        }

        public bool ValidateData()
        {
            if (layer == null || layer.Count == 0) return false;
            for (int i = layer.Count - 1; i >= 0; i--)
            {
                if (!layer[i].ValidateData())
                {
                    layer.RemoveAt(i);
                }
            }

            return layer.Count > 0;
        }
    }

    public class GrillLidData : GrillData
    {
        public int itemCondition;
    }

    public class GrillIceData : GrillData
    {
        public int priority;
    }

    public class GrillLockData : GrillData
    {
        public int priority;
    }

    public class GrillSpicyData : GrillData
    {
    }

    public class GrillBrokenData : GrillData
    {
    }

    [SelectionBase]
    public class LayerData
    {
        public ItemData[] itemData;

        public LayerData(int numberSlot)
        {
            itemData = new ItemData[numberSlot];
        }

        public bool ValidateData()
        {
            for (int i = 0; i < itemData.Length; i++)
            {
                if (itemData[i] != null && itemData[i].id > 0) return true;
            }

            return false;
        }
    }

    [Serializable]
    public class ItemData
    {
        public int id;
        public bool hidden;
        public ItemType itemType;

        public virtual ItemData Clone()
        {
            return new ItemData()
            {
                id = id,
                hidden = hidden,
                itemType = itemType,
            };
        }
    }

    public class ItemBombData : ItemData
    {
        public int moveLimit = 10;

        public override ItemData Clone()
        {
            return new ItemBombData()
            {
                id = id,
                hidden = hidden,
                itemType = itemType,
                moveLimit = moveLimit,
            };
        }
    }

    public class ItemKeyAreaData : ItemData
    {
    }

    public class ItemCoinData : ItemData
    {
        public int coin = 1;

        public override ItemData Clone()
        {
            return new ItemCoinData()
            {
                id = id,
                hidden = hidden,
                itemType = itemType,
                coin = coin,
            };
        }
    }

    public class Vector3Data
    {
        public float x;
        public float y;
        public float z;

        public Vector3Data(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public enum ObstacleType : byte
    {
        None = 0,
        Lock = 1,
        Hidden = 2,
        OctoChef = 3,
        LockAreaHorizontal = 4,
        LockAreaVertical = 5,
    }

    public enum GrillType : byte
    {
        Normal = 0,

        //Single = 1,
        //Drop = 2,
        //Drop7 = 3,
        Lock = 4,
        LockAds = 5,
        LockAndKey = 6,
        Ice = 7,
        Lid = 8,
        Vending = 9,
        LockAndKey2 = 10,

        // Drop7Lock,
        // Drop7LockAds,
        // Drop7LockAndKey,
        // Drop7LockAndKey2,
        // Drop7Ice,
        // Drop7Lid,
        // Drop5,
        // Drop6,
        SingleMin = 11,
        Spicy = 12,
        Broken = 13,
        Simple = 14
    }

    public enum ItemType : byte
    {
        Normal = 0,
        Hidden,
        Special,
        Bomb,
        Ice,
        Key,
        Key2,
        KeyArea,
        Coin
    }

    public enum MoveType : byte
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Drop = 3
    }

    public enum ConveyorType : byte
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        HorizontalMin = 3,
        HorizontalSimple = 4
    }

    public class ConveyorData
    {
        public byte id;
        public ConveyorType conveyorType;
        public MoveType moveType = MoveType.Horizontal;
        public float speed;
        public Vector3Data position;
        public List<int> grillIds = new();
        public float space = 0.75f;
    }

    public class LayerMultipleData
    {
        public int[] multiple;
    }

    public class OrderData
    {
        public int time;
        //public OrderAppearType appearType;
        public int appearCondition;
        //public int reward;
        public List<int> ids = new();
        public List<int> order = new();
        public List<OrderItemData> orderItem;

        [JsonIgnore]
        public List<OrderItemData> OrderItem
        {
            get
            {
                if (orderItem == null)
                {
                    orderItem = new List<OrderItemData>();
                    if (ids.Count > 0)
                    {
                        foreach (var id in ids)
                        {
                            OrderItemData oData = new OrderItemData()
                            {
                                id = id,
                            };
                            orderItem.Add(oData);
                        }
                    }
                    else if (order.Count > 0)
                    {
                        foreach (var layer in order)
                        {
                            OrderItemData oData = new OrderItemData()
                            {
                                layer = layer
                            };
                            orderItem.Add(oData);
                        }
                    }
                }

                return orderItem;
            }
        }
    }

    public class OrderItemData
    {
        public int id;
        public int layer;
        public bool spicy;
    }

    public enum OrderAppearType : byte
    {
        Sorting = 0,
        Ordering = 1,
        Time = 3
    }

    public class ObstacleData
    {
        public byte id;
        public ObstacleType obstacleType;
        public Vector3Data position;
        public List<int> grillIds = new();
    }

    // [Serializable]
    // public class GrillObstacleData : ObstacleData
    // {
    //     public int grillId;
    // }

    // public class LockAreaObstacleData : GrillObstacleData
    // {
    //     
    // }
}