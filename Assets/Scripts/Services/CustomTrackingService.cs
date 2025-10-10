using SonatFramework.Systems.EventBus;

namespace Sonat.CustomService
{
    public class CustomTrackingService : SonatFramework.Systems.TrackingModule.TrackingService
    {
        public override void LogEarnCurrency(EarnResourceEvent eventData)
        {
            throw new System.NotImplementedException();
        }

        public override void LogLevelComplete(int level)
        {
            throw new System.NotImplementedException();
        }

        public override void LogLevelEnd()
        {
            throw new System.NotImplementedException();
        }

        public override void LogLevelStart()
        {
            throw new System.NotImplementedException();
        }

        public override void LogLevelUp()
        {
            throw new System.NotImplementedException();
        }

        public override void LogShortcut(string shortcutName)
        {
            throw new System.NotImplementedException();
        }

        public override void LogShowAds()
        {
            throw new System.NotImplementedException();
        }

        public override void LogShowInterAds()
        {
            throw new System.NotImplementedException();
        }

        public override void LogShowRewardAds()
        {
            throw new System.NotImplementedException();
        }

        public override void LogSpendCurrency(SpendResourceEvent eventData)
        {
            throw new System.NotImplementedException();
        }

        public override void LogUseBooster(string boosterName)
        {
            throw new System.NotImplementedException();
        }

        public override void OnShowPopup(string uiName, string uiType, string uiClass, string openBy, string action = "open")
        {
            throw new System.NotImplementedException();
        }

        public override void TrackingScreenView()
        {
            throw new System.NotImplementedException();
        }
    }
}