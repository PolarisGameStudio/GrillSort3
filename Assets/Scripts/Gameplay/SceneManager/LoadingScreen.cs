using System.Collections;
using DG.Tweening;
using Manager;
using Sonat;
using Sonat.AdsModule;
using Sonat.Enums;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Systems.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.SceneManager
{
    public class LoadingScreen : MonoBehaviour
    {
        private bool sonatSdkInited = false;
        [SerializeField] private Slider slider;
        [SerializeField] private float minLoadingTime = 2;
        [SerializeField] private float startSpeed = 0.5f;
        [SerializeField] private float loadingSpeed = 0.01f;
        [SerializeField] private float endSpeed = 1f;

        private void Start()
        {
            slider.value = 0;

            if (PlayerPrefs.HasKey("LoadingFirstTime"))
            {
                minLoadingTime = 1;
            }
            else
            {
                PlayerPrefs.SetInt("LoadingFirstTime", 1);
            }

            StartCoroutine(PlaySlider());

            SonatSdkManager.Initialize(OnSonatSdkInited);
        }

        private void OnSonatSdkInited()
        {
            sonatSdkInited = true;
        }

        IEnumerator PlaySlider()
        {
            // tăng nhanh đến 0.5f và di chuyển tạm tới khi inited
            // bắt buộc load ít nhất 2s
            slider.value = 0;
            var startTime = Time.time;
            while (slider.value < 0.5f)
            {
                slider.value += startSpeed * Time.deltaTime;
                yield return null;
            }

            while (slider.value < 1)
            {
                slider.value += loadingSpeed * Time.deltaTime;
                yield return null;
                if (sonatSdkInited && Time.time - startTime >= minLoadingTime)
                {
                    break;
                }
            }

            while (slider.value < 1)
            {
                slider.value += endSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            CompleteLoading();

        }

        void CompleteLoading()
        {
            SonatAds.needShowAppOpenAds = false;
            GameRemoteConfigValue.LoadData();

            int level = MySonatFramework.GetService<UserDataService>().GetLevel();
            if (level >= GameRemoteConfigValue.levelForceHome)
            {
                MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Home);
            }
            else
            {
                MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Gameplay_SkewerJam);
            }
        }
    }
}