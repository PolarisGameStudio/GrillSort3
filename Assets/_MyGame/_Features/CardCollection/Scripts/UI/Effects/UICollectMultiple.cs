using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

public class UICollectMultiple : MonoBehaviour, IPoolingObject
{
    [Header("Resource")]
    [SerializeField] protected UIResourceItem uiResourceItem;

    [Space]
    [Header("Anim")]
    [SerializeField] protected float scaleDuration;
    [SerializeField] protected AnimationCurve scaleCurve;
    [SerializeField] protected float scaleDownDuration;
    [SerializeField] protected AnimationCurve moveXCurve, moveYCurve;
    [SerializeField] protected float moveDuration;
    [SerializeField] protected bool rotate;

    [Header("Destroy")]
    [SerializeField] protected bool returnPool = true;

    private int quantity;
    private Vector3 startPosition;
    private Transform targetRoot;
    private Action onCollect;
    private float delayMove;


    #region IPoolingObject
    public void OnCreateObj(params object[] args)
    {
        quantity = (int)args[0];
        startPosition = (Vector3)args[1];
        targetRoot = (Transform)args[2];
        onCollect = (Action)args[3];

        delayMove = (float)args[4];

        uiResourceItem.SetQuantity(quantity);
        DOEffect().Forget();
    }

    public void OnReturnObj()
    {
    }

    public void Setup()
    {
    }
    #endregion

    protected virtual async UniTask DOEffect()
    {
        transform.DOKill();
        transform.position = startPosition;

        transform.DOScale(1, scaleDuration).From(0).SetEase(scaleCurve);
        await UniTask.Delay((int)(delayMove * 1000));

        uiResourceItem.transform.DOScale(0, scaleDownDuration).From(1);

        transform.DOMoveX(targetRoot.position.x, moveDuration).SetEase(moveXCurve);
        transform.DOMoveY(targetRoot.position.y, moveDuration).SetEase(moveYCurve).OnComplete(() =>
        {
            try
            {
                onCollect?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            CustomDestroy();
        });
        if (rotate)
        {
            transform.Rotate(Vector3.forward, UnityEngine.Random.Range(-360, 360));
            transform.DORotate(Vector3.zero, moveDuration, RotateMode.FastBeyond360).SetEase(Ease.OutBounce);
        }
    }

    private void CustomDestroy()
    {
        if (returnPool)
            SonatSystem.GetService<PoolingServiceAsync>().ReturnObj(this);
        else
            gameObject.SetActive(false);
    }
}
