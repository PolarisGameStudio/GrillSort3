using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.QuestEvent.UI.Elements
{
    public class UISlider : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtProgress;
        [SerializeField] private Slider slider;
        [SerializeField] private UIBubbleReward bubbleReward;
        private readonly Service<QuestEventService> _questEventService = new();

        private void OnEnable()
        {
            var currentQuestIndex = _questEventService.Instance.CurrentQuestIndex;
            var currentItem = _questEventService.Instance.CurrentItem;
            var milestone = _questEventService.Instance.config.listMilestones[currentQuestIndex];

            slider.value = _questEventService.Instance.GetCurrentProgress();
            txtProgress.text = $"{currentItem}/{milestone.numItem}";

            bubbleReward.SetReward(milestone.rewardData);


        }
    }
}
