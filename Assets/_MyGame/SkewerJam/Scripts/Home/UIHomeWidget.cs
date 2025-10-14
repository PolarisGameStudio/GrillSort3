using Cysharp.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

public class UIHomeWidget : MonoBehaviour, IHomeProcess
{
    public bool forceOpen = false;
    [SerializeField] protected bool active = true;

    [SerializeField, BoxGroup("Show Condition")] private int levelShow = 5;
    [SerializeField, BoxGroup("Show Condition")] private int DayShow = 3;

    public virtual void Setup()
    {

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