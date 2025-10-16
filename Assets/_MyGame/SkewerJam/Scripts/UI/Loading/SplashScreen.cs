using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.SkewerJam.UI.Loading
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private LogoAnimation logoAnimation;

        [SerializeField] private SplashConfigSO configSO;

        private float _startTime = 0;

        private void Start()
        {
            slider.value = 0;
        }

        public void StartLoading(Action onComplete)
        {
            _startTime = Time.time;
            StartCoroutine(PlaySlider(onComplete));
        }

        IEnumerator PlaySlider(Action onComplete)
        {
            // tăng nhanh đến 0.5f và di chuyển tạm tới khi inited
            // bắt buộc load ít nhất 2s
            yield return new WaitForSeconds(configSO.delayStartSlider);
            slider.value = 0;
            while (slider.value < 0.5f)
            {
                slider.value += configSO.startSpeed * Time.deltaTime;
                yield return null;
            }

            while (slider.value < 1)
            {
                slider.value += configSO.loadingSpeed * Time.deltaTime;

                // chờ tới khi inited và đã đủ thời gian loading
                if (LoadingInstance.Instance.SonatSdkInited && CheckLoadingTime())
                {
                    break;
                }
                yield return null;
            }

            if (LoadingInstance.Instance.CheckForceGameplay())
            {
                logoAnimation.SetDelayEnd(configSO.delayCompleteGameplay);
            }
            else
            {
                logoAnimation.SetDelayEnd(configSO.delayCompleteHome);
            }
            logoAnimation.SetCompleted();
            while (slider.value < 1)
            {
                slider.value += configSO.endSpeed * Time.deltaTime;
                yield return null;
            }
            onComplete?.Invoke();

        }

        private bool CheckLoadingTime()
        {
            return Time.time - _startTime >= (configSO.minSliderLoadingTime);
        }
    }
}