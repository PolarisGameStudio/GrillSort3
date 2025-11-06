using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.QuestEvent.UI.Elements
{
    public class UISliderQuestEvent : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtProgress;
        [SerializeField] private Slider slider;
        [SerializeField] private UIBubbleReward bubbleReward;
        private readonly Service<QuestEventService> _questEventService = new();

        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            var currentQuestIndexView = QuestEventHelper.GetQuestIndexView();

            var currentItem = QuestEventHelper.GetNumberQuestView();

            if (currentQuestIndexView < _questEventService.Instance.config.listMilestones.Count)
            {
                var milestone = _questEventService.Instance.config.listMilestones[currentQuestIndexView];
                slider.value = _questEventService.Instance.GetCurrentProgress();
                txtProgress.text = $"{currentItem}/{milestone.numItem}";

                bubbleReward.SetReward(milestone.rewardData);
            }
            else
            {
                var milestone = _questEventService.Instance.config.listMilestones[_questEventService.Instance.config.listMilestones.Count - 1];
                slider.value = _questEventService.Instance.GetCurrentProgress();
                txtProgress.text = $"{milestone.numItem}/{milestone.numItem}";

                bubbleReward.SetReward(milestone.rewardData);
            }
        }
    }
}
