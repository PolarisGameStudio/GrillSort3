using System.Collections;
using System.Collections.Generic;
using MyGame.Modules.QuestEvent;
using MyGame.Modules.QuestEvent.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using UnityEngine;

public class PopupQuestEvent : Panel
{
    [SerializeField] private QuestEventScroll questEventScroll;
    [SerializeField] private float delayScrollToNextIndexQuest = 1f;

    private readonly Service<QuestEventService> _questEventService = new();

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        questEventScroll.ScrollToCurrent(true, () =>
        {
            if (_questEventService.Instance.CheckCanClaimQuest())
            {
                _questEventService.Instance.ClaimQuest();
            }
        });
    }

    private void OnEnable()
    {
        _questEventService.Instance.OnDataUpdated += OnDataUpdated;
    }
    private void OnDisable()
    {
        _questEventService.Instance.OnDataUpdated -= OnDataUpdated;
    }

    private void OnDataUpdated()
    {
        SonatUtils.DelayCall(delayScrollToNextIndexQuest, () =>
        {
            questEventScroll.ScrollToCurrent(false);
        }, this);
    }

}
