using System;
using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems.TimeManagement;
using SonatFramework.Systems.UserData;
using UnityEngine;

namespace MyGame.Modules.QuestEvent
{
    [CreateAssetMenu(fileName = "QuestEventService", menuName = "MyGame/SkewerJam/Features/QuestEvent/QuestEventService")]
    public class QuestEventService : BaseExpireService
    {
        public override string DATA_KEY => "QUEST_EVENT";

        public QuestEventConfigSO config;

        private IntDataPref _currentQuestIndex;
        private IntDataPref _currentItem;
        private IntDataPref _claimedQuestIndex;

        public int CurrentQuestIndex => _currentQuestIndex.Value;
        public int CurrentItem => _currentItem.Value;
        public int ClaimedQuestIndex => _claimedQuestIndex.Value;

        #region implement
        protected override void LoadData()
        {
            base.LoadData();

            _currentQuestIndex = new IntDataPref(DATA_KEY + "_currentQuestIndex", 0);
            _currentItem = new IntDataPref(DATA_KEY + "_currentItem", 0);
            _claimedQuestIndex = new IntDataPref(DATA_KEY + "_claimedQuestIndex", -1);
        }

        public override bool CanUnlock()
        {
            var level = MySonatFramework.GetService<UserDataService>().GetLevel();
            return level >= config.unlocklevel;
        }

        protected override long GetNextExpireTime()
        {
            var now = MySonatFramework.GetService<TimeService>().GetCurrentTime();
            var nextDay = now.Date.AddDays(7);
            return ((DateTimeOffset)nextDay).ToUnixTimeSeconds();
        }

        protected override void ProgressUnlockFeature()
        {
            // throw new NotImplementedException();
        }

        protected override async UniTask TryShowTutorial()
        {
            // // Hiện tut
            // if (PlayerPrefs.HasKey($"{DATA_KEY}_ShowTutorial") == false)
            // {
            //     HomeManager.Instance.BlockUI();
            //     PlayerPrefs.SetInt($"{DATA_KEY}_ShowTutorial", 1);
            // }
        }

        #endregion

        public float GetCurrentProgress()
        {
            var cur = _currentItem.Value;
            var max = config.listMilestones[_currentQuestIndex.Value].numItem;
            return cur * 1.0f / max;
        }
    }
}