using Base.Singleton;
using DG.Tweening;
using Manager;
using Sonat;
using Sonat.AdsModule;
using Sonat.Enums;
using Sonat.TrackingModule;
using SonatFramework.Scripts.Feature.Lives;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Systems.TrackingModule;
using SonatFramework.Systems.UserData;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Loading
{
    public class LoadingInstance : Singleton<LoadingInstance>
    {
        [SerializeField] private SplashScreen splashScreen;
        [SerializeField] private LoadingScreen loadingScreen;
        [SerializeField] private LoadingConfigSO configSO;

        [Header("Tracking")]
        [SerializeField] private string screenName = "Loading";
        [SerializeField] private string placement = "GP:::app_open";

        public LoadingConfigSO Config => configSO;

        public bool PlayFromLoadingScene { get; set; } = false;

        private bool _sonatSdkInited = false;
        public bool SonatSdkInited => _sonatSdkInited;

        protected override void OnAwake()
        {

        }

        private void Start()
        {
            _sonatSdkInited = false;
            loadingScreen.gameObject.SetActive(false);
            splashScreen.gameObject.SetActive(true);
            splashScreen.StartLoading(OnLoadingComplete);
            SonatSdkManager.Initialize(OnSonatSdkInited);
        }

        private void OnSonatSdkInited()
        {
            _sonatSdkInited = true;
            SonatAds.needShowAppOpenAds = false;
            GameRemoteConfigValue.LoadData();

            EventBus<UpdateScreenEvent>.Raise(new() { screen = screenName });
            EventBus<UpdatePlacementEvent>.Raise(new() { placement = placement });
            MySonatFramework.GetService<TrackingService>().TrackingScreenView();
        }

        public bool CheckForceGameplay()
        {
            int level = MySonatFramework.GetService<UserDataService>().GetLevel();
            return level < GameRemoteConfigValue.levelForceHome && MySonatFramework.GetService<LivesService>().CanPlay();
        }


        private void OnLoadingComplete()
        {
            int level = MySonatFramework.GetService<UserDataService>().GetLevel();
            if (level < GameRemoteConfigValue.levelForceHome && MySonatFramework.GetService<LivesService>().CanPlay())
            {
                PlayFromLoadingScene = true;
                MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Gameplay_SkewerJam);
            }
            else
            {
                MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Home);
                SonatUtils.DelayCall(configSO.delayHideLoading, () => HideLoading(), this);
            }
        }

        public void ShowLoading(float time = 3, bool stopMusic = true)
        {
            var canvasSplash = splashScreen.GetComponent<CanvasGroup>();
            var canvasLoading = loadingScreen.GetComponent<CanvasGroup>();
            canvasSplash.DOKill();
            canvasLoading.DOKill();
            canvasSplash.alpha = 1;
            canvasLoading.alpha = 1;
            splashScreen.gameObject.SetActive(false);
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.SetTime(time);

            if (stopMusic)
            {
                MySonatFramework.GetService<AudioService>().StopMusic();
            }
        }

        public void HideLoading()
        {
            SonatUtils.DelayCall(0.25f, () =>
            {
                var canvasSplash = splashScreen.GetComponent<CanvasGroup>();
                var canvasLoading = loadingScreen.GetComponent<CanvasGroup>();

                canvasSplash.DOKill();
                canvasLoading.DOKill();
                canvasSplash.DOFade(0, 0.3f).OnComplete(() =>
                {
                    splashScreen.gameObject.SetActive(false);
                });
                canvasLoading.DOFade(0, 0.3f).OnComplete(() =>
                {
                    loadingScreen.gameObject.SetActive(false);
                });
            }, this);
        }
    }
}
