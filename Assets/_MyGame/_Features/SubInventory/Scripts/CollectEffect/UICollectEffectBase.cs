using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

namespace MyGame.Modules.SubInventory.Scripts.CollectEffect
{
    public abstract class UICollectEffectBase : MonoBehaviour, IPoolingObject
    {
        public abstract void Setup();
        public abstract void OnCreateObj(params object[] args);
        public abstract void OnReturnObj();
    }
}