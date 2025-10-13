using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;

public class UICurrencyStarInGame : UICurrency
{
    private readonly Service<StarChestService> starChestService = new();
    private EventBinding<LevelStartedEvent> levelStartedEvent;
    public override void OnEnable()
    {
        base.OnEnable();

        levelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus<LevelStartedEvent>.Deregister(levelStartedEvent);
    }

    public void OnLevelStarted(LevelStartedEvent eventData)
    {
        UpdateValueView(false);
    }

    public override void UpdateValue(float duration, float delay = 0)
    {
        int oldvalue = this.value;
        value = starChestService.Instance.Star;
        if (gameObject.activeInHierarchy)
            txtValue.DOCounter(oldvalue, value, duration, addThousandsSeparator: false).SetDelay(delay);
        else
            txtValue.text = value.ToString();
    }

    public override void UpdateValueView(bool doCounter = true)
    {
        int oldvalue = this.value;
        value = starChestService.Instance.Star;

        if (value == oldvalue) return;
        if (gameObject.activeInHierarchy && doCounter)
            txtValue.DOCounter(oldvalue, value, counterDuration, addThousandsSeparator: false);
        else
            txtValue.text = value.ToString();
    }
}
