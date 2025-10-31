using System.Linq;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    [CreateAssetMenu(fileName = "RandomOrderSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/RandomOrderSO")]
    public class RandomOrderSO : BaseOrderSO
    {
        public override LogicOrderType LogicOrderType => LogicOrderType.Random;
        public override void Init()
        {

        }

        public override void SetLevelData(LevelData_SkewerJam levelData)
        {
        }

        public override (ItemId itemId, int num) GetOrder(GameplayInfoForLogicOrder gameplayInfo = null)
        {
            var itemDict = ItemHelper.GetItemIdDictInGameplay(-1);

            // trừ đi item đã order
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var orderItemsDict = orderManager.GetOrderItemsDict();
            foreach (var itemId in orderItemsDict.Keys)
            {
                if (itemDict.ContainsKey(itemId) == false)
                {
                    Debug.Log("<color=red>OrderHelper:</color> GetRandomOrder: itemDict.ContainsKey(itemId) == false");
                }
                itemDict[itemId] -= (orderItemsDict[itemId].maxItems - orderItemsDict[itemId].num);
                if (itemDict[itemId] <= 0)
                {
                    itemDict.Remove(itemId);
                }
            }


            // ưu tiên lấy order 2, 3 trước và item ở layer 1
            var itemsInLayer1 = ItemHelper.GetItemIdDictInGameplay(1);
            var filteredItemDict = itemDict.Where(e => e.Value >= 2 && itemsInLayer1.ContainsKey(e.Key) == true).ToList();
            if (filteredItemDict.Count > 0)
            {
                var rand = filteredItemDict[UnityEngine.Random.Range(0, filteredItemDict.Count)].Key;
                var num = itemDict[rand] > 3 ? 3 : itemDict[rand];
                Debug.Log("<color=green>OrderHelper:</color> GetRandomOrder: " + rand + " " + num);
                return ((ItemId)rand, num);
            }

            var randomItemId = itemDict.Keys.ToList()[UnityEngine.Random.Range(0, itemDict.Keys.ToList().Count)];
            var number = itemDict[randomItemId] > 3 ? 3 : itemDict[randomItemId];
            Debug.Log("<color=green>OrderHelper:</color> GetRandomOrder: " + randomItemId + " " + number);
            return ((ItemId)randomItemId, number);
        }
    }
}