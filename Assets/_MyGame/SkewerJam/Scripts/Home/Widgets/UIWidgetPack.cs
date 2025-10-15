using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop;
using SonatFramework.Systems;
using UnityEngine;

public class UIWidgetPack : UIHomeWidget
{
    [SerializeField] private ShopItemKey shopItemKey;

    private readonly Service<ShopService> shopService = new();

    public override void Setup()
    {
        base.Setup();

        if (shopService.Instance.VerifyPack(shopItemKey) == false)
        {
            active = false;
        }

        gameObject.SetActive(active);
    }
}