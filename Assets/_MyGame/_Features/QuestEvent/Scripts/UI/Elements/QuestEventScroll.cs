using MyGame.Modules.UI.LoopScroll;
using SonatFramework.Systems;
using UnityEngine;

namespace MyGame.Modules.QuestEvent.UI
{
    public class QuestEventScroll : CustomScrollViewBase<QuestEventData>
    {
        private readonly Service<QuestEventService> _questEventService = new();

        protected override int GetMaxElements()
        {
            return _questEventService.Instance.config.listMilestones.Count;
        }

        protected override QuestEventData GetData(int idx)
        {
            return new QuestEventData { index = GetMaxElements() - idx - 1 };
        }
    }
}