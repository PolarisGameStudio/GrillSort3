using Cysharp.Threading.Tasks;
using Manager;
using MyGame.Scripts.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using UnityEngine;

public class HomeWidgetManager : MonoBehaviour
{
    [SerializeField] private int delayProcess = 2500;
    [SerializeField] private UIHomeWidget[] widgets;
    public static int homeCount = -1;

    public void Setup()
    {
        homeCount++;
        foreach (var widget in widgets)
        {
            widget.Setup();
        }

        HomeManager.Instance.BlockUI();
        ProcessTasks().Forget();
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

    public async UniTask ProcessTasks()
    {
        await UniTask.Delay(delayProcess);
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
        HomeManager.Instance.UnlockUI();
        EventBus<HomeProcessEvent>.Raise(new HomeProcessEvent());
    }

    private void OnDisable()
    {

    }
}