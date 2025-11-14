using System;
using Cysharp.Threading.Tasks;
using Gameplay.LevelData;
using Sonat.Enums;
using UnityEngine;

namespace Gameplay.Entities.Items
{
    public class ItemIce : Item
    {
        [SerializeField] private SpriteRenderer iceSprite;
        //private bool isIced = true;

        // public override void OnMouseDown()
        // {
        // }
        public override void SetItemData(ItemData data, SlotBase slot)
        {
            base.SetItemData(data, slot);
            //isIced = true;
            if (data is CurrentItemData currentItemData)
            {
                locked = currentItemData.isLocked;
            }
            else
            {
                locked = true;
            }
            iceSprite.gameObject.SetActive(locked);

            itemBehaviorSO.eventSystemSO.RegisterEvents_OnCollectItem(OnCollectItem);
        }
        // public override bool CanNotTouch()
        // {
        //     return base.CanNotTouch();
        // }


        // protected override void OnMouseDrag()
        // {
        // }
        //
        // protected override void OnMouseUp()
        // {
        // }
        // public override void OnMoveItem()
        // {

        // }

        // public override void OnUnSelectItem()
        // {

        // }

        public override void OnComplete()
        {
            if (IsLocked)
            {
                PlayBreakEffect();
            }

            base.OnComplete();
        }

        private void PlayBreakEffect()
        {
            PlayBreakEffectAsync().Forget();
        }

        private async UniTask PlayBreakEffectAsync()
        {
            var effect = await MySonatFramework.poolingServiceAsync.CreateAsync<EffectPoolBase>("ItemIceBreakEffect", transform.position);
            MySonatFramework.audioService.PlaySound(AudioId.Obstacle_Ice_break_Grill_sort_01);
        }

        public void Unlock()
        {
            //isIced = false;
            locked = false;
            base.SetLockState(false);
            PlayBreakEffect();
            iceSprite.gameObject.SetActive(false);
        }

        private void OnCollectItem(int itemId)
        {
            if (Data.id != itemId) return;
            itemBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);
            Unlock();
        }

        public override void OnReturnObj()
        {
            itemBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);
            base.OnReturnObj();
        }




        public class CurrentItemData : ItemData
        {
            public bool isLocked;
        }

        public override ItemData GetCurrentItemData()
        {
            ItemData data = new CurrentItemData()
            {
                hidden = Data.hidden,
                id = Data.id,
                isLocked = IsLocked,
                itemType = Data.itemType,
            };
            return data;
        }
    }
}