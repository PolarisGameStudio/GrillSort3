using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyGame.SkewerJam.Gameplay;
using Gameplay.LevelData;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Gameplay.Entities;
using SonatFramework.Systems.SettingsManagement.Vibation;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Scripts.UIModule;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterShuffleBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterShuffleBehaviorSO")]
    public class BoosterShuffleBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterShuffle;

        [SerializeField] private float delay = 2f;

        #region Behavior
        public override async UniTask<bool> UseBooster(Vector3 position, bool isForce = false)
        {
            if (isForce == false)
            {
                await PlayBoosterAnim(position);
            }

            Debug.Log("<color=yellow>BoosterShuffleBehaviorSO: </color> UseBooster Shuffle");
            // shuffle cho tất cả các item order lên layer 1
            // tạo cảm giác shuffle layer 1 và 2
            // giữ nguyên layer còn lại chỉ swap với các item được đẩy lên layer 1

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrderItems = orderManager.GetOrderItemsDict();

            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var primaryGrills = grillManager.ListGrills;


            // swap orderitem lên layer 1
            var countItemInLayer0 = 0;
            var listItemDatasIn2Layer = new List<ItemData>(); // chứa item của 2 layer đầu
            var dictGrill2ListLayerData = new Dictionary<PrimaryGrill, List<LayerData>>();
            foreach (var primaryGrill in primaryGrills)
            {
                // primary layer
                if (primaryGrill.CanShuffle() == false) continue;
                listItemDatasIn2Layer.AddRange(primaryGrill.GetCurrentData().itemData);
                countItemInLayer0 += primaryGrill.GetCurrentData().itemData.Count(item => item != null);

                // sublayer 0
                if (primaryGrill.GetSubGrills() != null && primaryGrill.GetSubGrills().Count > 0)
                {
                    listItemDatasIn2Layer.AddRange(primaryGrill.GetSubGrills()[0].GetCurrentData().itemData);
                }
            }

            listItemDatasIn2Layer = listItemDatasIn2Layer.Where(item => item != null).ToList();
            var listItemInOrder = listItemDatasIn2Layer.Where(item => dictOrderItems.ContainsKey((ItemId)item.id)).ToList();
            var listItemNotInOrder = listItemDatasIn2Layer.Where(item => !dictOrderItems.ContainsKey((ItemId)item.id)).ToList();

            // tính số item in order cần ở những layer dưới
            var dictItemOrderDatas = dictOrderItems.ToDictionary(e => e.Key, e => e.Value.maxItems - e.Value.num);
            foreach (var item in listItemDatasIn2Layer)
            {
                if (dictItemOrderDatas.ContainsKey((ItemId)item.id))
                {
                    dictItemOrderDatas[(ItemId)item.id]--;
                    if (dictItemOrderDatas[(ItemId)item.id] <= 0)
                    {
                        dictItemOrderDatas.Remove((ItemId)item.id);
                    }
                }
            }

            // swap với các subgrill
            foreach (var primaryGrill in primaryGrills)
            {
                if (primaryGrill.CanShuffle() == false) continue;

                if (dictItemOrderDatas.Count == 0) break;

                if (primaryGrill.GetSubGrills() == null || primaryGrill.GetSubGrills().Count == 0) continue;

                var subGrills = primaryGrill.GetSubGrills();
                for (int i = 1; i < subGrills.Count; i++)
                {
                    var subGrill = subGrills[i];
                    var layerData = subGrill.GetCurrentData();
                    var checkSwap = layerData.itemData.Any(e =>
                        e != null
                        && dictItemOrderDatas.ContainsKey((ItemId)e.id)
                        && dictItemOrderDatas[(ItemId)e.id] > 0
                        );

                    if (checkSwap)
                    {
                        var newLayerData = new LayerData(layerData.itemData.Length);
                        for (int j = 0; j < layerData.itemData.Length; j++)
                        {
                            var item = layerData.itemData[j];
                            if (item == null)
                            {
                                newLayerData.itemData[j] = item;
                                continue;
                            }

                            if (dictItemOrderDatas.ContainsKey((ItemId)item.id))
                            {
                                dictItemOrderDatas[(ItemId)item.id]--;
                                if (dictItemOrderDatas[(ItemId)item.id] <= 0)
                                {
                                    dictItemOrderDatas.Remove((ItemId)item.id);
                                }


                                listItemInOrder.Add(item);

                                var randomItem = listItemNotInOrder[Random.Range(0, listItemNotInOrder.Count)];
                                newLayerData.itemData[j] = randomItem;
                                listItemNotInOrder.Remove(randomItem);
                            }
                            else
                            {
                                newLayerData.itemData[j] = item;
                            }
                        }
                        subGrill.SetShuffleLayerData(newLayerData);
                    }

                }
            }

            // TODO: -----------------------shuffle 2 layer đầu-----------------------------------------------------------------------------------
            listItemInOrder.Shuffle();
            listItemNotInOrder.Shuffle();
            var itemInLayer0 = new List<ItemData>(listItemInOrder);
            var randomItemNotInOrder = new List<ItemData>(listItemNotInOrder.Take(countItemInLayer0 - listItemInOrder.Count));
            listItemNotInOrder.RemoveRange(0, countItemInLayer0 - listItemInOrder.Count);
            itemInLayer0.AddRange(randomItemNotInOrder);

            foreach (var primaryGrill in primaryGrills)
            {
                if (primaryGrill.CanShuffle() == false) continue;

                var layerData = primaryGrill.GetCurrentData();
                var newLayerData = new LayerData(layerData.itemData.Length);
                for (int i = 0; i < layerData.itemData.Length; i++)
                {
                    var item = layerData.itemData[i];
                    if (item == null)
                    {
                        newLayerData.itemData[i] = null;
                        continue;
                    }

                    var randItem = itemInLayer0[Random.Range(0, itemInLayer0.Count)];
                    newLayerData.itemData[i] = randItem;
                    itemInLayer0.Remove(randItem);
                }
                primaryGrill.SetShuffleLayerData(newLayerData);
            }

            // shuffle các subgrill
            foreach (var primaryGrill in primaryGrills)
            {
                if (primaryGrill.CanShuffle() == false) continue;

                var subGrills = primaryGrill.GetSubGrills();
                if (subGrills == null || subGrills.Count == 0) continue;

                var subGrill = subGrills[0];
                var layerData = subGrill.GetCurrentData();
                var newLayerData = new LayerData(layerData.itemData.Length);
                for (int i = 0; i < layerData.itemData.Length; i++)
                {
                    var item = layerData.itemData[i];
                    if (item == null)
                    {
                        newLayerData.itemData[i] = null;
                        continue;
                    }

                    var randItem = listItemNotInOrder[Random.Range(0, listItemNotInOrder.Count)];
                    newLayerData.itemData[i] = randItem;
                    listItemNotInOrder.Remove(randItem);
                }
                subGrill.SetShuffleLayerData(newLayerData);
            }

            MySonatFramework.GetService<VibrationService>().Vibrate(50);
            await UniTask.Delay(2000);
            return true;
        }

        public override async UniTask<bool> ForceUseBooster()
        {
            return false;
        }

        protected override async UniTask PlayBoosterAnim(Vector3 position)
        {
            var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim>(
                "BoosterAnimShuffle",
                PanelManager.Instance.transform);
            // boosterAnim.SetBooster(boosterType);
            // boosterAnim.SetData(position);
            await UniTask.Delay((int)(delay * 1000));
        }
        #endregion

        public override bool CheckSuggest()
        {
            // chỉ gợi ý shuffle khi mà có grill có thể shuffle
            // suggest shuffle khi grill shuffle có chưa item trên order
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var filteredGrills = grillManager.ListGrills.Where(e => e.CanShuffle()).ToList();

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrderItems = orderManager.GetOrderItemsDict();

            foreach (var grill in filteredGrills)
            {
                var layerData = grill.GetCurrentData();
                foreach (var item in layerData.itemData)
                {
                    if (item != null && dictOrderItems.ContainsKey((ItemId)item.id))
                    {
                        var orderItem = dictOrderItems[(ItemId)item.id];
                        orderItem.num++;
                        dictOrderItems[(ItemId)item.id] = orderItem;

                        if (orderItem.num >= orderItem.maxItems)
                        {
                            return true;
                        }
                    }
                }

                var subGrills = grill.GetSubGrills();
                if (subGrills != null && subGrills.Count > 0)
                {
                    var subLayerData = subGrills[0].GetCurrentData();
                    foreach (var item in subLayerData.itemData)
                    {
                        if (item != null && dictOrderItems.ContainsKey((ItemId)item.id))
                        {
                            var orderItem = dictOrderItems[(ItemId)item.id];
                            orderItem.num++;
                            dictOrderItems[(ItemId)item.id] = orderItem;

                            if (orderItem.num >= orderItem.maxItems)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}