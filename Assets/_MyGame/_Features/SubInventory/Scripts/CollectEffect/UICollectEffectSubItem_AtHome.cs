using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Modules.SubInventory;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyGame.Modules.SubInventory.Scripts.CollectEffect
{
    public class UICollectEffectSubItem_AtHome : UICollectEffectBase
    {
        protected Vector3 startPosition;
        protected Vector3 startPosition2;
        protected Vector3 targetPosition;

        protected Action onCollect;
        protected Action onCollectLoop;

        [SerializeField] protected TMP_Text txtQuantity;
        [SerializeField] protected string format = "x{0}";
        [Space]
        [Header("Anim")]
        [SerializeField] protected float scaleDuration;
        [SerializeField] protected AnimationCurve scaleCurve;
        [SerializeField] protected float delayMove;
        [SerializeField] protected AnimationCurve moveXCurve, moveYCurve;
        [SerializeField] protected float duration;
        [SerializeField] protected bool rotate;
        [SerializeField] protected float scaleDown;

        [Header("Destroy")]
        [SerializeField] protected bool returnPool = true;

        protected virtual async UniTask DOEffect()
        {
            transform.DOKill();
            await transform.DOScale(1, duration).From(0).SetEase(scaleCurve);

            await UniTask.Delay((int)(delayMove * 1000));
            txtQuantity.DOScale(0, 0.3f).SetEase(Ease.Linear);

            transform.DOScale(scaleDown, duration).SetEase(Ease.Linear);
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

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.position = startPosition;

            txtQuantity.DOKill();
            txtQuantity.transform.localScale = Vector3.one;
            txtQuantity.text = SonatUtils.FormatNumber(quantity, format);
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