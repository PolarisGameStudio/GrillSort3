using System.Collections;
using System.Collections.Generic;
using MyGame.Modules.QuestEvent;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupQuestEventInfo : Panel
{
    [SerializeField] private TMP_Text txtProgress;
    [SerializeField] private Slider slider;
    [SerializeField] private UIBubbleReward bubbleReward;
    [SerializeField] private UITimeCounter timeCounter;

    private readonly Service<QuestEventService> _questEventService = new();

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        var currentQuestIndex = _questEventService.Instance.CurrentQuestIndex;
        var currentItem = _questEventService.Instance.CurrentItem;
        var milestone = _questEventService.Instance.config.listMilestones[currentQuestIndex];

        slider.value = _questEventService.Instance.GetCurrentProgress();
        txtProgress.text = $"{currentItem}/{milestone.numItem}";

        bubbleReward.SetReward(milestone.rewardData);
        timeCounter.SetData(_questEventService.Instance.GetRemainTime());

    }
}
