using MyGame.Modules.CardCollection;
using MyGame.Modules.CardCollection.CardStarExchange;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupExchangeCardStar : Panel
{
    [SerializeField] private TMP_Text txtStar;
    [SerializeField] private RectTransform rootDesc;
    [SerializeField] private ChestController[] chestControllers;
    private readonly Service<CardCollectionService> _cardCollectionService = new();

    private int star = 0;

    private void OnEnable()
    {
        var starModule = _cardCollectionService.Instance.StarSubmodule;
        starModule.OnStarChanged += OnStarChanged;
    }

    private void OnDisable()
    {
        var starModule = _cardCollectionService.Instance.StarSubmodule;
        starModule.OnStarChanged -= OnStarChanged;
    }

    private void OnStarChanged(int star)
    {
        UpdateUI();
    }

    public override void OnSetup()
    {
        base.OnSetup();

        for (int i = 0; i < chestControllers.Length; i++)
        {
            chestControllers[i].SetData(i);
        }

    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        UpdateUI();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootDesc);
    }

    private void UpdateUI()
    {
        var starModule = _cardCollectionService.Instance.StarSubmodule;
        star = starModule.NumberStar;

        txtStar.text = star.ToString();

        for (int i = 0; i < chestControllers.Length; i++)
        {
            chestControllers[i].UpdateUI();
        }
    }
}
