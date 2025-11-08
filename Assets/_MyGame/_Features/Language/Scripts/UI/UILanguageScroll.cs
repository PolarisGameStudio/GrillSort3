using System;
using MyGame.Modules.UI.LoopScroll;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame.Modules.Language.UI
{
    public class UILanguageScroll : CustomScrollViewBase<LanguageData>
    {
        [SerializeField] private float speedStart = 5000f;
        [SerializeField] private float speedNextIndexQuest = 1000f;

        protected override int GetMaxElements()
        {
            // return _questEventService.Instance.config.listMilestones.Count;
            return 0;
        }

        protected override LanguageData GetData(int idx)
        {
            // return new LanguageData { languageName = LanguageName[idx], languageCode = LanguageCode[idx] };
            return null;
        }

        public void ScrollToCurrent(bool start, Action onComplete = null)
        {
            // var currentQuestIndex = _questEventService.Instance.GetCurrentQuestIndexView();
            // var index = GetMaxElements() - currentQuestIndex - 1;
            // scroll.ScrollToCell(index, start ? speedStart : speedNextIndexQuest);

            // SonatUtils.DelayCall(0.75f, () =>
            // {
            //     onComplete?.Invoke();
            // });
        }
    }
}