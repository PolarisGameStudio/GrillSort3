using SonatFramework.Scripts.SonatSDKAdapterModule;
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
            MySonatFramework.OnNoAdsUpdate += OnNoAdsUpdate;
            OnNoAdsUpdate(SonatSDKAdapter.IsNoads());
        }
        private void OnDisable()
        {
            MySonatFramework.OnNoAdsUpdate -= OnNoAdsUpdate;
        }

        private void OnNoAdsUpdate(bool value)
        {
            var isShowBanner = MySonatFramework.IsShowBanner();
            objects.anchoredPosition = isShowBanner == false ? posNoAds.anchoredPosition : posAds.anchoredPosition;
        }
    }
}