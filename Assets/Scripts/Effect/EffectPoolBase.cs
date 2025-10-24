using DG.Tweening;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

public class EffectPoolBase : MonoBehaviour, IPoolingObject
{
    public float timeLive = 0;
    [SerializeField] private bool returnPool = false;

    public virtual void Setup()
    {
    }

    public virtual void OnCreateObj(params object[] args)
    {
        transform.localScale = Vector3.one;
        if (timeLive > 0)
            SonatUtils.DelayCall(timeLive, () => Destroy(), this);
    }

    public virtual void OnReturnObj()
    {
    }

    public void Destroy()
    {
        if (returnPool)
        {
            SonatSystem.GetService<PoolingServiceAsync>().ReturnObj(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
        // SonatSystem.GetService<PoolingServiceAsync>().ReturnObj(this);
    }
}