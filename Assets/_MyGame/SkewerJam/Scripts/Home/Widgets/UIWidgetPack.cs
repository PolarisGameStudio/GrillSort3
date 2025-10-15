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

        shopService.Instance.OnBuySuccess += OnBuySuccess;
    }

    private void OnDestroy()
    {
        shopService.Instance.OnBuySuccess -= OnBuySuccess;
    }

    private void OnBuySuccess(ShopItemKey shopItemKey)
    {
        if (shopItemKey != this.shopItemKey) return;

        if (shopService.Instance.VerifyPack(shopItemKey) == false)
        {
            gameObject.SetActive(false);
        }

    }


}