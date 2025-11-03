using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems.TimeManagement;

namespace MyGame.Modules
{
    public abstract class BaseExpireService : BaseLockService
    {
        private LongDataPref expireTime;

        public override void Initialize()
        {
            base.Initialize();

            if (IsUnlocked())
            {
                CheckExpire().Forget();
            }
        }

        protected override void LoadData()
        {
            base.LoadData();
            expireTime = new LongDataPref($"{DATA_KEY}_expireTime");
        }

        protected override void ResetData()
        {
            expireTime.Value = GetNextExpireTime();
        }
        #region Unlock
        public override void Unlock()
        {
            base.Unlock();
            CheckExpire().Forget();
        }
        #endregion

        #region Expire
        protected abstract long GetNextExpireTime();
        private async UniTask CheckExpire()
        {
            if (expireTime.Value > 0)
            {
                var remainTime = GetRemainTime();
                if (remainTime > 0)
                {
                    await UniTask.Delay((long)remainTime * 1000);
                }
            }

            ResetData();
            CheckExpire().Forget();
        }

        public long GetRemainTime()
        {
            var currentTime = MySonatFramework.GetService<TimeService>().GetUnixTimeSeconds();
            return expireTime.Value - currentTime;
        }
        #endregion
    }
}