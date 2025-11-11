using System.Collections.Generic;
using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Gameplay.LogicOrder.Configs;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "BasicOrderSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/BasicOrderSO_v2")]
    public class BasicOrderSO_v2 : BaseOrderSO
    {
        [SerializeField] private BasicOrderConfigSO_v2 basicOrderConfigSO;

        public override LogicOrderType LogicOrderType => LogicOrderType.Basic_v2;

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            var remainingSlot = basicOrderConfigSO.GetRandomRemainingSlot(gameplayInfo.phase);

            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            var numEmptyWaitingSlot = waitingGrillManager.ListWaitingGrills.Count(
                e => e.IsActive == true
                && e.GetSlots().Count(s => s.GetItem() == null) > 0
                );

            Debug.Log("<color=purple>BasicOrderSO_v2:</color> GetOrder: " + remainingSlot + " " + numEmptyWaitingSlot);
            var dictNeededSlots = gameplayInfo.DictNeededSlots;
            var listRandomItemIds = new List<(ItemId itemId, int numItems)>();
            foreach (var itemId in dictNeededSlots.Keys)
            {
                foreach (var (numItems, step) in dictNeededSlots[itemId])
                {
                    if (numEmptyWaitingSlot - step == remainingSlot)
                    {
                        listRandomItemIds.Add((itemId, numItems));
                    }
                }
            }
            if (listRandomItemIds.Count > 0)
            {
                var maxNumItems = listRandomItemIds.Max(e => e.numItems);
                var randomItemIds = listRandomItemIds.Where(e => e.numItems == maxNumItems).ToList();
                var randomItemId = randomItemIds[UnityEngine.Random.Range(0, randomItemIds.Count)];
                return (randomItemId.itemId, randomItemId.numItems);
            }
            else
            {
                return (ItemId.None, 0);
            }
        }

        public override void Init()
        {
            throw new System.NotImplementedException();
        }
    }
}