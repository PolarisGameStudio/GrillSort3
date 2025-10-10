using Gameplay.Entities.Grills;
using Gameplay.Entities.Obstacle;
using Gameplay.LevelData;
using SonatFramework.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class ItemKeyArea : Item
    {
        [SerializeField] private Transform keyObj;

        public override void SetItemData(ItemData data, SlotBase slot)
        {
            base.SetItemData(data, slot);
            SonatUtils.ExecuteNextFrame(() => { LockAreaObstacle.instance.SetKey(this); }, 2);
            keyObj.gameObject.SetActive(true);
        }

        public override void OnComplete()
        {
            base.OnComplete();
            LockAreaObstacle.instance.OnCollectItemKey(this);
        }

        public void OnUnlockByBooster()
        {
            keyObj.gameObject.SetActive(false);
        }
    }
}