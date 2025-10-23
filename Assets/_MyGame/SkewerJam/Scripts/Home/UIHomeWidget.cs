using Cysharp.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using SonatFramework.Systems.EventBus;
using SonatFramework.Scripts.UIModule;

public class UIHomeWidget : MonoBehaviour, IHomeProcess
{
    public bool forceOpen = false;
    [SerializeField] protected bool active = true;

    [SerializeField, BoxGroup("Show Condition")] private int levelShow = 5;
    [SerializeField, BoxGroup("Show Condition")] private int DayShow = 3;

    private EventBinding<PanelUpdatedEvent> onPanelsUpdatedEvent;

    public virtual void Setup()
    {
        onPanelsUpdatedEvent = new EventBinding<PanelUpdatedEvent>(OnPanelsUpdated);
    }

    private void OnDestroy()
    {
        EventBus<PanelUpdatedEvent>.Deregister(onPanelsUpdatedEvent);
    }

    private void OnPanelsUpdated(PanelUpdatedEvent eventData)
    {
        if (eventData.isOpen == false)
        {
            OnFocus();
        }
        else
        {
            OnLoseFocus();
        }
    }

    public virtual void OnFocus()
    {
    }

    public virtual void OnLoseFocus()
    {
    }

    public virtual async UniTask<bool> ProcessTask()
    {
        return false;
    }

    protected virtual bool CheckActive()
    {
        int level = MySonatFramework.userDataService.GetLevel();
        int day = MySonatFramework.userDataService.UserDay;
        return level >= levelShow && day >= DayShow;
    }
}