using System;
using System.Linq;
using MyGame.SkewerJam.Objects.Entities;
using MyGame.SkewerJam.Utils;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using TMPro;
using UnityEngine;

public class PopupHightlightGameplay : Panel
{
    public const string BOOSTER_KEY = "BoosterKey";
    public const string ON_CLOSE = "OnClose";
    public const string ON_SELECT_ITEM = "OnSelectItem";

    [Serializable]
    public class BoosterAndObject
    {
        public GameResource boosterType;
        public GameObject objectToHighlight;
    }
    [SerializeField] private BoosterAndObject[] boosterAndObjects;
    [SerializeField] private Canvas bgCanvas;
    [SerializeField] private Canvas guideCanvas;
    [SerializeField] private TMP_Text contentText;

    private Action onClose;
    private Action<OrderEntity> onSelectItem;

    public override void Open(UIData data)
    {
        base.Open(data);
        bgCanvas.sortingLayerName = LayerManager.TopUI;
        bgCanvas.sortingOrder = 50;

        guideCanvas.sortingLayerName = LayerManager.TopUI;
        guideCanvas.sortingOrder = 51;

        HideAllObjects();
        if (data != null)
        {
            onClose = data.TryGet<Action>(ON_CLOSE, out var action) ? action : null;
            if (!data.TryGet<Action<OrderEntity>>(ON_SELECT_ITEM, out onSelectItem)) onSelectItem = null;

            if (data.TryGet<GameResource>(BOOSTER_KEY, out var boosterType))
            {
                var boosterAndObject = boosterAndObjects.FirstOrDefault(e => e.boosterType == boosterType);
                if (boosterAndObject != null)
                {
                    boosterAndObject.objectToHighlight.SetActive(true);
                }
            }
        }
    }

    private void HideAllObjects()
    {
        foreach (var boosterAndObject in boosterAndObjects)
        {
            boosterAndObject.objectToHighlight.SetActive(false);
        }
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
                        onSelectItem?.Invoke(order);
                        base.Close();
                        break;
                    }
                }
            }
        }
    }

    public override void Close()
    {
        onClose?.Invoke();
        base.Close();
    }
}