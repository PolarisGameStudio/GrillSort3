using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Utils;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using UnityEngine;

public class PopupForceBooster : Panel
{
    public const string BOOSTER_TYPE_KEY = "BOOSTER_TYPE_KEY";

    [SerializeField] private RectTransform hand;
    [SerializeField] private Canvas canvas;

    private GameResource boosterType;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        canvas.sortingLayerName = LayerManager.TopUI;
        canvas.sortingOrder = 50;

        if (uiData.TryGet(BOOSTER_TYPE_KEY, out boosterType))
        {
            var uiBooster = GameController.Instance.GameplayScreen.GetUIBooster(boosterType);
            uiBooster.SetSortingOrder(true, LayerManager.TopUI, 51);
            hand.position = uiBooster.transform.position;
        }
    }
}