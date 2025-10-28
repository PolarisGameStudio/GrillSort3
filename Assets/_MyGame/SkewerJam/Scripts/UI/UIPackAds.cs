using System;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.CheckInternet;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;
using UnityEngine.Events;

public class UIPackAds : MonoBehaviour
{
    [SerializeField] private UnityEvent onWatchedAds;

    private readonly Service<CheckInternetService> checkInternetService = new();
    // private LongDataPref _lastWatchedAdsTime;

    private void Awake()
    {
        // _lastWatchedAdsTime = new LongDataPref("last_watched_ads_time");
    }

    public void OnClickWatchAds()
    {
        if (!checkInternetService.Instance.TryCheckInternet())
        {
            return;
        }

        if (CheckCanWatchAds())
        {
            SonatSDKAdapter.ShowRewardAds(OnWatchedAds, "x2_coin_win", "x2_coin_win");

        }
        else
        {
            PopupToast.Cretate("Come back tomorrow");
        }
    }

    private bool CheckCanWatchAds()
    {
        return true;
        // return _lastWatchedAdsTime.Value == 0
        // || MySonatFramework.GetService<TimeService>().GetCurrentTime().Date != DateTimeOffset.FromUnixTimeSeconds(_lastWatchedAdsTime.Value).Date;
    }

    private void OnWatchedAds()
    {
        // _lastWatchedAdsTime.Value = MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
        // TODO: Implement watched ads
        var reward = new RewardData();
        reward.AddReward(new ResourceData(GameResource.Coin, 30));
        var log = new EarnResourceLogData()
        {
            spendType = "rwd_ads_free",
            spendId = "rwd_ads_free",
            source = "non_iap"
        };
        MySonatFramework.GetService<InventoryService>().AddReward(reward, log, false);

        onWatchedAds?.Invoke();
        PanelManager.Instance.OpenPanel<PopupReward>(new UIData().Add(PopupReward.KEY_REWARD, reward));
    }
}