using System;
using MyGame.Modules.UI.LoopScroll;
using SonatFramework.Systems;
using UnityEngine;

namespace MyGame.Modules.Language.UI
{
    public class UILanguageScroll : CustomScrollViewBase<LanguageData>
    {
        [SerializeField] private float speedStart = 5000f;
        [SerializeField] private float speedNextIndexQuest = 1000f;

        private readonly Service<LanguageService> _languageService = new();

        protected override int GetMaxElements()
        {
            return _languageService.Instance.GetConfig().listLanguages.Count;
        }

        protected override LanguageData GetData(int idx)
        {
            return new LanguageData { language = _languageService.Instance.GetConfig().listLanguages[idx] };
        }

        // public void ScrollToCurrent(bool start, Action onComplete = null)
        // {
        //     // var currentQuestIndex = _questEventService.Instance.GetCurrentQuestIndexView();
        //     // var index = GetMaxElements() - currentQuestIndex - 1;
        //     // scroll.ScrollToCell(index, start ? speedStart : speedNextIndexQuest);

        //     // SonatUtils.DelayCall(0.75f, () =>
        //     // {
        //     //     onComplete?.Invoke();
        //     // });
        // }
    }
}