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
    [CreateAssetMenu(fileName = "QuestEventService", menuName = "MyGame/Features/QuestEvent/QuestEventService")]
    public class QuestEventService : BaseExpireService<QuestEventConfigSO>
    {
        public override string DATA_KEY => "QUEST_EVENT";
        public const string ITEM_NAME = "quest_item";

        private IntDataPref _currentItem;
        private IntDataPref _claimedQuestIndex;

        public int CurrentItem => _currentItem.Value;

        public event Action OnDataUpdated;
        public event Action OnResetItemQuestEvent;

        private int _numCollectAtHome = 0;

        protected override void Init()
        {
            new EventBinding<LevelStartedEvent>(OnLevelStarted);
            new EventBinding<LevelEndedEvent>(OnLevelEnded);
            new EventBinding<LevelQuitEvent>(OnLevelQuit);
            new EventBinding<LevelReplayEvent>(OnLevelReplay);
        }

        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            if (IsUnlocked() == false) return;
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
            OnResetItemQuestEvent?.Invoke();
        }

        private void OnLevelEnded(LevelEndedEvent eventData)
        {
            if (IsUnlocked() == false) return;
            if (eventData.success)
            {
                _numCollectAtHome = MySonatFramework.GetService<SubInventoryService>().GetResource(SubGameResource.QuestEventItem);
                _currentItem.Value += _numCollectAtHome;
            }
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
            OnResetItemQuestEvent?.Invoke();
        }

        private void OnLevelQuit(LevelQuitEvent eventData)
        {
            if (IsUnlocked() == false) return;
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
            OnResetItemQuestEvent?.Invoke();
        }

        private void OnLevelReplay(LevelReplayEvent eventData)
        {
            if (IsUnlocked() == false) return;
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
            OnResetItemQuestEvent?.Invoke();
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

        protected override void ResetData()
        {
            base.ResetData();
            _currentItem.Value = 0;
            _claimedQuestIndex.Value = -1;

            _numCollectAtHome = 0;
            MySonatFramework.GetService<SubInventoryService>().SetResource(SubGameResource.QuestEventItem, 0);
            OnDataUpdated?.Invoke();
        }

        public override bool CanUnlock()
        {
            var level = MySonatFramework.GetService<UserDataService>().GetLevel();
            return level >= GetConfig().unlockLevel;
        }

        protected override long GetNextExpireTime()
        {
            var now = MySonatFramework.GetService<TimeService>().GetCurrentTime();
            int daysToEndOfWeek = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
            var endOfNextWeek = now.Date.AddDays(daysToEndOfWeek + 1);
            return ((DateTimeOffset)endOfNextWeek).ToUnixTimeSeconds();
        }

        protected override void ProgressUnlockFeature()
        {
            if (_numCollectAtHome > 0)
            {
                HomeManager.Instance.BlockUIManager.RegisterBlockUI(nameof(QuestEventService) + "_receive_at_home");
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


                SonatUtils.DelayCall(3f, () =>
                {
                    _numCollectAtHome = 0;
                    if (CheckCanClaimQuest())
                    {
                        PanelManager.Instance.OpenPanel<PopupQuestEvent>();
                    }
                    HomeManager.Instance.BlockUIManager.DeregisterBlockUI(nameof(QuestEventService) + "_receive_at_home");
                });
            }
        }

        protected override async UniTask TryShowTutorial()
        {
            // Hiện tut
            if (PlayerPrefs.HasKey($"{DATA_KEY}_ShowTutorial") == false)
            {
                PlayerPrefs.SetInt($"{DATA_KEY}_ShowTutorial", 1);

                await UniTask.Delay(300);
                var popup = PanelManager.Instance.OpenPanelByName<Panel>("PopupQuestEventInfo");
                await UniTask.WaitUntil(() => popup == null || popup.gameObject.activeInHierarchy == false);
                await UniTask.Delay(300);


                var popup2 = PanelManager.Instance.OpenPanelByName<Panel>("PopupQuestEvent");
                await UniTask.WaitUntil(() => popup2 == null || popup2.gameObject.activeInHierarchy == false);
            }
        }
        #endregion

        public int GetCurrentQuestIndexView()
        {
            return _claimedQuestIndex.Value + 1;
        }

        public bool CheckCanClaimQuest()
        {
            var currentQuestIndex = GetCurrentQuestIndexView();
            var itemRequired = GetConfig().listMilestones[currentQuestIndex].numItem;
            return _currentItem.Value >= itemRequired;
        }

        public void ClaimQuest()
        {
            if (!CheckCanClaimQuest())
            {
                return;
            }
            var currentQuestIndex = GetCurrentQuestIndexView();
            var itemRequired = GetConfig().listMilestones[currentQuestIndex].numItem;

            _currentItem.Value -= itemRequired;
            _claimedQuestIndex.Value += 1;
            OnDataUpdated?.Invoke();

            var rewardData = GetConfig().listMilestones[currentQuestIndex].rewardData;
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

            // loop claim
            ClaimQuest();
        }

        public bool CheckCompleteAllQuest()
        {
            return _claimedQuestIndex.Value >= GetConfig().listMilestones.Count - 1;
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