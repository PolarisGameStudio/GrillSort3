using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;

namespace MyGame.Modules.QuestEvent.UI.Elements
{
    public class UIWidgetQuestEvent : UIHomeWidget
    {
        [SerializeField] private UITimeCounterQuestEvent timeCounter;
        [SerializeField] private UISliderQuestEvent sliderQuestEvent;
        [SerializeField] private TMP_Text txtLevelUnlock;
        // [SerializeField] private LocalizationParamsManager levelUnlockParamsManager;

        [Header("Lock")]
        [SerializeField] private GameObject[] lockedObjects;
        [SerializeField] private GameObject[] unlockedObjects;

        private readonly Service<QuestEventService> _questEventService = new();


        public override void Setup()
        {
            base.Setup();

            active = _questEventService.Instance.IsUnlocked();

            SetActiveObjects(active);

            if (active)
            {
                // timeCounter.UpdateUI();
                // sliderQuestEvent.UpdateUI();
            }
            else
            {
                var unlockLevel = _questEventService.Instance.GetConfig().unlockLevel;
                // levelUnlockParamsManager.SetParameterValue("VALUE", $"{unlockLevel}");
                txtLevelUnlock.text = $"Unlock at level {unlockLevel}";
            }
        }

        private void SetActiveObjects(bool active)
        {
            foreach (var obj in lockedObjects)
            {
                obj.SetActive(!active);
            }
            foreach (var obj in unlockedObjects)
            {
                obj.SetActive(active);
            }
        }

        public void OnClickOpenQuestEvent()
        {
            if (active)
            {
                PanelManager.Instance.OpenPanel<PopupQuestEvent>();
            }
            else
            {
                var unlockLevel = _questEventService.Instance.GetConfig().unlockLevel;
                PopupToast.Cretate($"Unlock at level {unlockLevel}");
            }
        }
    }
}