using MyGame.Modules.SubInventory.UI.Elements;
using SonatFramework.Systems;

namespace MyGame.Modules.QuestEvent.UI.Elements
{
    public class UICurrencyQuestItem : UICurrencySubItem
    {
        private readonly Service<QuestEventService> questEventService = new();

        public override void OnEnable()
        {
            if (questEventService.Instance.CanSpawItemQuestEvent())
            {
                base.OnEnable();
                questEventService.Instance.OnResetItemQuestEvent += OnResetItemQuestEvent;
            }
            else
            {
                gameObject.SetActive(false);
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            questEventService.Instance.OnResetItemQuestEvent -= OnResetItemQuestEvent;
        }

        private void OnResetItemQuestEvent()
        {
            UpdateValueView(false);
        }
    }
}