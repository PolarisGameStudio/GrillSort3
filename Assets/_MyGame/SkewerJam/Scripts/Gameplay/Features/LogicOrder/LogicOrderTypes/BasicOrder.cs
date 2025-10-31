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
            Debug.Log("<color=white>BasicOrderSO:</color> SetIndexBO: " + indexBO);
            selectedBO = basicOrderConfigSO.listBasicOrderConfigs[indexBO];
        }

        public void SetMinNum(int minNum)
        {
            this.minNum = minNum;
        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder info)
        {
            var selectedDeltaSlot = GetSelectedDeltaSlot();
            Debug.Log("<color=white>BasicOrderSO:</color> GetOrder: selectedDeltaSlot: " + selectedDeltaSlot);
            var (itemId, num, deltaSlot) = GetItemOrderBasic(selectedDeltaSlot, info);

            if (itemId == ItemId.None)
            {
                Debug.Log("<color=yellow>OrderHelper:</color> GetItemOrderBasic: No item found");
                (itemId, num, deltaSlot) = ForceGetItemOrderBasic(info);
                return (itemId, num);
            }
            return (itemId, num);
        }

        private int GetSelectedDeltaSlot()
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

        private (ItemId itemId, int num, int deltaSlot) GetItemOrderBasic(int selectedDeltaSlot, GameplayInfoForLogicOrder info)
        {
            // NOTE: Ưu tiên lấy theo numSteps, sau đó mới tính đến numItems
            var dictDeltaSlots = info.DictDeltaSlots;
            if (dictDeltaSlots.Count == 0)
            {
                return (ItemId.None, 0, 0);
            }

            var itemIds = dictDeltaSlots.Keys;

            var maxDeltaSlot = dictDeltaSlots.Values.Max(e => e.Values.Max());
            for (int deltaSlot = selectedDeltaSlot; deltaSlot <= maxDeltaSlot; deltaSlot++)
            {
                var randomItemIds = itemIds.Where(e => dictDeltaSlots[e].ContainsValue(deltaSlot)).ToList();
                var randomItemIdsWithMinNum = new List<ItemId>();
                foreach (var id in randomItemIds)
                {
                    foreach (var numItems in dictDeltaSlots[id].Keys)
                    {
                        if (numItems >= minNum && dictDeltaSlots[id][numItems] == deltaSlot)
                        {
                            randomItemIdsWithMinNum.Add(id);
                        }
                    }
                }
                if (randomItemIdsWithMinNum == null || randomItemIdsWithMinNum.Count == 0) continue;

                var (itemId, num, s) = OrderHelper.GetOptimizedRandomItem(randomItemIdsWithMinNum, deltaSlot, info, minNum);
                Debug.Log("<color=white>OrderHelper:</color> GetItemOrderBasic: " + itemId + " " + num + " step: " + s);
                return (itemId, num, s);
            }

            return (ItemId.None, 0, 0);
        }

        public (ItemId itemId, int num, int deltaSlot) ForceGetItemOrderBasic(GameplayInfoForLogicOrder gameplayInfo)
        {
            var dictDeltaSlots = gameplayInfo.DictDeltaSlots;
            if (dictDeltaSlots.Count == 0)
            {
                Debug.Log("<color=red>OrderHelper:</color> ForceGetItemOrderBasic: dictDeltaSlots.Count == 0");
                return (ItemId.None, 0, 0);
            }
            var minDeltaSlot = dictDeltaSlots.Values.Min(e => e.Values.Min());

            var randomItemIds = dictDeltaSlots.Keys.Where(e => dictDeltaSlots[e].ContainsValue(minDeltaSlot)).ToList();
            var (itemId, num, deltaSlot) = OrderHelper.GetOptimizedRandomItem(randomItemIds, minDeltaSlot, gameplayInfo);
            Debug.Log("<color=yellow>OrderHelper:</color> ForceGetItemOrderBasic: " + itemId + " " + num + " minDeltaSlot: " + deltaSlot);
            return (itemId, num, deltaSlot);
        }
    }
}