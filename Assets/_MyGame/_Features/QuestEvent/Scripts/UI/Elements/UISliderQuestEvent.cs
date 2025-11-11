using DG.Tweening;
using MyGame.Modules.SubInventory.UI.Elements;
using Sirenix.OdinInspector;
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

        [Header("Collect at home")]
        [SerializeField] private bool useCollectAtHome = false;

        private readonly Service<QuestEventService> _questEventService = new();

        private int _value = 0;

        private void OnEnable()
        {
            if (useCollectAtHome)
            {
                var currentQuestIndexView = _questEventService.Instance.GetCurrentQuestIndexView();
                var maxItem = _questEventService.Instance.GetConfig().GetMaxItem(currentQuestIndexView);

                var currentItem = _questEventService.Instance.GetCurrentItemView();
                _value = currentItem;

                SetSliderValue(_value, maxItem);

                bubbleReward.SetReward(_questEventService.Instance.GetConfig().listMilestones[currentQuestIndexView].rewardData);
            }
            else
            {
                UpdateUI();
            }
            _questEventService.Instance.OnDataUpdated += OnDataUpdated;
        }

        private void OnDisable()
        {
            _questEventService.Instance.OnDataUpdated -= OnDataUpdated;
        }

        private void OnDataUpdated()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            var currentQuestIndexView = _questEventService.Instance.GetCurrentQuestIndexView();
            var maxItem = _questEventService.Instance.GetConfig().GetMaxItem(currentQuestIndexView);

            var currentItem = _questEventService.Instance.CurrentItem;

            SetSliderValue(currentItem, maxItem);

            bubbleReward.SetReward(_questEventService.Instance.GetConfig().listMilestones[currentQuestIndexView].rewardData);
        }

        private void SetSliderValue(int currentItem, int maxItem)
        {
            currentItem = Mathf.Min(currentItem, maxItem);
            slider.value = currentItem * 1.0f / maxItem;
            txtProgress.text = $"{currentItem}/{maxItem}";
        }

        public void PlayUpdateSlider()
        {
            var currentQuestIndexView = _questEventService.Instance.GetCurrentQuestIndexView();
            var maxItem = _questEventService.Instance.GetConfig().GetMaxItem(currentQuestIndexView);

            var currentItem = _questEventService.Instance.CurrentItem;
            currentItem = Mathf.Min(currentItem, maxItem);

            var oldValue = _value * 100;
            var newValue = currentItem * 100;

            var temp = _value * 100;
            _value = currentItem;

            DOTween.To(() => oldValue, x => temp = x, newValue, 0.3f).OnUpdate(() =>
            {
                txtProgress.text = $"{Mathf.FloorToInt(temp * 1.0f / 100)}/{maxItem}";

                slider.value = (temp * 1.0f / 100) / maxItem;
            });
        }
    }
}
