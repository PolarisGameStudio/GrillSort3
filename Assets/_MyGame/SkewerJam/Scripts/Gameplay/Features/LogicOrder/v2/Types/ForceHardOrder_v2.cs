using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "ForceHardOrder_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/ForceHardOrder_v2")]
    public class ForceHardOrder_v2 : BaseOrderSO
    {
        public override LogicOrderType LogicOrderType => LogicOrderType.ForceHard_v2;

        [SerializeField] private ForceHardOrderConfigSO_v2 forceHardOrderConfigSO_v2;

        private ForceHardOrderConfigSO_v2.ForceHardOrderData forceHardOrderData = null;
        private int previousRemainingSlot = int.MaxValue;

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            var numEmptyWaitingSlot = waitingGrillManager.ListWaitingGrills.Count(
                e => e.IsActive == true
                && e.GetSlots().Count(s => s.GetItem() == null) > 0
                );


            var curveIndex = gameplayInfo.GetTempData(LogicOrderHandler_v2.KEY_CURVE_INDEX);
            forceHardOrderData = forceHardOrderConfigSO_v2.GetForceHardOrderData(curveIndex);
            var remainingSlotAfter = forceHardOrderData.GetRandomRemainingSlot(numEmptyWaitingSlot);

            Debug.Log("BasicOrderSO_v2: GetOrder: " + "currentNumEmptyWaitingSlot: " + numEmptyWaitingSlot + " >>> expected remainingSlot: " + remainingSlotAfter);
            var dictDeltaSlots = gameplayInfo.DictDeltaSlots;
            var dictDiff2ItemIdsAndNum = new Dictionary<int, List<(ItemId itemId, int numItems, int deltaSlot)>>();

            foreach (var itemId in dictDeltaSlots.Keys)
            {
                foreach (var (numItems, delta) in dictDeltaSlots[itemId])
                {
                    var numSlotIfOrder = numEmptyWaitingSlot - delta; // số slot còn lại nếu thực hiện order này
                    var difference = remainingSlotAfter - numSlotIfOrder;

                    dictDiff2ItemIdsAndNum.TryAdd(difference, new List<(ItemId itemId, int numItems, int deltaSlot)>());
                    dictDiff2ItemIdsAndNum[difference].Add((itemId, numItems, delta));
                }
            }

            if (dictDiff2ItemIdsAndNum.Count != 0)
            {
                var listDiffKeys = dictDiff2ItemIdsAndNum.Keys.ToList();
                var positiveDiffKeys = listDiffKeys.Where(e => e >= 0).ToList().OrderBy(e => e).ToList();

                foreach (var diff in positiveDiffKeys)
                {
                    var listItemIdAndNum = dictDiff2ItemIdsAndNum[diff];
                    var maxNumItems = listItemIdAndNum.Max(e => e.numItems);
                    if (maxNumItems == 1) continue;

                    var filteredRandomItemIds = listItemIdAndNum.Where(e => e.numItems == maxNumItems).ToList();
                    var randomItemId = filteredRandomItemIds[UnityEngine.Random.Range(0, filteredRandomItemIds.Count)];
                    Debug.Log("BasicOrderSO_v2: GetOrder: " + "result remaining slots: >= " + (numEmptyWaitingSlot - randomItemId.deltaSlot));
                    Debug.Log("BasicOrderSO_v2: GetOrder: >> delta slot: " + randomItemId.deltaSlot);
                    Debug.Log("BasicOrderSO_v2: GetOrder: >> item id: " + randomItemId.itemId);
                    Debug.Log("BasicOrderSO_v2: GetOrder: >> num items: " + randomItemId.numItems);
                    gap = 1;
                    return (randomItemId.itemId, randomItemId.numItems);
                }



                var negativeDiffKeys = listDiffKeys.Where(e => e < 0).ToList().OrderBy(e => Random.Range(0, 100)).ToList();

                foreach (var diff in negativeDiffKeys)
                {
                    var listItemIdAndNum = dictDiff2ItemIdsAndNum[diff];
                    var maxNumItems = listItemIdAndNum.Max(e => e.numItems);
                    if (maxNumItems == 1) continue;

                    var filteredRandomItemIds = listItemIdAndNum.Where(e => e.numItems == maxNumItems).ToList();
                    var randomItemId = filteredRandomItemIds[UnityEngine.Random.Range(0, filteredRandomItemIds.Count)];
                    Debug.Log("BasicOrderSO_v2: GetOrder: " + "result remaining slots: >= " + (numEmptyWaitingSlot - randomItemId.deltaSlot));
                    Debug.Log("BasicOrderSO_v2: GetOrder: >> delta slot: " + randomItemId.deltaSlot);
                    Debug.Log("BasicOrderSO_v2: GetOrder: >> item id: " + randomItemId.itemId);
                    Debug.Log("BasicOrderSO_v2: GetOrder: >> num items: " + randomItemId.numItems);
                    gap = 1;
                    return (randomItemId.itemId, randomItemId.numItems);
                }
            }

            return (ItemId.None, 0);
        }

        public override void Init()
        {
            previousRemainingSlot = int.MaxValue;
            gap = 0;
        }

        public override void ResetData()
        {
            previousRemainingSlot = int.MaxValue;
        }

        private int gap = 0;
        private int nextGap = 3;


        public override bool CanUse(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            if (gap == 0) return true;

            if (gap == nextGap && forceHardOrderData != null && forceHardOrderData.CanNextUse() == true) return true;
            return false;
        }
        public override bool CheckForceUse(bool isRescue = false)
        {
            if (gap != 0)
            {
                gap += 1;
            }
            return false;
        }
    }
}