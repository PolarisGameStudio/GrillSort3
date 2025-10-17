using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.UI.PopupVideoBar;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Systems.TimeManagement;
using UnityEngine;
using UnityEngine.UI;

public class PopupVideoBar : Panel
{
    private const string VIDEO_BAR_KEY = "VIDEO_BAR_KEY";
    [Header("Config")]
    [SerializeField] private VideoBarConfigSO _config;

    [Header("Progress Bar")]
    [SerializeField] private Slider _slider;
    [SerializeField] private Transform _milestoneContainer;
    [SerializeField] private UITimeCounter timeCounter;

    [Header("Delay")]
    [SerializeField] private float _delay = 0.3f;
    [SerializeField] private float _duration = 0.5f;
    private IntDataPref _currentNum;
    private IntDataPref _claimedMilestoneNum;
    private IntDataPref _currentNumVisual;
    private LongDataPref _expireTime;

    private bool _collected;
    private int _maxNumber;

    private readonly Service<PoolingContainerService> poolingContainer = new();

    private void Reset()
    {
        _collected = false;
    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        Reset();
        LoadData();

        _maxNumber = _config.milestones.Count;
        poolingContainer.Instance.CleanContainer(_milestoneContainer);
        for (int i = 0; i < _config.milestones.Count; i++)
        {
            var milestone = poolingContainer.Instance.CreateObject<UIMilestone>(_milestoneContainer);
            milestone.SetData(_config.milestones[i].index + 1, (_config.milestones[i].index + 1) * 1.0f / _maxNumber, _config.milestones[i].rewardData);
        }



        if (CheckFull())
        {
            CheckExpire();
        }
        else
        {
            timeCounter.gameObject.SetActive(false);
        }

        _slider.value = (_currentNumVisual.Value + 1) * 1.0f / _maxNumber;
    }


    private void LoadData()
    {
        _currentNum = new IntDataPref(VIDEO_BAR_KEY + "_currentNum", -1);
        _claimedMilestoneNum = new IntDataPref(VIDEO_BAR_KEY + "_claimedMilestoneNum", -1);
        _currentNumVisual = new IntDataPref(VIDEO_BAR_KEY + "_currentNumVisual", -1);

        _expireTime = new LongDataPref(VIDEO_BAR_KEY + "_expireTime");
    }

    private IEnumerator Countdown(long remainTime, Action onComplete = null)
    {
        while (remainTime > 0)
        {
            yield return new WaitForSeconds(1);
            remainTime--;
        }
        onComplete?.Invoke();
    }

    private void ResetData()
    {
        _currentNum.Value = 0;
        _claimedMilestoneNum.Value = -1;
        _currentNumVisual.Value = 0;

        timeCounter.gameObject.SetActive(false);
        _slider.value = (_currentNumVisual.Value + 1) * 1.0f / _maxNumber;
    }

    public void OnClickWatchAds()
    {
        if (CheckFull())
        {
            PopupToast.Cretate("Come back later!");
            return;
        }

        if (_collected) return;
        if (SonatSDKAdapter.IsRewardAdsReady())
        {
            _collected = true;
            SonatSDKAdapter.ShowRewardAds(OnWatchedVideo, "x2_coin_win", "x2_coin_win");
        }
        else
        {
            PopupToast.Cretate("No video available!");
        }
    }

    private bool CheckFull()
    {
        return _currentNum.Value + 1 >= _maxNumber;
    }

    private void OnWatchedVideo()
    {
        _currentNum.Value++;

        if (CheckFull())
        {
            _expireTime.Value = MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds() + _config.duration;
            CheckExpire();
        }

        // var nextMildestone = _config.milestones[_currentNum.Value];
        // if (_currentNum.Value == nextMildestone.index)
        // {
        var currentMilestone = _config.milestones[_currentNum.Value];
        Claim(currentMilestone);
        // }
    }

    private void CheckExpire()
    {
        var remainTime = _expireTime.Value - MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
        if (remainTime > 0)
        {
            timeCounter.gameObject.SetActive(true);
            timeCounter.SetData(remainTime);
            StartCoroutine(Countdown(remainTime, ResetData));
        }
        else
        {
            ResetData();
        }
    }

    private void Claim(MilestoneData currentMilestone)
    {
        _claimedMilestoneNum.Value = currentMilestone.index;
        var log = new EarnResourceLogData
        {
            spendType = "video_bar",
            spendId = "video_bar"
        };
        MySonatFramework.GetService<InventoryService>().AddReward(currentMilestone.rewardData, log, false);

        UpdateUI(() =>
        {
            _collected = false;
            PanelManager.Instance.OpenPanel<PopupReward>(new UIData().Add(PopupReward.KEY_REWARD, currentMilestone.rewardData));
        }).Forget();
    }

    private async UniTask UpdateUI(Action onComplete)
    {
        await UniTask.Delay((int)_delay * 1000);
        var current = (_currentNumVisual.Value + 1) * 1.0f / _maxNumber;
        var newValue = (_currentNum.Value + 1) * 1.0f / _maxNumber;
        await _slider.DOValue(newValue, _duration).From(current);
        _currentNumVisual.Value = _currentNum.Value;
        onComplete?.Invoke();
    }
}
