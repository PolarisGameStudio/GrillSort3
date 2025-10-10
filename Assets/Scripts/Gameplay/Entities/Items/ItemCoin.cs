using Gameplay.LevelData;

namespace Gameplay.Entities.Items
{
    public class ItemCoin : Item
    {
        private int coin;

        public int Coin => coin;

        public override void SetItemData(ItemData data, SlotBase slot)
        {
            base.SetItemData(data, slot);

            if (data is ItemCoinData itemCoinData)
            {
                coin = itemCoinData.coin;
            }
            else
            {
                coin = 1;
            }

        }

        public override void OnMoveItem()
        {

        }

        public override void OnUnSelectItem()
        {

        }
    }
}