using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] protected Image bg;
        [SerializeField] protected Transform root;
        [Space]
        [Header("Anim")]
        [SerializeField] protected float scaleDuration;
        [SerializeField] protected AnimationCurve scaleCurve;
        [SerializeField] protected float delayMove;
        [SerializeField] protected AnimationCurve moveXCurve, moveYCurve;
        [SerializeField] protected float duration;
        [SerializeField] protected bool rotate;
        [SerializeField] protected float scaleDown;
        [SerializeField] protected float fadeOutDuration;
        [SerializeField] protected AnimationCurve fadeOutCurve;

        [Header("Destroy")]
        [SerializeField] protected bool returnPool = true;

        protected virtual async UniTask DOEffect()
        {
            root.transform.DOKill();
            await root.transform.DOScale(1, scaleDuration).From(0).SetEase(scaleCurve);

            await UniTask.Delay((int)(delayMove * 1000));
            txtQuantity.DOScale(0, 0.3f).SetEase(Ease.Linear);

            root.transform.DOScale(scaleDown, duration).SetEase(Ease.Linear);
            root.transform.DOMoveX(targetPosition.x, duration).SetEase(moveXCurve);
            root.transform.DOMoveY(targetPosition.y, duration).SetEase(moveYCurve).OnComplete(() =>
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

            bg.DOKill();
            bg.DOFade(0, fadeOutDuration).SetEase(fadeOutCurve);

            if (rotate)
            {
                root.transform.Rotate(Vector3.forward, Random.Range(-360, 360));
                root.transform.DORotate(Vector3.zero, duration, RotateMode.FastBeyond360).SetEase(Ease.OutBounce);
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

            root.transform.DOKill();
            root.transform.localScale = Vector3.one;
            root.transform.position = startPosition;

            txtQuantity.DOKill();
            txtQuantity.transform.localScale = Vector3.one;
            txtQuantity.text = SonatUtils.FormatNumber(quantity, format);

            DOEffect();
        }

        public override void OnReturnObj()
        {
            onCollect = null;
            bg.DOKill();
        }

        private void OnDisable()
        {
            onCollect = null;
        }
    }
}