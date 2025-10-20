using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class ItemHidden : Item
    {
        protected override void OnItemBeginDrag()
        {
            slot.OnItemDrag();
            visual.OnBeginDrag();
        }

        public override void MoveToOrder()
        {
            base.MoveToOrder();

            (visual as ItemVisualHidden).Show();
        }

        public override void OnReturnObj()
        {
            base.OnReturnObj();

            (visual as ItemVisualHidden).Hide();
        }
    }
}