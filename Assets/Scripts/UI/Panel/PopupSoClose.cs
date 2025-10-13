using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.InventoryManagement;
public class PopupSoClose : PopupContinueBase
{
    public override void OnSetup()
    {
        base.OnSetup();
    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);


        MySonatFramework.GetService<AudioService>().StopMusic();
        MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Lose_OutOfMove_popup_Grill3);

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