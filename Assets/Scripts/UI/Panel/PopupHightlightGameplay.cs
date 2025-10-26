using System;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Booster;
using MyGame.SkewerJam.Objects.Entities;
using MyGame.SkewerJam.Utils;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static MyGame.SkewerJam.Objects.Entities.OrderEntity;

public class PopupHightlightGameplay : Panel
{
    public const string BOOSTER_KEY = "BoosterKey";
    public const string ON_CLOSE = "OnClose";
    public const string ON_SELECT_ITEM = "OnSelectItem";

    [SerializeField] private Canvas bgCanvas;
    [SerializeField] private RectTransform handSuggest;

    private Action onClose;
    private Action<OrderEntity> onSelectItem;

    private GameResource boosterType;
    private UIBooster uiBooster;

    public override void Open(UIData data)
    {
        base.Open(data);
        bgCanvas.sortingLayerName = LayerManager.TopUI;
        bgCanvas.sortingOrder = 50;

        // guideCanvas.sortingLayerName = LayerManager.TopUI;
        // guideCanvas.sortingOrder = 51;

        if (data != null)
        {
            onClose = data.TryGet<Action>(ON_CLOSE, out var action) ? action : null;
            if (!data.TryGet<Action<OrderEntity>>(ON_SELECT_ITEM, out onSelectItem)) onSelectItem = null;

            if (data.TryGet<GameResource>(BOOSTER_KEY, out var boosterType))
            {
                this.boosterType = boosterType;
                uiBooster = GameController.Instance.GameplayScreen.GetUIBooster(boosterType);
                if (uiBooster != null)
                {
                    uiBooster.SetSortingOrder(true, LayerManager.TopUI, 51);
                }
            }
        }

        var boosterManager = GameController.Instance.GameLogicHandler.BoosterManager;
        if (boosterManager.IsForceUseBooster(boosterType))
        {
            ShowSuggest(true);
        }
        else
        {
            ShowSuggest(false);
        }
    }

    private void ShowSuggest(bool show)
    {
        handSuggest.gameObject.SetActive(show);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.transform.TryGetComponent<OrderEntity>(out var order))
                    {
                        if (order.IsActive == false || order.State == OrderEntityState.Complete) continue;

                        UseSuccess(order);
                        break;
                    }
                }
            }
        }
    }

    private void UseSuccess(OrderEntity order)
    {
        onSelectItem?.Invoke(order);

        Close();
    }

    public void OnClickClose()
    {
        var boosterManager = GameController.Instance.GameLogicHandler.BoosterManager;
        if (boosterManager.IsForceUseBooster(boosterType))
        {
            return;
        }
        onClose?.Invoke();
        Close();
    }

    public override void Close()
    {
        base.Close();
    }
}