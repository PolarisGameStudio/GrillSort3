using System;
using Sonat.CustomService;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.CheckInternet;
using SonatFramework.Scripts.Feature.Lives;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.LevelManagement;
using SonatFramework.Systems.LoadObject;
using SonatFramework.Systems.ObjectPooling;
using SonatFramework.Systems.TrackingModule;
using SonatFramework.Systems.UserData;
using UnityEngine;

public class MySonatFramework : SonatSystem
{
    public static SonatPoolingService poolingService;
    public static SonatPoolingContainer poolingContainer;
    public static SonatLoadAddressableAsync sonatLoadAddressableAsync;
    public static SonatLevelService sonatLevelService;
    public static AudioService audioService;
    public static UserDataService userDataService;
    public static LivesService livesService;
    public static InventoryService inventoryService;
    public static LevelService levelService;
    public static TrackingService trackingService;
    public static GameplayAnalyticsService gameplayAnalyticsService;
    public static SonatBoosterService sonatBoosterService;
    public static CheckInternetService checkInternetService;

    public static event Action<bool> OnNoAdsUpdate;

    private void Start()
    {
        Application.targetFrameRate = 80;
        Input.multiTouchEnabled = false;
        InitService();
    }

    public static void InitService()
    {
        poolingService = GetService<SonatPoolingService>();
        poolingContainer = GetService<SonatPoolingContainer>();
        sonatLoadAddressableAsync = GetService<SonatLoadAddressableAsync>();
        sonatLevelService = GetService<SonatLevelService>();
        audioService = GetService<AudioService>();
        userDataService = GetService<UserDataService>();
        livesService = GetService<LivesService>();
        inventoryService = GetService<InventoryService>();
        levelService = GetService<LevelRemoteService>();
        trackingService = GetService<TrackingService>();
        gameplayAnalyticsService = GetService<GameplayAnalyticsService>();
        sonatBoosterService = GetService<SonatBoosterService>();
        checkInternetService = GetService<CheckInternetService>();
    }

    public static LevelDifficulty GetLevelDifficulty(int level)
    {
        // try
        // {

        //     return LevelGenerator.levelService != null
        //         ? LevelGenerator.levelService.GetLevelData<Gameplay.LevelData.LevelData>(level, GameMode.Classic).difficulty
        //             : levelService.GetLevelData<Gameplay.LevelData.LevelData>(level, GameMode.Classic).difficulty;
        // }
        // catch (Exception)
        // {
        return LevelDifficulty.Easy1;
        // }
    }

    public static LevelType GetLevelType(int level)
    {
        return levelService.GetLevelData<Gameplay.LevelData.LevelData>(level, GameMode.Classic).levelType;
    }

    public static bool IsRewardAdsReady()
    {
        if (checkInternetService.TryCheckInternet() && SonatSDKAdapter.IsRewardAdsReady())
        {
            return true;
        }
        return false;
    }

    public static void TryShowBanner()
    {
        if (SonatSDKAdapter.IsNoads())
        {
            SonatSDKAdapter.SetBanner(false);
        }
        else
        {
            SonatSDKAdapter.SetBanner(true);
        }
    }

    public static void SetNoAds(bool value)
    {
        SonatSDKAdapter.SetNoAds(true);
        SonatSDKAdapter.SetBanner(false);

        OnNoAdsUpdate?.Invoke(value);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PanelManager.Instance.OpenPanel<CheatPanel>();
        }
    }
#endif
}