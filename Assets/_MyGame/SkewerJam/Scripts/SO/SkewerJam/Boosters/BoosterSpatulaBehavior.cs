using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyGame.SkewerJam.Gameplay;
using System.Linq;
using Manager;
using System.Collections.Generic;
using Gameplay.Entities;
using MyGame.SkewerJam.Objects.Entities;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Scripts.UIModule;
using static MyGame.SkewerJam.Objects.Entities.OrderEntity;
using System;
using MyGame.SkewerJam.Utils;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterSpatulaBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterSpatulaBehaviorSO")]
    public class BoosterSpatulaBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterSpatula;

        [SerializeField] private float delayBetweenItems = 0.3f;
        [SerializeField] private float delay = 2f;

        private bool finished = false;
        private bool sucess = false;

        #region Behavior
        public override async UniTask<bool> UseBooster(Vector3 position, bool isForce = false)
        {
            finished = false;
            sucess = false;

            await WaitOrderReady();

            // chỉ hightlight những order có thể chọn
            HighlightOrders(true);

            var uiData = new UIData();
            uiData.Add(PopupHightlightGameplay.BOOSTER_KEY, boosterType);
            uiData.Add(PopupHightlightGameplay.ON_CLOSE, new Action(ClosePopupHightlightGameplay));
            uiData.Add(PopupHightlightGameplay.ON_SELECT_ITEM, new Action<OrderEntity>(OnSelectItem));
            var popupHightlightGameplay = PanelManager.Instance.OpenPanelByName<PopupHightlightGameplay>("PopupHightlightGameplay_BoosterSpatula", uiData);

            await UniTask.WaitUntil(() => finished);

            HighlightOrders(false);
            return sucess;
        }

        private async UniTask WaitOrderReady()
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var listOrders = new List<OrderEntity>(orderManager.ListOrders);

            foreach (var order in listOrders)
            {
                if (order.IsActive == false) continue;
                if (order.State == OrderEntityState.Complete) continue;
                await UniTask.WaitUntil(() => order.State == OrderEntityState.Ready);
            }
        }

        private void OnSelectItem(OrderEntity order)
        {
            CompleteOrder(order).Forget();
        }

        private void ClosePopupHightlightGameplay()
        {
            var uiBooster = GameController.Instance.GameplayScreen.GetUIBooster(boosterType);
            if (uiBooster != null)
            {
                uiBooster.SetSortingOrder(false);
            }
            finished = true;
            sucess = false;
        }

        private void HighlightOrders(bool highlight)
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            foreach (var order in orderManager.ListOrders)
            {
                if (order.IsActive == false) continue;
                if (order.State == OrderEntityState.Complete) continue;
                if (order.State == OrderEntityState.Ready)
                {
                    if (highlight)
                    {
                        order.Visual.SetSortingGroup(true, LayerManager.TopUI, 100);
                    }
                    else
                    {
                        order.Visual.SetSortingGroup(false);
                    }
                }
            }
        }

        private async UniTask CompleteOrder(OrderEntity order, bool isForce = false)
        {
            HighlightOrders(false);
            if (isForce == false)
            {
                await PlayBoosterAnim(order.transform.position);
            }

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
            sucess = true;
            finished = true;
        }

        public override async UniTask<bool> ForceUseBooster()
        {
            // await PlayBoosterAnim(Vector3.zero);

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var listOrders = new List<OrderEntity>(orderManager.ListOrders);
            Debug.Log($"BoosterSpatulaBehaviorSO: UseBooster: {listOrders.Count}");
            foreach (var order in listOrders)
            {
                if (order.IsActive == false) continue;
                if (order.State == OrderEntityState.Complete) continue;

                await UniTask.WaitUntil(() => order.State == OrderEntityState.Ready);

                await CompleteOrder(order, true);
                return true;
            }
            return false;
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
        #endregion

        public override bool CheckSuggest()
        {
            return true;
        }

        public override async UniTask PostUseBooster()
        {
            // order tiếp theo cũng phải là rescue
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var orderManager = gameLogicHandler.OrderManager;
            orderManager.LogicOrderHandler.SetForceRescue(true);
        }
    }
}