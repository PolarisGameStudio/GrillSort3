using System;
using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.TimeManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Features.VideoBar
{
    public abstract class VideoBarService : SonatServiceSo, IServiceInitialize
    {
        protected abstract string VIDEO_BAR_KEY { get; }

        [Header("Config")]
        [SerializeField] private VideoBarConfigSO _config;

        public VideoBarConfigSO Config => _config;
        public event Action<MilestoneData> OnClaimReward;
        public event Action OnResetData;
        public int CurrentIndex => _currentIndex.Value;

        private IntDataPref _currentIndex;
        // private IntDataPref _claimedMilestoneIndex;
        private LongDataPref _expireTime;
        private LongDataPref _nextWatchTime;

        public void Initialize()
        {
            LoadData();

            CheckExpire();
        }


        private void LoadData()
        {
            _currentIndex = new IntDataPref(VIDEO_BAR_KEY + "_currentNum", -1);
            // _claimedMilestoneIndex = new IntDataPref(VIDEO_BAR_KEY + "_claimedMilestoneNum", -1);

            _expireTime = new LongDataPref(VIDEO_BAR_KEY + "_expireTime");
            _nextWatchTime = new LongDataPref(VIDEO_BAR_KEY + "_nextWatchTime");
        }

        private async UniTask Countdown(long remainTime, Action onComplete = null)
        {
            while (remainTime > 0)
            {
                await UniTask.Delay(1000);
                remainTime--;
            }
            onComplete?.Invoke();
        }

        private void ResetData()
        {
            _currentIndex.Value = -1;
            // _claimedMilestoneIndex.Value = -1;
            _nextWatchTime.Value = 0;
            SetExpireTime();

            OnResetData?.Invoke();
        }

        private void SetExpireTime()
        {
            var now = MySonatFramework.GetService<TimeService>().GetCurrentTime();
            var nextDay = now.Date.AddDays(1);
            _expireTime.Value = ((DateTimeOffset)nextDay).ToUnixTimeSeconds();
            CheckExpire();
        }

        public bool CheckFull()
        {
            return _currentIndex.Value + 1 >= _config.milestones.Count;
        }

        public long GetRemainingTime()
        {
            return _expireTime.Value - MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
        }

        public void OnWatchedVideo()
        {
            if (CanWatchAds() == false) return;
            _currentIndex.Value++;

            var currentMilestone = _config.milestones[_currentIndex.Value];
            // _claimedMilestoneIndex.Value = currentMilestone.index;
            var log = new EarnResourceLogData
            {
                spendType = "video_bar",
                spendId = "video_bar"
            };
            MySonatFramework.GetService<InventoryService>().AddReward(currentMilestone.rewardData, log, false);

            if (CheckFull() == false)
            {
                UpdateNextWatchTime();
            }
            OnClaimReward?.Invoke(currentMilestone);
        }

        private void UpdateNextWatchTime()
        {
            var now = MySonatFramework.GetService<TimeService>().GetCurrentTime();
            _nextWatchTime.Value = ((DateTimeOffset)now.AddSeconds(_config.milestones[_currentIndex.Value + 1].duration)).ToUnixTimeSeconds();
        }

        private void CheckExpire()
        {
            var remainTime = _expireTime.Value - MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
            if (remainTime > 0)
            {
                Countdown(remainTime, ResetData).Forget();
            }
            else
            {
                ResetData();
            }
        }

        public long GetNextWatchTime()
        {
            return _nextWatchTime.Value - MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
        }

        public bool CanWatchAds()
        {
            return GetNextWatchTime() <= 0 && CheckFull() == false;
        }
    }
}