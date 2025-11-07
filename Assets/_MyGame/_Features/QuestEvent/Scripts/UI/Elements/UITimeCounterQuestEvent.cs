using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using UnityEngine;

namespace MyGame.Modules.QuestEvent.UI.Elements
{
    public class UITimeCounterQuestEvent : MonoBehaviour
    {
        [SerializeField] private UITimeCounter timeCounter;
        private readonly Service<QuestEventService> _questEventService = new();


        private void OnEnable()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            timeCounter.SetData(_questEventService.Instance.GetRemainTime());
        }
    }
}