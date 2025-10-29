using System.Collections;
using System.Collections.Generic;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using UnityEngine;

namespace MyGame.SkewerJam.Features.BannerController
{
    public class BannerController : MonoBehaviour
    {
        private void OnEnable()
        {
            MySonatFramework.OnNoAdsUpdate += OnNoAdsUpdate;
            if (MySonatFramework.IsShowBanner() == false)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            MySonatFramework.OnNoAdsUpdate -= OnNoAdsUpdate;
        }

        private void OnNoAdsUpdate(bool value)
        {
            if (value)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
