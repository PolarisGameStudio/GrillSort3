using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyGame.SkewerJam.Features.VideoBar;
using MyGame.UI.PopupVideoBar;
using SonatFramework.Scripts.Feature.CheckInternet;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;
using UnityEngine.UI;

public class PopupVideoBar : Panel
{
    private const string VIDEO_BAR_KEY = "VIDEO_BAR_KEY";

    [Header("Progress Bar")]
    [SerializeField] private Slider _slider;
    [SerializeField] private Transform _milestoneContainer;
    [SerializeField] private UITimeCounter timeCounter;

    [Header("Delay")]
    [SerializeField] private float _delay = 0.3f;
    [SerializeField] private float _duration = 0.5f;

    private IntDataPref _currentVisualIndex;

    private bool _collected;
    private List<UIMilestone> _milestoneList = new List<UIMilestone>();

    private readonly Service<VideoBarService> videoBarService = new();
    private readonly Service<PoolingContainerService> poolingContainer = new();
    private readonly Service<CheckInternetService> checkInternetService = new();
    private Coroutine coroutine;
    private void Reset()
    {
        _collected = false;
    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        Reset();
        LoadData();
        _slider.value = (_currentVisualIndex.Value + 1) * 1.0f / videoBarService.Instance.Config.milestones.Count;

        poolingContainer.Instance.CleanContainer(_milestoneContainer);
        var config = videoBarService.Instance.Config;
        _milestoneList.Clear();
        for (int i = 0; i < config.milestones.Count; i++)
        {
            var milestone = poolingContainer.Instance.CreateObject<UIMilestone>(_milestoneContainer);
            milestone.SetData(config.milestones[i].index + 1, (config.milestones[i].index + 1) * 1.0f / config.milestones.Count, config.milestones[i].rewardData);
            _milestoneList.Add(milestone);
        }

        timeCounter.gameObject.SetActive(true);
        timeCounter.SetData(videoBarService.Instance.GetRemainingTime() + 1, () => // cộng thêm 1s cho chắc là service đã reset
        {
            Close();
        });

        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(UpdateUI(() => { }));

        videoBarService.Instance.OnClaimReward += OnClaimReward;
        // videoBarService.Instance.OnResetData += ResetData;
    }

    public override void Close()
    {
        base.Close();
        videoBarService.Instance.OnClaimReward -= OnClaimReward;
        // videoBarService.Instance.OnResetData -= ResetData;
    }

    private void LoadData()
    {
        _currentVisualIndex = new IntDataPref(VIDEO_BAR_KEY + "_currentNumVisual", -1);

        if (_currentVisualIndex.Value > videoBarService.Instance.CurrentIndex)
        {
            _currentVisualIndex.Value = -1;
        }
    }

    private void OnClaimReward(MilestoneData milestoneData)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(UpdateUI(() =>
        {
            _collected = false;
            PanelManager.Instance.OpenPanel<PopupReward>(new UIData().Add(PopupReward.KEY_REWARD, milestoneData.rewardData));
        }));
    }

    private void ResetData()
    {
        _currentVisualIndex.Value = -1;

        var config = videoBarService.Instance.Config;
        _slider.value = (_currentVisualIndex.Value + 1) * 1.0f / config.milestones.Count;
        foreach (var milestone in _milestoneList)
        {
            milestone.SetComplete(false);
        }
    }

    public void OnClickWatchAds()
    {
        if (videoBarService.Instance.CheckFull())
        {
            PopupToast.Cretate("Come back later!");
            return;
        }

        if (_collected) return;

        if (MySonatFramework.IsRewardAdsReady())
        {
            _collected = true;
            SonatSDKAdapter.ShowRewardAds(OnWatchedVideo, "x2_coin_win", "x2_coin_win");
        }
        else
        {
            PopupToast.Cretate("No video available!");
        }
    }

    private void OnWatchedVideo()
    {
        videoBarService.Instance.OnWatchedVideo();
    }

    private IEnumerator UpdateUI(Action onComplete)
    {
        for (int i = 0; i < videoBarService.Instance.Config.milestones.Count; i++)
        {
            var milestoneObj = _milestoneList[i];
            milestoneObj.SetComplete(videoBarService.Instance.CurrentIndex >= i);
        }
        yield return new WaitForSeconds(_delay);
        var current = (_currentVisualIndex.Value + 1) * 1.0f / videoBarService.Instance.Config.milestones.Count;

        var currentIndex = videoBarService.Instance.CurrentIndex;
        var newValue = (currentIndex + 1) * 1.0f / videoBarService.Instance.Config.milestones.Count;
        _slider.DOValue(newValue, _duration).From(current);

        yield return new WaitForSeconds(_duration);
        _currentVisualIndex.Value = currentIndex;
        onComplete?.Invoke();
    }
}
