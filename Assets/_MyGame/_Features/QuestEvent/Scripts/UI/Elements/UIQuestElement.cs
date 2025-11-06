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

            txtTitle.text = _index.ToString();
            var currentQuestIndex = _questEventService.Instance.CurrentQuestIndex;
            if (_index == currentQuestIndex)
            {
                SetCurrentState();
            }
            else
            {
                if (_index < currentQuestIndex)
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

        private void SetCurrentState()
        {
            _state = State.Current;
            bg.sprite = spriteBgCurrent;

            var rewardData = _questEventService.Instance.config.listMilestones[_index].rewardData;
            uiRewardGroup.SetData(rewardData);
            uiRewardGroup.gameObject.SetActive(true);

            SetObjectState(State.Current);
        }

        private void SetLockedState()
        {
            _state = State.Locked;
            bg.sprite = spriteBgNormal;

            var rewardData = _questEventService.Instance.config.listMilestones[_index].rewardData;
            uiRewardGroup.SetData(rewardData);
            uiRewardGroup.gameObject.SetActive(true);

            SetObjectState(State.Locked);
        }

        private void SetCompletedState()
        {
            _state = State.Completed;
            SetObjectState(State.Completed);
            uiRewardGroup.gameObject.SetActive(false);
        }

        private void SetObjectState(State state)
        {
            objComplete.SetActive(state == State.Completed);
            objCurrent.SetActive(state == State.Current);
            objLock.SetActive(state == State.Locked);
        }

        public void OnClickClaim()
        {
            if (_state == State.Current)
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
