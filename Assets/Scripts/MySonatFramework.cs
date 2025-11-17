using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.Modules.CardCollection;
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
    public static PoolingServiceAsync poolingServiceAsync;
    public static SonatPoolingContainer poolingContainer;
    public static SonatLoadAddressableAsync sonatLoadAddressableAsync;
    public static AudioService audioService;
    public static UserDataService userDataService;
    public static LivesService livesService;
    public static InventoryService inventoryService;
    public static LevelServiceAsync levelServiceAsync;
    public static TrackingService trackingService;
    public static GameplayAnalyticsService gameplayAnalyticsService;
    public static SonatBoosterService sonatBoosterService;
    public static CheckInternetService checkInternetService;

    public static event Action<bool> OnNoAdsUpdate;

    private static Dictionary<int, LevelDifficulty> cachedLevelDifficulty = new Dictionary<int, LevelDifficulty>();

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        InitService();
    }

    public static void InitService()
    {
        poolingServiceAsync = GetService<PoolingServiceAsync>();
        poolingContainer = GetService<SonatPoolingContainer>();
        sonatLoadAddressableAsync = GetService<SonatLoadAddressableAsync>();
        audioService = GetService<AudioService>();
        userDataService = GetService<UserDataService>();
        livesService = GetService<LivesService>();
        inventoryService = GetService<InventoryService>();
        levelServiceAsync = GetService<SonatLevelServiceAsync>();
        trackingService = GetService<TrackingService>();
        gameplayAnalyticsService = GetService<GameplayAnalyticsService>();
        sonatBoosterService = GetService<SonatBoosterService>();
        checkInternetService = GetService<CheckInternetService>();
    }

    public static async UniTask<LevelDifficulty> GetLevelDifficulty()
    {
        var level = userDataService.GetLevel(GameMode.Classic);
        try
        {
            if (cachedLevelDifficulty.TryGetValue(level, out var difficulty))
            {
                return difficulty;
            }

            var levelData = await levelServiceAsync.GetLevelData<Gameplay.LevelData.LevelData>(level, GameMode.Classic);
            cachedLevelDifficulty.Add(level, levelData.difficulty);

            return levelData.difficulty;
        }
        catch (Exception)
        {
            return LevelDifficulty.Easy;
        }
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
        if (IsShowBanner() == false)
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

    public static bool IsShowBanner()
    {
        var level = userDataService.GetLevel();
        if (level >= GameRemoteConfigValue.levelStartShowBanner && SonatSDKAdapter.IsNoads() == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CanReceive(GameResource resource)
    {
        switch (GameResourceHelper.ResourceType(resource))
        {
            case GameResourceType.Card:
                return MySonatFramework.GetService<CardCollectionService>().IsUnlocked();
            default:
                return true;
        }
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