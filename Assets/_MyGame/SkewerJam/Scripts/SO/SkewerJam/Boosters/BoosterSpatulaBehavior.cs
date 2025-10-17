using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyGame.SkewerJam.Gameplay;
using System.Linq;
using Manager;
using System;
using System.Collections.Generic;
using Gameplay.Entities;
using MyGame.SkewerJam.Objects.Entities;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Scripts.UIModule;
using static MyGame.SkewerJam.Objects.Entities.OrderEntity;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterSpatulaBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterSpatulaBehaviorSO")]
    public class BoosterSpatulaBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterSpatula;

        [SerializeField] private float delayBetweenItems = 0.3f;
        [SerializeField] private float delay = 2f;

        public override async UniTask UseBooster(Vector3 position)
        {
            await PlayBoosterAnim(position);
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var listOrders = new List<OrderEntity>(orderManager.ListOrders);
            Debug.Log($"BoosterSpatulaBehaviorSO: UseBooster: {listOrders.Count}");
            foreach (var order in listOrders)
            {
                if (order.IsActive == false) continue;
                if (order.State == OrderEntityState.Complete) continue;

                await UniTask.WaitUntil(() => order.State == OrderEntityState.Ready);

                var targetItemId = order.ItemIdTarget;
                var maxItems = order.MaxItems;
                var currentItems = order.GetSlots().Count(e => e.GetItem() != null);

                // tìm số item còn lại để nhảy vào
                var selectedItems = await FindItems(targetItemId, maxItems - currentItems);

                var gameLogicHandler = GameController.Instance.GameLogicHandler;
                foreach (var item in selectedItems)
                {
                    gameLogicHandler.SelectItem(item);
                    await UniTask.Delay((int)(delayBetweenItems * 1000));
                }
                return;
            }
        }

        private async UniTask<List<Item>> FindItems(ItemId targetItemId, int num)
        {
            // lấy item từ theo layer từ 0 -> ...
            var listItems = new List<Item>();
            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var listGrills = grillManager.ListGrills;

            var layer = 0;
            while (listItems.Count < num)
            {
                foreach (var grill in listGrills)
                {
                    var layerData = grill.GetLayerData(layer);
                    if (layerData == null) continue;

                    var slotIdx = 0;
                    foreach (var itemData in layerData.itemData)
                    {
                        if ((ItemId)itemData.id == targetItemId)
                        {
                            var item = await grill.GetItem(layer, slotIdx);
                            listItems.Add(item);
                            if (listItems.Count == num) return listItems;
                        }

                        slotIdx++;
                    }
                }

                layer++;
            }

            return null;
        }

        protected override async UniTask PlayBoosterAnim(Vector3 position)
        {
            var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim>(
                "BoosterAnimSpatula",
                PanelManager.Instance.transform);
            // boosterAnim.SetBooster(boosterType);
            // boosterAnim.SetData(position);
            await UniTask.Delay((int)(delay * 1000));
        }
    }
}