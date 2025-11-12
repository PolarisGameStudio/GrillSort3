using SonatFramework.Systems;
using UnityEngine;

namespace MyGame.Modules
{
    public abstract class MyService<T> : SonatServiceSo, IServiceInitialize where T : MyServiceConfigSO
    {
        public abstract string DATA_KEY { get; }

        [SerializeField] protected T config;
        public T GetConfig() => config as T;

        public virtual void Initialize()
        {
            LoadConfig();
            LoadData();

            Init();
        }

        protected abstract void Init();
        protected abstract void LoadData();
        protected abstract void LoadConfig();
        protected abstract void ResetData();
    }
}