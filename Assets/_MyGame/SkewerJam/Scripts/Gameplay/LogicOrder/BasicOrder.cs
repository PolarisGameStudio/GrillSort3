using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "BasicOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/BasicOrderSO")]
    public class BasicOrderSO : BaseOrderSO
    {
        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder info)
        {
            var level = GameController.Instance.Level;
            var selectedNumStep = level % 4;
            var (itemId, num, step) = GetItemOrderBasic(selectedNumStep, info);

            if (itemId == ItemId.None)
            {
                Debug.Log("<color=yellow>OrderHelper:</color> GetItemOrderBasic: No item found");
                (itemId, num, step) = ForceGetItemOrderBasic(info);
                return (itemId, num);
            }
            return (itemId, num);
        }

        private (ItemId itemId, int num, int step) GetItemOrderBasic(int minStep, GameplayInfoForLogicOrder info)
        {
            // NOTE: Ưu tiên lấy theo numSteps, sau đó mới tính đến numItems
            var dictNeededSlots = info.DictNeededSlots;
            if (dictNeededSlots.Count == 0)
            {
                return (ItemId.None, 0, 0);
            }

            var itemIds = dictNeededSlots.Keys;

            var maxStep = dictNeededSlots.Values.Max(e => e.Values.Max());
            for (int step = minStep; step <= maxStep; step++)
            {
                var randomItemIds = itemIds.Where(e => dictNeededSlots[e].ContainsValue(step)).ToList();
                if (randomItemIds == null || randomItemIds.Count == 0) continue;

                var (itemId, num, s) = OrderHelper.GetOptimizedRandomItem(randomItemIds, step, info);
                Debug.Log("<color=white>OrderHelper:</color> GetItemOrderBasic: " + itemId + " " + num + " minStep: " + s);
                return (itemId, num, s);
            }

            return (ItemId.None, 0, 0);
        }

        private (ItemId itemId, int num, int step) ForceGetItemOrderBasic(GameplayInfoForLogicOrder gameplayInfo)
        {
            var dictNeededSlots = gameplayInfo.DictNeededSlots;
            var minStep = dictNeededSlots.Values.Min(e => e.Values.Min());

            var randomItemIds = dictNeededSlots.Keys.Where(e => dictNeededSlots[e].ContainsValue(minStep)).ToList();
            var (itemId, num, step) = OrderHelper.GetOptimizedRandomItem(randomItemIds, minStep, gameplayInfo);
            Debug.Log("<color=yellow>OrderHelper:</color> ForceGetItemOrderBasic: " + itemId + " " + num + " minStep: " + step);
            return (itemId, num, step);
        }

    }
}