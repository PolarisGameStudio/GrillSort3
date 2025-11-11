using System;
using MyGame.Modules.UI.LoopScroll;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame.Modules.QuestEvent.UI
{
    public class QuestEventScroll : CustomScrollViewBase<QuestEventData>
    {
        [SerializeField] private float speedStart = 5000f;
        [SerializeField] private float speedNextIndexQuest = 1000f;

        private readonly Service<QuestEventService> _questEventService = new();

        protected override int GetMaxElements()
        {
            return _questEventService.Instance.GetConfig().listMilestones.Count;
        }

        protected override QuestEventData GetData(int idx)
        {
            return new QuestEventData { index = GetMaxElements() - idx - 1, max = GetMaxElements() };
        }

        public void ScrollToCurrent(bool start, Action onComplete = null)
        {
            var currentQuestIndex = _questEventService.Instance.GetCurrentQuestIndexView();
            var index = GetMaxElements() - currentQuestIndex - 1;
            scroll.ScrollToCell(index, start ? speedStart : speedNextIndexQuest);

            SonatUtils.DelayCall(0.75f, () =>
            {
                onComplete?.Invoke();
            });
        }
    }
}