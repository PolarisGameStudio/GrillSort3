using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BoosteeManagement;
using Gameplay.Effect;
using Gameplay.Entities.Items;
using Gameplay.LevelData;
using Manager;
using Sonat.Enums;
using UnityEngine;

namespace Gameplay.Entities.Grills
{
    public class PrimaryGrillLockAndKey : PrimaryGrill
    {
        public static PrimaryGrillLockAndKey grillLock;
        [SerializeField] private Transform[] posForKey;
        [SerializeField] private GameObject[] lockObjects;
        private List<KeyEffect> keyEffects = new();
        [SerializeField] private int keyNeeded = 2;
        private int keyRemaining;

        public void AddKeyInGame()
        {
            keyRemaining++;
            if (keyRemaining > keyNeeded) keyRemaining = keyNeeded;
            lockObjects[keyRemaining - 1].SetActive(true);
        }

        public override async UniTask SetData(GrillData grillData)
        {
            lockState = 1;
            keyRemaining = 1;
            base.SetData(grillData);
            grillVisual.CloseGrill(false);
            grillLock = this;
            keyEffects = new List<KeyEffect>();
            SetLockItems(true);
        }

        public override void OpenGrillWhenStart()
        {
        }

        public override ShuffleLayerData GetShuffleLayerData()
        {
            if (IsLock)
                return null;
            return base.GetShuffleLayerData();
        }

        public override List<ShuffleLayerData> GetSubsShuffleLayerData()
        {
            if (IsLock)
                return null;
            return base.GetSubsShuffleLayerData();
        }

        public void OnCollectItemKey(ItemKey item)
        {
            if (!IsLock) return;
            keyRemaining--;
            if (keyRemaining < 0) keyRemaining = 0;

            CollectAsync(item).Forget();
        }

        private async UniTask CollectAsync(ItemKey item)
        {
            var keyEffect = await MySonatFramework.poolingServiceAsync.CreateAsync<KeyEffect>("KeyEffect");
            keyEffects.Add(keyEffect);
            keyEffect.transform.SetParent(this.transform);
            Vector3 pos = item != null ? item.transform.position : Vector3.zero;
            keyEffect.SetData(pos, posForKey[keyRemaining].position, () =>
            {
                MySonatFramework.audioService.PlaySound(AudioId.Obstacle_Locknkey_Open_Grill_sort);
                if (keyRemaining == 0)
                    Unlock();
                if (keyEffect != null)
                {
                    keyEffect.StopAllCoroutines();
                    MySonatFramework.poolingServiceAsync.ReturnObj(keyEffect);
                    keyEffects.Remove(keyEffect);
                }

                lockObjects[keyRemaining].SetActive(false);
            });
        }

        public override void OnReturnObj()
        {
            base.OnReturnObj();
            foreach (var keyEffect in keyEffects)
            {
                if (keyEffect != null)
                {
                    keyEffect.StopAllCoroutines();
                    MySonatFramework.poolingServiceAsync.ReturnObj(keyEffect);
                }
            }

            keyEffects.Clear();
            keyRemaining = 0;
        }

        private void OnDisable()
        {
            grillLock = null;
        }

        public override void Unlock()
        {
            base.Unlock();
            grillVisual.OpenGrill();
        }
    }
}