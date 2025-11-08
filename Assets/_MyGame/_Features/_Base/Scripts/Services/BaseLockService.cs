using Cysharp.Threading.Tasks;
using MyGame.SkewerJam.Gameplay;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.Modules
{
    public abstract class BaseLockService<T> : MyService<T> where T : LockServiceConfigSO
    {
        private IntDataPref isUnlocked;

        public override void Initialize()
        {
            base.Initialize();

            new EventBinding<HomeSetupEvent>(OnHomeSetupEvent);
            new EventBinding<HomeProcessEvent>(OnHomeProcessEvent);
        }

        protected override void LoadConfig()
        {

        }

        protected override void LoadData()
        {
            isUnlocked = new IntDataPref($"{DATA_KEY}_isUnlocked", 0);
        }

        protected override void ResetData()
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
                HomeManager.Instance.TutorialFeatureManager.AddTutorial(new TutorialFeatureData()
                {
                    featureName = DATA_KEY,
                    order = GetConfig().orderTutorial,
                    action = async () =>
                    {
                        await TryShowTutorial();
                    }
                });
            }
        }

        private void OnHomeProcessEvent(HomeProcessEvent @event)
        {
            if (IsUnlocked() == true)
            {
                ProgressUnlockFeature();
            }
        }

        protected abstract UniTask TryShowTutorial();
        protected abstract void ProgressUnlockFeature();
    }
}