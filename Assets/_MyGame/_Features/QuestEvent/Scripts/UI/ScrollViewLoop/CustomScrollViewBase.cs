using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.UI.LoopScroll
{
    public abstract class CustomScrollViewBase<T> : MonoBehaviour, LoopScrollDataSource, LoopScrollPrefabSource where T : ItemData
    {
        [SerializeField] protected LoopScrollRect scroll;
        [SerializeField] protected ItemViewBase<T> itemViewPrefab;

        protected Transform poolRoot;
        protected Stack<Transform> pool = new();
        protected readonly HashSet<int> activeItems = new();
        protected readonly Dictionary<Transform, int> transToIndex = new();

        protected virtual void Awake()
        {
            poolRoot = new GameObject("PoolRoot").transform;
            poolRoot.SetParent(transform, false);
            poolRoot.gameObject.SetActive(false);

            scroll.prefabSource = this;
            scroll.dataSource = this;

            // scroll.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnEnable()
        {
            activeItems.Clear();
            transToIndex.Clear();

            RefreshScroll();
        }

        protected void RefreshScroll()
        {
            scroll.totalCount = GetMaxElements();
            scroll.RefillCells();
        }
        protected abstract T GetData(int idx);
        protected abstract int GetMaxElements();

        public void ProvideData(Transform transform, int idx)
        {
            transToIndex[transform] = idx;
            activeItems.Add(idx);

            var itemView = transform.GetComponent<ItemViewBase<T>>();
            var data = GetData(idx);
            itemView.Bind(data);
        }

        public GameObject GetObject(int index)
        {
            if (pool.Count == 0)
                return Instantiate(itemViewPrefab.gameObject);

            var t = pool.Pop();
            t.gameObject.SetActive(true);
            return t.gameObject;
        }

        public void ReturnObject(Transform trans)
        {
            if (transToIndex.TryGetValue(trans, out var idx))
            {
                activeItems.Remove(idx);
                transToIndex.Remove(trans);
            }

            trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            trans.gameObject.SetActive(false);
            trans.SetParent(poolRoot, false);
            pool.Push(trans);
        }
    }
}