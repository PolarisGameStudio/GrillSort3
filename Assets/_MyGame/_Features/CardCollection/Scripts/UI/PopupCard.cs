using System;
using DG.Tweening;
using MyGame.Modules.CardCollection;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using UnityEngine;

public class PopupCard : Panel
{
    public const string CARD_TYPE_KEY = "cardType";
    public const string POSITION_KEY = "position";
    public const string ON_COMPLETE_CLOSE_KEY = "onCompleteClose";

    [SerializeField] private UICard card;
    [SerializeField] private float scaleUp = 2;
    [SerializeField] private float durationScaleUp = 0.5f;
    [SerializeField] private AnimationCurve curveScaleUp;
    [SerializeField] private AnimationCurve curveMoveUp;

    [SerializeField] private float scaleDown = 1f;
    [SerializeField] private float durationScaleDown = 0.5f;
    [SerializeField] private AnimationCurve curveScaleDown;
    [SerializeField] private AnimationCurve curveMoveDown;

    private CardType cardType;
    private Vector3 startPos;
    private Vector3 startLocalPos;
    private Action onCompleteClose;

    private readonly Service<CardCollectionService> _cardCollectionService = new();

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        cardType = uiData.Get<CardType>(CARD_TYPE_KEY);
        startPos = uiData.Get<Vector3>(POSITION_KEY);
        onCompleteClose = uiData.Get<Action>(ON_COMPLETE_CLOSE_KEY);
        card.Setup(cardType);
        card.UpdateData();
        PlayAppearAnimation();

        // var inventoryModule = _cardCollectionService.Instance.CardInventoryModule;
        // inventoryModule.RemoveNewCard(cardType);
    }

    private void PlayAppearAnimation()
    {
        card.transform.DOKill();
        card.transform.position = startPos;
        startLocalPos = card.transform.localPosition;
        card.transform.localScale = Vector3.one;
        card.transform.DOScale(scaleUp, durationScaleUp).SetEase(curveScaleUp);
        card.transform.DOLocalMove(Vector3.zero, durationScaleUp).SetEase(curveMoveUp);
    }

    private void PlayDisappearAnimation()
    {
        card.transform.DOKill();
        card.transform.DOScale(scaleDown, durationScaleDown).SetEase(curveScaleDown);
        card.transform.DOLocalMove(startLocalPos, durationScaleDown).SetEase(curveMoveDown);
    }

    private bool isClose = false;
    public override void Close()
    {
        if (isClose) return;
        isClose = true;

        PlayDisappearAnimation();

        SonatUtils.DelayCall(durationScaleDown, () =>
        {
            onCompleteClose?.Invoke();
            base.Close();
        });
    }

    public void OnClickSend()
    {
        PopupToast.Cretate("Coming soon");
    }
}
