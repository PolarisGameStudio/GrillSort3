using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.TrackingModule;
using UnityEngine;

public class PopupSoClose : PopupContinueBase
{
    public override void OnSetup()
    {
        base.OnSetup();
    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

    }

    public virtual void PlayOnWithCoinClick()
    {
        if (inventoryService.Instance.CanReduce(playOnPrice.resource, playOnPrice.quantity))
        {
            var log = new SpendResourceLogData()
            {
                earnType = "add_trays",
                earnId = "revive",
            };
            inventoryService.Instance.ReduceResource(playOnPrice.resource, playOnPrice.quantity,
                log);
            PlayOn("play_on_add_trays");
        }
        else
        {
            PopupToast.Cretate("Not enough coin!");
            PanelManager.Instance.OpenPanelByName<ShopPanelBase>("ShopPanel");
        }
    }

    public override void PlayOnWithAdsClick()
    {
        // SonatSDKAdapter.ShowRewardAds(OnReviveWithAds, "booster", "revive");
        OnReviveWithAds();
    }

    protected override void OnReviveWithAds()
    {
        PlayOn("play_on_add_trays");
    }
}