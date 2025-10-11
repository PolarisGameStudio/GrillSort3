using System.Collections;
using DG.Tweening;
using Sonat;
using Sonat.AdsModule;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Systems.UserData;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.SceneManager
{
    public class LoadingScreen : MonoBehaviour
    {
        private bool sonatSdkInited = false;
        [SerializeField] private Slider slider;
        // private SkeletonGraphic logoAnim;
        // [SerializeField] private LocalizedEntry<SkeletonGraphic> logo;
        private float loadingTime = 2;

        private void Start()
        {
            slider.value = 0;

            if (PlayerPrefs.HasKey("LoadingFirstTime"))
            {
                loadingTime = 1;
            }
            else
            {
                loadingTime = 0.5f;
                PlayerPrefs.SetInt("LoadingFirstTime", 1);
            }

            // logoAnim = logo.GetLocalizedEntry();

            // slider.DOValue(1, loadingTime).OnComplete(() => { StartCoroutine(IELoading()); });
            StartCoroutine(IELoading());

            SonatSdkManager.Initialize(OnSonatSdkInited);
            // OnSonatSdkInited();
        }

        private void OnSonatSdkInited()
        {
            sonatSdkInited = true;
            // SonatUtils.ExecuteNextFrame(SonatTrackingService.Setup, 2);
        }

        IEnumerator IELoading()
        {
            yield return new WaitUntil(() => sonatSdkInited);
            // yield return new WaitForSeconds(0.1f);
            // logoAnim.AnimationState.ClearTracks();
            // logoAnim.Initialize(true);
            // logoAnim.AnimationState.SetAnimation(0, "End", false);
            // yield return new WaitForSeconds(0.43f);
            SonatAds.needShowAppOpenAds = false;
            int level = MySonatFramework.GetService<UserDataService>().GetLevel();
            // if (level >= GameRemoteConfigValue.levelForceHome)
            // {
            //     MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Home);
            // }
            // else
            // {
            MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Gameplay_SkewerJam);
            // }
        }
    }
}