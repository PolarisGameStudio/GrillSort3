using System;
using DG.Tweening;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

public class UIEffectDifficultyLevelInGame : MonoBehaviour, IPoolingObject
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Vector3 scaleDown = new Vector3(0.8f, 0.8f, 0.8f);

    public void Setup(Transform target, Action action = null)
    {
        transform.DOScale(scaleDown, duration);
        canvasGroup.DOFade(0.5f, duration);
        transform.DOJump(target.position, 1.75f, 1, duration).OnComplete(() =>
        {
            action?.Invoke();
            target.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            MySonatFramework.poolingServiceAsync.ReturnObj(this);
        });
    }

    public void Setup()
    {

    }

    public void OnCreateObj(params object[] args)
    {
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
    }

    public void OnReturnObj()
    {

    }
}
