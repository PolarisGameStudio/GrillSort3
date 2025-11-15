using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Effect;
using Gameplay.Entities.Items;
using Gameplay.Entities.Obstacle.Visual;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Entities.Obstacle
{
    public class LockAreaObstacle : ObstacleBase
    {
        public static LockAreaObstacle instance;
        [SerializeField] private ObstacleType obstacleType;
        [SerializeField] private Transform[] slots;
        [SerializeField] private LockAreaVisual visual;

        public override ObstacleType ObstacleType => obstacleType;
        private List<PrimaryGrill> primaryGrills;
        private ItemKeyArea key;
        [SerializeField] private SortingGroup sortingGroup;

        private List<KeyEffect> keyEffects = new();
        //private int keyRemaining;
        //[SerializeField] private int keyNeeded = 1;

        public Transform LockPosition => visual.GetLockPosition();

        // public void AddKeyInGame()
        // {
        //     keyRemaining++;
        //     if (keyRemaining > keyNeeded) keyRemaining = keyNeeded;
        //     lockObjects[keyRemaining - 1].SetActive(true);
        // }

        public override void SetData(ObstacleData data)
        {
            base.SetData(data);
            transform.position = data.position.ToVector3();
            visual.Lock();
            sortingGroup.enabled = false;
            keyEffects = new List<KeyEffect>();
        }

        public override void SetGrill(List<GrillBase> grills)
        {
            primaryGrills = new List<PrimaryGrill>();
            int index = 0;
            var listPosition = new List<Vector3>();
            foreach (var grill in grills)
            {
                if (grill is PrimaryGrill primaryGrill)
                {
                    primaryGrill.AddLockState();
                    primaryGrills.Add(primaryGrill);
                    primaryGrill.transform.SetParent(slots[index]);
                    // primaryGrill.transform.localPosition = Vector3.zero;

                    primaryGrill.transform.localScale = Vector3.one * 0.85f;
                    if (obstacleType == ObstacleType.LockAreaHorizontal)
                    {
                        primaryGrill.transform.SetLocalPositionY(0);
                    }
                    else
                    {
                        primaryGrill.transform.SetLocalPositionX(0);
                    }

                    index++;

                    listPosition.Add(primaryGrill.transform.position);
                }
            }

            instance = this;
            active = true;
        }

        public void SetKey(ItemKeyArea key)
        {
            this.key = key;
        }

        public override void OnComplete()
        {
        }

        public void OnCollectItemKey(ItemKeyArea item)
        {
            if (!active) return;

            CollectAsync(item).Forget();
        }

        private async UniTask CollectAsync(ItemKeyArea item)
        {
            var keyEffect = await MySonatFramework.poolingServiceAsync.CreateAsync<KeyEffect>("KeyEffect");
            keyEffects.Add(keyEffect);
            keyEffect.transform.SetParent(this.transform);
            keyEffect.SetData(item.transform.position, LockPosition.position, () =>
            {
                MySonatFramework.audioService.PlaySound(AudioId.Obstacle_Locknkey_Open_Grill_sort);
                Unlock();
                if (keyEffect != null)
                {
                    keyEffect.StopAllCoroutines();
                    MySonatFramework.poolingServiceAsync.ReturnObj(keyEffect);
                    keyEffects.Remove(keyEffect);
                }
            });
        }

        private void Unlock()
        {
            if (!active) return;
            foreach (var primaryGrill in primaryGrills)
            {
                var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
                primaryGrill.transform.SetParent(grillManager.transform);
                primaryGrill.UnlockState();
                //if (obstacleType == ObstacleType.LockAreaHorizontal)
                primaryGrill.transform.DOScale(1, 0.3f).SetEase(Ease.InOutQuad).SetDelay(0.65f);
            }

            visual.Unlock();
            active = false;
            OnComplete();
        }

        public void UnlockByBooster()
        {
            if (!active) return;
            Unlock();
            key?.OnUnlockByBooster();
        }

        public void DestroyObstacle()
        {
            MyGame.SkewerJam.Gameplay.GameFactory.Instance.ReturnEntity(this);
        }

        public override void Setup()
        {
        }

        public override void OnCreateObj(params object[] args)
        {
        }

        public override void OnReturnObj()
        {
            foreach (var keyEffect in keyEffects)
            {
                if (keyEffect != null)
                {
                    keyEffect.StopAllCoroutines();
                    MySonatFramework.poolingServiceAsync.ReturnObj(keyEffect);
                }
            }

            keyEffects.Clear();
        }

        private void OnDisable()
        {
            instance = null;
        }

        public override void Highlight(bool highlight)
        {
            base.Highlight(highlight);
            sortingGroup.enabled = highlight;
        }
    }
}