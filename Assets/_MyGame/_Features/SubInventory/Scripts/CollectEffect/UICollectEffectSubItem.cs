using System;
using DG.Tweening;
using MyGame.Modules.SubInventory;
using Sonat.Enums;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyGame.Modules.SubInventory.Scripts.CollectEffect
{
    public class UICollectEffectSubItem : UICollectEffectBase
    {
        protected Vector3 startPosition;
        protected Vector3 startPosition2;
        protected Vector3 targetPosition;

        protected Action onCollect;
        protected Action onCollectLoop;

        [SerializeField] protected AnimationCurve moveXCurve, moveYCurve;
        [SerializeField] protected float duration;
        [SerializeField] protected bool rotate;

        [Header("Destroy")]
        [SerializeField] protected bool returnPool = true;

        protected virtual void DOEffect()
        {
            transform.DOMoveX(targetPosition.x, duration).SetEase(moveXCurve);
            transform.DOMoveY(targetPosition.y, duration).SetEase(moveYCurve).OnComplete(() =>
            {
                try
                {
                    onCollect?.Invoke();
                    onCollectLoop?.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                if (returnPool)
                    SonatSystem.GetService<PoolingServiceAsync>().ReturnObj(this);
                else
                    gameObject.SetActive(false);
            });
            if (rotate)
            {
                transform.Rotate(Vector3.forward, Random.Range(-360, 360));
                transform.DORotate(Vector3.zero, duration, RotateMode.FastBeyond360).SetEase(Ease.OutBounce);
            }
        }

        public override void Setup()
        {
        }

        public override void OnCreateObj(params object[] args)
        {
            SubGameResource resource = (SubGameResource)args[0];
            int quantity = (int)args[1];
            startPosition = (Vector3)args[2];
            startPosition2 = (Vector3)args[3];
            targetPosition = (Vector3)args[4];
            if (args.Length > 5)
                onCollect = (Action)args[5];
            else
            {
                onCollect = null;
            }

            if (args.Length > 6)
                onCollectLoop = (Action)args[6];
            else
            {
                onCollectLoop = null;
            }

            transform.position = startPosition;
            DOEffect();
        }

        public override void OnReturnObj()
        {
            onCollect = null;
        }

        private void OnDisable()
        {
            onCollect = null;
        }
    }
}