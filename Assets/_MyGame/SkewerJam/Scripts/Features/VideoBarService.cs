using System;
using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.TimeManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Features.VideoBar
{
    [CreateAssetMenu(fileName = "VideoBarService", menuName = "MyGame/SkewerJam/Features/VideoBarService")]
    public class VideoBarService : SonatServiceSo, IServiceInitialize
    {
        private const string VIDEO_BAR_KEY = "VIDEO_BAR_KEY";
        [Header("Config")]
        [SerializeField] private VideoBarConfigSO _config;

        public VideoBarConfigSO Config => _config;
        public event Action<MilestoneData> OnClaimReward;
        public event Action OnResetData;
        public int CurrentIndex => _currentIndex.Value;

        private IntDataPref _currentIndex;
        private IntDataPref _claimedMilestoneIndex;
        private LongDataPref _expireTime;

        public void Initialize()
        {
            LoadData();

            if (CheckFull())
            {
                CheckExpire();
            }
        }


        private void LoadData()
        {
            _currentIndex = new IntDataPref(VIDEO_BAR_KEY + "_currentNum", -1);
            _claimedMilestoneIndex = new IntDataPref(VIDEO_BAR_KEY + "_claimedMilestoneNum", -1);

            _expireTime = new LongDataPref(VIDEO_BAR_KEY + "_expireTime");
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
            _claimedMilestoneIndex.Value = -1;

            OnResetData?.Invoke();
        }

        private void SetExpireTime()
        {
            _expireTime.Value = MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds() + _config.duration;
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
            _currentIndex.Value++;

            if (CheckFull())
            {
                SetExpireTime();
            }

            var currentMilestone = _config.milestones[_currentIndex.Value];
            _claimedMilestoneIndex.Value = currentMilestone.index;
            var log = new EarnResourceLogData
            {
                spendType = "video_bar",
                spendId = "video_bar"
            };
            MySonatFramework.GetService<InventoryService>().AddReward(currentMilestone.rewardData, log, false);
            OnClaimReward?.Invoke(currentMilestone);
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
    }
}