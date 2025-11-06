using UnityEngine;

namespace MyGame.Modules.UI.LoopScroll
{
    public abstract class ItemViewBase<T> : MonoBehaviour where T : ItemData
    {
        public abstract void Bind(T data);
    }

    public abstract class ItemData
    {
    }
}