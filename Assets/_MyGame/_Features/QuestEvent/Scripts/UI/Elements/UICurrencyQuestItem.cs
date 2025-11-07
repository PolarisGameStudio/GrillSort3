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
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}