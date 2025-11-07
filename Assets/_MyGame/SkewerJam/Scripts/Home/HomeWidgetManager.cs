using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
using UnityEngine;

public class HomeWidgetManager : MonoBehaviour
{
    [SerializeField] private float delayProcess = 2.5f;
    [SerializeField] private float blockUIDuration = 3f;
    [SerializeField] private UIHomeWidget[] widgets;
    public static int homeCount = -1;


    public void Setup()
    {
        homeCount++;
        foreach (var widget in widgets)
        {
            widget.Setup();
        }

        HomeManager.Instance.BlockUIManager.RegisterBlockUI(nameof(HomeWidgetManager));
        SonatUtils.DelayCall(blockUIDuration, () =>
        {
            HomeManager.Instance.BlockUIManager.DeregisterBlockUI(nameof(HomeWidgetManager));
        }, this);
        // ProcessTasks().Forget();
    }

    public void OnFocus()
    {
        foreach (var widget in widgets)
        {
            widget.OnFocus();
        }
    }

    public void OnLoseFocus()
    {
        foreach (var widget in widgets)
        {
            widget.OnLoseFocus();
        }
    }

    public async UniTask ProcessTasks(Action onComplete = null)
    {
        await UniTask.Delay((int)(delayProcess * 1000));
        int popupCount = 0;
        foreach (var widget in widgets)
        {
            if (!widget.forceOpen && popupCount >= GameRemoteConfigValue.maxPopupMO) continue;
            var open = await widget.ProcessTask();
            if (open)
            {
                popupCount++;
            }
        }
        EventBus<HomeProcessEvent>.Raise(new HomeProcessEvent());
        onComplete?.Invoke();
    }

    private void OnDisable()
    {

    }
}