using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Utils
{
    public class AlignByBanner : MonoBehaviour
    {
        [SerializeField] private RectTransform objects;
        [SerializeField] private RectTransform posNoAds;
        [SerializeField] private RectTransform posAds;

        private EventBinding<LevelStartedEvent> onLevelStartedEvent;

        private void OnEnable()
        {
            onLevelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);
            MySonatFramework.OnNoAdsUpdate += OnNoAdsUpdate;
            OnNoAdsUpdate(SonatSDKAdapter.IsNoads());
        }
        private void OnDisable()
        {
            MySonatFramework.OnNoAdsUpdate -= OnNoAdsUpdate;
            EventBus<LevelStartedEvent>.Deregister(onLevelStartedEvent);
        }

        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            OnNoAdsUpdate(SonatSDKAdapter.IsNoads());
        }

        private void OnNoAdsUpdate(bool value)
        {
            var isShowBanner = MySonatFramework.IsShowBanner();
            objects.anchoredPosition = isShowBanner == false ? posNoAds.anchoredPosition : posAds.anchoredPosition;
        }
    }
}