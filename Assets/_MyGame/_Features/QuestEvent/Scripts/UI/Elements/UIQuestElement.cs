using System.Collections;
using System.Collections.Generic;
using MyGame.Modules.UI.LoopScroll;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.QuestEvent.UI
{
    public enum State
    {
        Completed,
        Current,
        Claim,
        Locked,
    }

    public class UIQuestElement : ItemViewBase<QuestEventData>
    {
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private UIRewardGroup uiRewardGroup;
        [SerializeField] private Image bg;
        [SerializeField] private GameObject objLock;
        [SerializeField] private GameObject objComplete;
        [SerializeField] private GameObject objCurrent;

        [Header("bg sprite")]
        [SerializeField] private Sprite spriteBgNormal;
        [SerializeField] private Sprite spriteBgCurrent;

        private readonly Service<QuestEventService> _questEventService = new();

        private int _index;
        private State _state;

        public override void Bind(QuestEventData data)
        {
            _index = data.index;

            txtTitle.text = (_index + 1).ToString();

            var currentQuestIndexView = QuestEventHelper.GetQuestIndexView();
            if (_index == currentQuestIndexView)
            {
                if (QuestEventHelper.CheckClaimedQuest())
                {
                    SetCurrentState();
                }
                else
                {
                    SetClaimState();
                }
            }
            else
            {
                if (_index < currentQuestIndexView)
                {
                    // completed quest
                    SetCompletedState();
                }
                else
                {
                    // locked quest
                    SetLockedState();
                }
            }
        }

        private void SetClaimState()
        {
            _state = State.Claim;

            SetObjectState(State.Claim);
        }

        private void SetCurrentState()
        {
            _state = State.Current;

            SetObjectState(State.Current);
        }

        private void SetLockedState()
        {
            _state = State.Locked;

            SetObjectState(State.Locked);
        }

        private void SetCompletedState()
        {
            _state = State.Completed;

            SetObjectState(State.Completed);
        }

        private void SetObjectState(State state)
        {
            objComplete.SetActive(state == State.Completed);
            objCurrent.SetActive(state == State.Current || state == State.Claim);
            objLock.SetActive(state == State.Locked);

            switch (state)
            {
                case State.Completed:
                    bg.sprite = spriteBgNormal;
                    uiRewardGroup.gameObject.SetActive(false);
                    break;
                case State.Locked:
                    bg.sprite = spriteBgNormal;

                    var rewardDataLocked = _questEventService.Instance.config.listMilestones[_index].rewardData;
                    uiRewardGroup.SetData(rewardDataLocked);
                    uiRewardGroup.gameObject.SetActive(true);
                    break;
                case State.Current:
                case State.Claim:
                    bg.sprite = spriteBgCurrent;

                    var rewardData = _questEventService.Instance.config.listMilestones[_index].rewardData;
                    uiRewardGroup.SetData(rewardData);
                    uiRewardGroup.gameObject.SetActive(true);
                    break;
            }
        }

        public void OnClickClaim()
        {
            if (_state == State.Claim)
            {
                PopupToast.Cretate("You can claim the reward after completing the quest");
                return;
            }
        }
    }

    public class QuestEventData : ItemData
    {
        public int index;
    }
}
