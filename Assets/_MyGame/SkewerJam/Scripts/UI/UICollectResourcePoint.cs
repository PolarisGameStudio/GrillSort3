using System;
using System.Collections;
using System.Collections.Generic;
using Base.Singleton;
using DG.Tweening;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using UnityEngine;

public class UICollectBoostersPoint : MonoBehaviour
{
    [SerializeField] private List<GameResource> resourceTypes;
    private EventBinding<AddItemEvent> collectItemEvent;
    private void OnEnable()
    {
        collectItemEvent = new EventBinding<AddItemEvent>(OnCollectResource);
    }

    private void OnDisable()
    {
        EventBus<AddItemEvent>.Deregister(collectItemEvent);
    }

    protected virtual void OnCollectResource(AddItemEvent eventData)
    {
        if (!resourceTypes.Contains(eventData.resource)) return;

        if (eventData.collectEffect != null)
        {
            eventData.collectEffect.Collect(eventData.resource, eventData.quantity, eventData.position, transform.position, () =>
            {
                float defaultScale = transform.localScale.x;
                transform.DOKill();
                transform.DOScale(defaultScale * 1.1f, 0.075f).SetLoops(2, LoopType.Yoyo);
            });
        }
    }
}
