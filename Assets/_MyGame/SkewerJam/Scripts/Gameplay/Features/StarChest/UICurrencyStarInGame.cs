using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
using UnityEngine;

public class UICurrencyStarInGame : UICurrency
{
    [Header("UIcurrencyStarInGame")]
    [SerializeField] private bool forceValue = false;
    [SerializeField] private bool isMiss = false;

    private readonly Service<StarChestService> starChestService = new();
    private EventBinding<LevelStartedEvent> levelStartedEvent;
    public override void OnEnable()
    {
        base.OnEnable();
        if (!starChestService.Instance.Config.active)
        {
            gameObject.SetActive(false);
            return;
        }

        levelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);

        if (forceValue)
        {
            value = starChestService.Instance.Star * (isMiss ? -1 : 1);
            txtValue.text = value.ToString();
        }
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
