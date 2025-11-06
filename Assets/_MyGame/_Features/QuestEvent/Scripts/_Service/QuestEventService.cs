using System;
using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.TimeManagement;
using SonatFramework.Systems.UserData;
using UnityEngine;

namespace MyGame.Modules.QuestEvent
{
    [CreateAssetMenu(fileName = "QuestEventService", menuName = "MyGame/SkewerJam/Features/QuestEvent/QuestEventService")]
    public class QuestEventService : BaseExpireService
    {
        public override string DATA_KEY => "QUEST_EVENT";
        public const string ITEM_NAME = "quest_item";

        public QuestEventConfigSO config;

        private IntDataPref _currentItem;
        private IntDataPref _claimedQuestIndex;

        public int CurrentItem => _currentItem.Value;
        public int ClaimedQuestIndex => _claimedQuestIndex.Value;

        private int numItemInGame = 0;

        public override void Initialize()
        {
            base.Initialize();

            new EventBinding<LevelStartedEvent>(OnLevelStarted);
            new EventBinding<LevelEndedEvent>(OnLevelEnded);
        }

        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            numItemInGame = 0;
        }

        private void OnLevelEnded(LevelEndedEvent eventData)
        {
            if (eventData.success)
            {
                _currentItem.Value += numItemInGame;
            }
        }

        public void AddNumItemInGame(int num)
        {
            numItemInGame += num;
        }

        #region implement
        protected override void LoadData()
        {
            base.LoadData();

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

        public int GetCurrentQuestIndexView()
        {
            return _claimedQuestIndex.Value + 1;
        }

        public bool CheckCanClaimQuest()
        {
            var currentQuestIndex = GetCurrentQuestIndexView();
            var itemRequired = config.listMilestones[currentQuestIndex].numItem;
            return _currentItem.Value >= itemRequired;
        }

        public void ClaimQuest()
        {
            var currentQuestIndex = GetCurrentQuestIndexView();
            var itemRequired = config.listMilestones[currentQuestIndex].numItem;

            _currentItem.Value -= itemRequired;
            _claimedQuestIndex.Value += 1;

            var rewardData = config.listMilestones[currentQuestIndex].rewardData;
            var log = new EarnResourceLogData
            {
                spendType = "feature",
                spendId = "quest_event"
            };
            MySonatFramework.GetService<InventoryService>().AddReward(rewardData, log);

            // ui
            var uiData = new UIData();
            uiData.Add(PopupReward.REWARD_KEY, rewardData);
            PanelManager.Instance.OpenPanel<PopupReward>(uiData);

        }
    }
}