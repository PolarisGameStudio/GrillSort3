using System;
using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;

namespace MyGame.Modules
{
    public abstract class BaseLockService : SonatServiceSo, IServiceInitialize
    {
        public abstract string DATA_KEY { get; }

        private IntDataPref isUnlocked;

        public virtual void Initialize()
        {
            LoadConfig();
            LoadData();

            new EventBinding<HomeSetupEvent>(OnHomeSetupEvent);
            new EventBinding<HomeProcessEvent>(OnHomeProcessEvent);
        }

        protected virtual void LoadConfig()
        {

        }

        protected virtual void LoadData()
        {
            isUnlocked = new IntDataPref($"{DATA_KEY}_isUnlocked", 0);
        }

        protected virtual void ResetData()
        {

        }

        protected virtual void SaveData()
        {

        }

        #region Unlock
        public bool IsUnlocked()
        {
            return isUnlocked.Value == 1;
        }

        public abstract bool CanUnlock();

        public virtual void Unlock()
        {
            isUnlocked.Value = 1;
            ResetData();
        }
        #endregion



        private void OnHomeSetupEvent(HomeSetupEvent eventData)
        {
            if (IsUnlocked() == false && CanUnlock() == true)
            {
                Unlock();
            }
        }

        private void OnHomeProcessEvent(HomeProcessEvent @event)
        {
            if (IsUnlocked() == true)
            {
                TryShowTutorial().Forget();
                ProgressUnlockFeature();
            }
        }

        protected abstract UniTask TryShowTutorial();
        protected abstract void ProgressUnlockFeature();
    }
}