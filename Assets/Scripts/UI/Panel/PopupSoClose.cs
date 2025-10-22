using Manager;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

public class PopupSoClose : PopupContinueBase
{
    [Header("PopupSoClose")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform pos1;
    [SerializeField] private RectTransform pos2;
    [SerializeField] private ProgressLoseController progressLose;
    public override void OnSetup()
    {
        base.OnSetup();

    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (progressLose.CheckActive())
        {
            panel.anchoredPosition = pos1.anchoredPosition;
        }
        else
        {
            panel.anchoredPosition = pos2.anchoredPosition;
        }

        if (playOnWithAdsBtn != null && playOnWithAdsBtn.activeSelf)
        {
            var activeProgressLose = GameRemoteConfigValue.activeProgressLose;
            playOnWithAdsBtn.SetActive(activeProgressLose == false);
        }

        MySonatFramework.GetService<AudioService>().StopMusic();
        MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Lose_OutOfMove_popup_Grill3);

    }

    public virtual void PlayOnWithCoinClick()
    {
        if (inventoryService.Instance.CanReduce(playOnPrice.resource, playOnPrice.quantity))
        {
            var log = new SpendResourceLogData()
            {
                earnType = "add_order",
                earnId = "order",
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

    public void OnClickPlayOn(string by = "play_on_add_trays")
    {
        PlayOn(by);
    }
}