using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems;
using SonatFramework.Systems.TimeManagement;

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
    }
}