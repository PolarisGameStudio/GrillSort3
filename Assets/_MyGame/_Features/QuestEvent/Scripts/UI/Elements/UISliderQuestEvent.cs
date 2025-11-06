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
            var currentQuestIndexView = _questEventService.Instance.GetCurrentQuestIndexView();
            var maxItem = _questEventService.Instance.config.GetMaxItem(currentQuestIndexView);
            var currentItem = _questEventService.Instance.CurrentItem;

            currentItem = Mathf.Min(currentItem, maxItem);

            slider.value = currentItem * 1.0f / maxItem;
            txtProgress.text = $"{currentItem}/{maxItem}";

            bubbleReward.SetReward(_questEventService.Instance.config.listMilestones[currentQuestIndexView].rewardData);
        }
    }
}
