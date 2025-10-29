using System;
using MyGame.SkewerJam.Features.VideoBar;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPackAds : MonoBehaviour
{
    [SerializeField] private Button btnWatchAds;
    [SerializeField] private Button btnTime;
    [SerializeField] private UITimeCounter timeCounter;
    [SerializeField] private UIRewardItem uiRewardItem;

    [SerializeField] private UnityEvent onWatchedAds;

    private readonly Service<VideoBarServiceInShop> videoBarServiceInShop = new();

    private bool _collected;

    void OnEnable()
    {
        videoBarServiceInShop.Instance.OnClaimReward += OnClaimReward;
        videoBarServiceInShop.Instance.OnResetData += OnResetData;
        UpdateUI();
    }

    void OnDisable()
    {
        videoBarServiceInShop.Instance.OnClaimReward -= OnClaimReward;
        videoBarServiceInShop.Instance.OnResetData -= OnResetData;
    }

    public void OnClickTime()
    {
        PopupToast.Cretate("Come back later!");
        return;
    }

    public void OnClickWatchAds()
    {
        if (videoBarServiceInShop.Instance.CanWatchAds() == false)
        {
            PopupToast.Cretate("Come back later!");
            return;
        }

        if (_collected) return;

        if (MySonatFramework.IsRewardAdsReady())
        {
            _collected = true;
            SonatSDKAdapter.ShowRewardAds(OnWatchedVideo, "free_coin_in_shop", "free_coin_in_shop");
        }
        else
        {
            PopupToast.Cretate("No video available!");
        }
    }

    private void OnWatchedVideo()
    {
        videoBarServiceInShop.Instance.OnWatchedVideo();
    }

    private void OnResetData()
    {
        UpdateUI();
    }

    private void OnClaimReward(MilestoneData milestoneData)
    {
        onWatchedAds?.Invoke();
        UpdateUI(() =>
        {
            _collected = false;
            PanelManager.Instance.OpenPanel<PopupReward>(new UIData().Add(PopupReward.KEY_REWARD, milestoneData.rewardData));
        });
    }

    private void UpdateUI(Action onComplete = null)
    {
        _collected = false;

        if (videoBarServiceInShop.Instance.CheckFull() == false)
        {
            var currentIndex = videoBarServiceInShop.Instance.CurrentIndex;
            var rewardData = videoBarServiceInShop.Instance.Config.milestones[currentIndex + 1].rewardData;
            uiRewardItem.Init(rewardData.resourceDatas[0].resource, rewardData.resourceDatas[0].quantity);

            if (videoBarServiceInShop.Instance.CanWatchAds())
            {
                btnWatchAds.gameObject.SetActive(true);
                btnTime.gameObject.SetActive(false);
            }
            else
            {
                btnWatchAds.gameObject.SetActive(false);
                btnTime.gameObject.SetActive(true);
                timeCounter.SetData(videoBarServiceInShop.Instance.GetNextWatchTime(), () =>
                {
                    UpdateUI();
                });
            }
        }
        else
        {
            var milestones = videoBarServiceInShop.Instance.Config.milestones;
            var rewardData = milestones[milestones.Count - 1].rewardData;
            uiRewardItem.Init(rewardData.resourceDatas[0].resource, rewardData.resourceDatas[0].quantity);

            btnWatchAds.gameObject.SetActive(false);
            btnTime.gameObject.SetActive(true);

            var remainTime = videoBarServiceInShop.Instance.GetRemainingTime() + 1;
            if (remainTime > 0)
            {
                timeCounter.SetData(remainTime);
            }
        }
        onComplete?.Invoke();
    }
}