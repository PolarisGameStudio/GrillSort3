using System;
using Sirenix.OdinInspector;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.CheckInternet;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.SkewerJam.Pack
{
    public class UICustomShopPack : UIShopPackBase
    {
        [SerializeField] protected UnityEvent onBuySuccessAfterPopupReward;

        [Header("No Ads Icon")]
        [SerializeField] protected bool showNoAdsIcon = true;
        [SerializeField, ShowIf("showNoAdsIcon")] protected GameObject noAdsIcon;

        private readonly Service<CheckInternetService> checkInternetService = new();

        public ShopItemKey ShopItemKey => key;


        public override void SetPackData()
        {
            base.SetPackData();
            if (noAdsIcon != null && showNoAdsIcon)
            {
                noAdsIcon.SetActive(shopPack.noAds || shopPack.noAdsFree);
            }
        }

        protected override void OnBuyClick()
        {
            if (!checkInternetService.Instance.TryCheckInternet())
            {
                return;
            }

            base.OnBuyClick();
        }


        protected override void BuyComplete()
        {
            base.BuyComplete();
            PanelManager.Instance.OpenPanel<PopupReward>(
                new UIData()
            .Add(PopupReward.KEY_REWARD, shopPack.rewardData)
            .Add(PopupReward.KEY_ON_CLAIM_COMPLETE, (Action)(() =>
            {
                onBuySuccessAfterPopupReward?.Invoke();
            })));
        }
    }
}