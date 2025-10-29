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

        public override void SetLevelData(LevelData_SkewerJam levelData)
        {

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
            var blindedItems = ItemHelper.GetItemIdsInBlindedGrill();
            var randomItemId = blindedItems[UnityEngine.Random.Range(0, blindedItems.Count)];
            // var (itemId, num, step) = OrderHelper.GetOptimizedRandomSpecialItem(blindedItems, gameplayInfo);
            Debug.Log("<color=orange>OrderHelper:</color> GetOrder Blinded: " + randomItemId + " " + 1);
            gap += 1;
            return (randomItemId, 1);
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