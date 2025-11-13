using UnityEngine;

namespace MyGame.Modules
{
    public abstract class BaseInventoryModule<T, TConfig> : ScriptableObject where T : MyService<TConfig> where TConfig : MyServiceConfigSO
    {
        public abstract string DATA_KEY { get; }
        [SerializeField] protected T baseService;
        protected TConfig config => baseService.GetConfig();

        public abstract void LoadData();

        public abstract void ResetData();
    }
}