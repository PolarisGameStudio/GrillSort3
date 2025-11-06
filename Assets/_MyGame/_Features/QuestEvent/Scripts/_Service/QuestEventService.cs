using System;
using Cysharp.Threading.Tasks;
using MyGame.Modules.SubInventory;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
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

        public event Action OnDataUpdated;

        private int _numCollectAtHome = 0;

        public override void Initialize()
        {
            base.Initialize();

            new EventBinding<LevelStartedEvent>(OnLevelStarted);
            new EventBinding<LevelEndedEvent>(OnLevelEnded);
        }

        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
        }

        private void OnLevelEnded(LevelEndedEvent eventData)
        {
            if (eventData.success)
            {
                _numCollectAtHome = MySonatFramework.GetService<SubInventoryService>().GetResource(SubGameResource.QuestEventItem);
                _currentItem.Value += _numCollectAtHome;
            }
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
        }

        public void AddNumItemInGame(int num, Vector3 position)
        {
            MySonatFramework.GetService<SubInventoryService>().AddResource(SubGameResource.QuestEventItem, num);
            EventBus<AddSubItemEvent>.Raise(new AddSubItemEvent()
            {
                resource = SubGameResource.QuestEventItem,
                quantity = 1,
                position = position,
                collectEffect = new CollectEffectMultiple()
                {
                    collectEffectName = "UICollectEffectSubItem"
                }
            });
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
            if (_numCollectAtHome > 0)
            {
                EventBus<ForceEffectSubItemEvent>.Raise(new ForceEffectSubItemEvent()
                {
                    resource = SubGameResource.QuestEventItem,
                    quantity = _numCollectAtHome,
                    position = Vector3.zero,
                    collectEffect = new CollectEffectMultiple()
                    {
                        collectEffectName = "UICollectEffectSubItem_AtHome"
                    }
                });

                _numCollectAtHome = 0;

                SonatUtils.DelayCall(2f, () =>
                {
                    if (CheckCanClaimQuest())
                    {
                        PanelManager.Instance.OpenPanel<PopupQuestEvent>();
                    }
                });
            }
        }

        protected override async UniTask TryShowTutorial()
        {
            // throw new NotImplementedException();
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
            if (!CheckCanClaimQuest())
            {
                return;
            }
            var currentQuestIndex = GetCurrentQuestIndexView();
            var itemRequired = config.listMilestones[currentQuestIndex].numItem;

            _currentItem.Value -= itemRequired;
            _claimedQuestIndex.Value += 1;
            OnDataUpdated?.Invoke();

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

        public bool CheckCompleteAllQuest()
        {
            return _claimedQuestIndex.Value >= config.listMilestones.Count - 1;
        }

        public int GetCurrentItemView()
        {
            return _currentItem.Value - _numCollectAtHome;
        }

        public bool CanSpawItemQuestEvent()
        {
            return IsUnlocked() && !CheckCompleteAllQuest();
        }
    }
}