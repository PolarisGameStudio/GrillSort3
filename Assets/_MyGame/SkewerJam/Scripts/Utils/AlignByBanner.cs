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

        private void OnEnable()
        {
            GameController.OnInitLevel += OnInitLevel;
            MySonatFramework.OnNoAdsUpdate += OnNoAdsUpdate;
            OnNoAdsUpdate(SonatSDKAdapter.IsNoads());
        }
        private void OnDisable()
        {
            MySonatFramework.OnNoAdsUpdate -= OnNoAdsUpdate;
            GameController.OnInitLevel -= OnInitLevel;
        }

        private void OnInitLevel()
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