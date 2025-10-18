using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.SkewerJam.Gameplay.LogicOrder;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public class LogicOrderHandler : MonoBehaviour
    {
        [SerializeField] private List<BaseOrderSO> listLogicOrders;

        // OPTIMIZE: LOGIC ORDER
        private int rescueGap = 0;
        private int currentNumberRescues = 0;
        private int stepGap = 0;
        public int maxStep2Gap = 3;


        public void Reset()
        {
            rescueGap = 0;
            currentNumberRescues = 0;

            stepGap = 0;
        }

        public bool CheckCreateNextOrder()
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;

            var dictAllItemIds = ItemHelper.GetItemIdDictInGameplay(-1);
            var orderItemsDict = orderManager.GetOrderItemsDict();

            // số item còn lại <= số item còn lại tạo order
            foreach (var id in dictAllItemIds.Keys)
            {
                if (orderItemsDict.ContainsKey(id) == true)
                {
                    // Nếu item target + item còn lại cùng loại nó > maxItems thì cần tạo thêm order
                    if (dictAllItemIds[id] + orderItemsDict[id].num > orderItemsDict[id].maxItems) return true;
                }
                else
                {
                    // Nếu item còn lại không có trong target thì cần tạo order
                    return true;
                }
            }
            return false;
        }

        public async UniTask<(ItemId itemId, int num)> GetItemOrder(bool isRescue = false)
        {
            var forceLogicOrder = listLogicOrders.FirstOrDefault(e => e.ForceUse(isRescue));

            // khoảng gap giữa các lần rescue và giới hạn số lần
            if (forceLogicOrder != null)
            {
                Debug.Log("<color=blue>OrderHelper:</color> Use " + forceLogicOrder.name + " to rescue");
                var (rescueItemId, rescueNum) = forceLogicOrder.GetOrder();
                return (rescueItemId, rescueNum);
            }

            // lấy order info mỗi layer (2 layer đầu) --> OPTIMIZE: giảm tính toán
            var gameplayInfoForLogicOrder = new GameplayInfoForLogicOrder();
            gameplayInfoForLogicOrder.UpdateState();
            var selectedLogicOrder = ChooseLogicOrder();
            Debug.Log("<color=green>OrderHelper:</color> Use " + selectedLogicOrder.name);
            return selectedLogicOrder.GetOrder(gameplayInfoForLogicOrder);

            // var (itemId, num, step) = GetItemOrderBasic(logicOrderConfig.minNumberSteps, gameplayInfo);
            // if (itemId != ItemId.None)
            // {
            //     if (rescueGap > 0) rescueGap--;
            //     if (step >= 2 && stepGap <= 0)
            //     {
            //         stepGap = maxStep2Gap;
            //         return (itemId, num);
            //     }

            //     if (step < 2)
            //     {
            //         stepGap -= 1;
            //         stepGap = Mathf.Min(stepGap, maxStep2Gap);
            //         return (itemId, num);
            //     }
            // }
            // stepGap -= 1;
            // stepGap = Mathf.Min(stepGap, maxStep2Gap);
            // var (itemId2, num2, step2) = ForceGetItemOrderBasic(gameplayInfo);
            // if (step2 >= 2) stepGap = maxStep2Gap;
            // return (itemId2, num2);

        }

        private BaseOrderSO ChooseLogicOrder()
        {
            // kiểm tra xem có sử dụng được nó không
            foreach (var logicOrder in listLogicOrders)
            {
                if (logicOrder.CanUse())
                {
                    return logicOrder;
                }
            }
            Debug.Log("<color=red>OrderHelper:</color> No logic order can use");
            return null;
        }
    }
}