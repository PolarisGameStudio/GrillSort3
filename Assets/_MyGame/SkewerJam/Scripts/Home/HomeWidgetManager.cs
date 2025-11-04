using Cysharp.Threading.Tasks;
using Manager;
using MyGame.Scripts.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using UnityEngine;

public class HomeWidgetManager : MonoBehaviour
{
    private BlockPanel blockPanel;
    [SerializeField] private int delayProcess = 750;
    [SerializeField] private UIHomeWidget[] widgets;
    public static int homeCount = -1;

    public void Setup()
    {
        homeCount++;
        foreach (var widget in widgets)
        {
            widget.Setup();
        }

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
        BlockUI();
        await UniTask.Delay(delayProcess);
        EventBus<HomeProcessEvent>.Raise(new HomeProcessEvent());
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
        UnlockUI();
    }


    public void BlockUI()
    {
        if (blockPanel != null) return;
        blockPanel = PanelManager.Instance.OpenPanel<BlockPanel>();
    }

    public void UnlockUI()
    {
        if (blockPanel == null) return;
        blockPanel.Close();
        blockPanel = null;
    }

    private void OnDisable()
    {
        UnlockUI();
    }
}