using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public abstract class BaseLogicOrderHandler : MonoBehaviour
    {
        [SerializeField] protected List<BaseOrderSO> listLogicOrders;

        public bool IsForceRescue { get; private set; }
        public ForceRescueData ForceRescueData => _forceRescueData;
        private ForceRescueData _forceRescueData = new ForceRescueData();

        public virtual void Init()
        {
            foreach (var logicOrder in listLogicOrders)
            {
                logicOrder.Init();
            }

            SetForceRescue(false, -1);
        }

        public virtual void Clear()
        {
            SetForceRescue(false, -1);
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



        public void SetForceRescue(bool isForceRescue, int deltaSlot = -1)
        {
            IsForceRescue = isForceRescue;
            if (isForceRescue == true)
            {
                _forceRescueData.deltaSlot = deltaSlot;
            }
            else
            {
                _forceRescueData.deltaSlot = -1;
            }
        }



        public abstract UniTask<(ItemId itemId, int num, LogicOrderType logicOrderType)> GetItemOrder(bool isRescue = false);
    }

    public class ForceRescueData
    {
        public int deltaSlot = -1;
    }
}