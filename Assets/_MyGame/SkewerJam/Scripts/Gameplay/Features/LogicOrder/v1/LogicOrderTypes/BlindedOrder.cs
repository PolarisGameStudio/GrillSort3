using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "BlindedOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/BlindedOrderSO")]
    public class BlindedOrderSO : BaseOrderSO
    {
        [SerializeField] private int maxGap = 2;

        private int gap = 0;

        public override LogicOrderType LogicOrderType => LogicOrderType.Blinded;
        public override void Init()
        {
            gap = 0;
        }

        public override bool CanUse(GameplayInfoForLogicOrder gameplayInfo)
        {
            var blindedItems = ItemHelper.GetItemIdsInBlindedGrill();
            return blindedItems.Count > 0;
        }
        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo)
        {
            // - Hidden Order không gọi Order n>1 (tránh quá khó)
            // - Không gọi 2 Hidden Order liên tục (tránh quá khó)


            if (CanUse(gameplayInfo) == false || gap != 0)
            {
                return (ItemId.None, 0);
            }

            // lấy ra item Id giống với item bị lock mà có số step nhỏ nhất và tối ưu số num
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var dictOrder = orderManager.GetOrderItemsDict();

            var blindedItems = ItemHelper.GetItemIdsInBlindedGrill();
            var itemIdsInLayer1 = ItemHelper.GetItemIdDictInGameplay(1, true);

            // không gọi vào blind chưa key trên order và layer 1
            blindedItems = blindedItems.Where(e => itemIdsInLayer1.ContainsKey(e) == false || dictOrder.ContainsKey(e) == false).ToList();

            if (blindedItems.Count == 0) return (ItemId.None, 0);
            Debug.Log("<color=orange>OrderHelper:</color> GetOrder Blinded: " + blindedItems[UnityEngine.Random.Range(0, blindedItems.Count)] + " " + 1);
            gap += 1;
            return (blindedItems[UnityEngine.Random.Range(0, blindedItems.Count)], 1);
        }

        public override bool ForceUse(bool isRescue = false)
        {
            // chỉ để tính gap
            // vì lần vào gọi order cx sẽ chạy qua
            if (gap != 0)
            {
                gap += 1;
                if (gap >= maxGap)
                {
                    gap = 0;
                }
            }
            return false;
        }
    }
}