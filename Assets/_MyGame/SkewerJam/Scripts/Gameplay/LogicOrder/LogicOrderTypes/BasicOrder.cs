using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "BasicOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/BasicOrderSO")]
    public class BasicOrderSO : BaseOrderSO
    {
        [SerializeField] private BasicOrderConfigSO basicOrderConfigSO;

        private BO selectedBO;
        private int minNum = 1;


        public override LogicOrderType LogicOrderType => LogicOrderType.Basic;
        public override void Init()
        {

        }

        public override void SetLevelData(LevelData_SkewerJam levelData)
        {

        }

        public void SetData(int indexBO)
        {
            Debug.Log("<color=white>BasicOrderSO:</color> SetData: " + indexBO);
            selectedBO = basicOrderConfigSO.listBasicOrderConfigs[indexBO];
        }

        public void SetMinNum(int minNum)
        {
            this.minNum = minNum;
        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder info)
        {
            var selectedNumStep = GetSelectedNumStep();
            Debug.Log("<color=white>BasicOrderSO:</color> GetOrder: selectedNumStep: " + selectedNumStep);
            var (itemId, num, step) = GetItemOrderBasic(selectedNumStep, info);

            if (itemId == ItemId.None)
            {
                Debug.Log("<color=yellow>OrderHelper:</color> GetItemOrderBasic: No item found");
                (itemId, num, step) = ForceGetItemOrderBasic(info);
                return (itemId, num);
            }
            return (itemId, num);
        }

        private int GetSelectedNumStep()
        {
            var random = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("<color=white>BasicOrderSO:</color> GetSelectedNumStep: " + Mathf.Round(random * 100f) * 0.01f + " >>> " + selectedBO.rateWith0Step + " - " + selectedBO.rateWith1Step + " - " + selectedBO.rateWith2Step + " - " + selectedBO.rateWith3Step);
            if (random < selectedBO.rateWith0Step)
            {
                return 0;
            }
            else if (random < selectedBO.rateWith0Step + selectedBO.rateWith1Step)
            {
                return 1;
            }
            else if (random < selectedBO.rateWith0Step + selectedBO.rateWith1Step + selectedBO.rateWith2Step)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        private (ItemId itemId, int num, int step) GetItemOrderBasic(int selectedStep, GameplayInfoForLogicOrder info)
        {
            // NOTE: Ưu tiên lấy theo numSteps, sau đó mới tính đến numItems
            var dictNeededSlots = info.DictNeededSlots;
            if (dictNeededSlots.Count == 0)
            {
                return (ItemId.None, 0, 0);
            }

            var itemIds = dictNeededSlots.Keys;

            var maxStep = dictNeededSlots.Values.Max(e => e.Values.Max());
            for (int step = selectedStep; step <= maxStep; step++)
            {
                var randomItemIds = itemIds.Where(e => dictNeededSlots[e].ContainsValue(step)).ToList();
                var randomItemIdsWithMinNum = new List<ItemId>();
                foreach (var id in randomItemIds)
                {
                    foreach (var numItems in dictNeededSlots[id].Keys)
                    {
                        if (numItems >= minNum && dictNeededSlots[id][numItems] == step)
                        {
                            randomItemIdsWithMinNum.Add(id);
                        }
                    }
                }
                if (randomItemIdsWithMinNum == null || randomItemIdsWithMinNum.Count == 0) continue;

                var (itemId, num, s) = OrderHelper.GetOptimizedRandomItem(randomItemIdsWithMinNum, step, info, minNum);
                Debug.Log("<color=white>OrderHelper:</color> GetItemOrderBasic: " + itemId + " " + num + " step: " + s);
                return (itemId, num, s);
            }

            return (ItemId.None, 0, 0);
        }

        public (ItemId itemId, int num, int step) ForceGetItemOrderBasic(GameplayInfoForLogicOrder gameplayInfo)
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