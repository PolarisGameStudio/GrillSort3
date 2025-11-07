using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sonat.Debugger;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using UnityEngine;

namespace Manager
{
    public static class GameRemoteConfigValue
    {
        public static bool showNativeAds;
        public static int levelForceHome;
        public static int levelAppearLuckySpin;
        public static int levelBeginReceivePiggyPoint;
        public static AudioId bgrMusic;
        public static bool forceTutBooster;
        public static int numberSpecialItemPerLevel;
        public static int iceGrillStep = 4;
        public static int iceGrillStep_SkewerJam = 6;
        public static int itemBombLimit = 10;
        public static bool popupUnlockTray;
        public static int numberItemCanMergeAfterShuffle = 5;
        public static int levelShowInterGoHome;
        public static int levelShowInterReplay;
        public static int levelShowInterLose;

        public static bool localizeJapan;
        public static int countLoseToShowOffer;
        public static bool noCharacter;
        public static int timeRevive;
        public static bool noOrder;
        public static int maxPopupMO;
        public static int coinRevive;

        public static bool shuffleGrills;

        //public static bool noRewarded;
        public static bool swapItem;
        public static bool bonusTime;
        public static bool shuffleItemIds;
        public static LevelReplayData levelReplayData;


        public static int showRewardAdsLevelAddOrder;
        public static int showRewardAdsLevelAddPlate;
        // public static int showRewardAdsLevelAddPackBooster;
        public static int levelStartShowPopupBuyBooster2;
        public static bool activeProgressLose;
        public static int levelStartShowBanner;
        public static int levelShowRwdBooster;

        public static int levelStartRate;
        public static int minNumStar;

        public static void LoadData()
        {
            showRewardAdsLevelAddOrder = SonatSDKAdapter.GetRemoteInt("show_reward_ads_level_add_order", 9999);
            showRewardAdsLevelAddPlate = SonatSDKAdapter.GetRemoteInt("show_reward_ads_level_add_plate", 9999);
            // showRewardAdsLevelAddPackBooster = SonatSDKAdapter.GetRemoteInt("show_reward_ads_level_add_pack_booster", 9999);

            levelForceHome = SonatSDKAdapter.GetRemoteInt("level_force_home", 6);

            levelShowInterReplay = SonatSDKAdapter.GetRemoteInt("level_show_inter_replay", 9999);
            levelShowInterGoHome = SonatSDKAdapter.GetRemoteInt("level_show_inter_go_home", 9999);
            levelShowInterLose = SonatSDKAdapter.GetRemoteInt("level_show_inter_lose", 9999);

            activeProgressLose = SonatSDKAdapter.GetRemoteBool("active_progress_lose", true);
            levelStartShowBanner = SonatSDKAdapter.GetRemoteInt("level_start_show_banner", 9999);
            levelShowRwdBooster = SonatSDKAdapter.GetRemoteInt("level_show_rwd_booster", 1);
            levelStartShowPopupBuyBooster2 = SonatSDKAdapter.GetRemoteInt("level_start_show_popup_buy_booster2", 9999);

            // Popup Rate
            levelStartRate = SonatSDKAdapter.GetRemoteInt("POPUP_RATE_level_start", 3);
#if UNITY_ANDROID
            minNumStar = SonatSDKAdapter.GetRemoteInt("POPUP_RATE_min_num_star", 5);
#elif UNITY_IOS
            minNumStar = SonatSDKAdapter.GetRemoteInt("POPUP_RATE_min_num_star", 1);
#endif

            // In Game
            numberSpecialItemPerLevel = SonatSDKAdapter.GetRemoteInt("IN_GAME_number_special_item_per_level", 3);

            countLoseToShowOffer = SonatSDKAdapter.GetRemoteInt("count_lose_to_show_offer", 0);
            noCharacter = SonatSDKAdapter.GetRemoteBool("no_character", true);
            timeRevive = SonatSDKAdapter.GetRemoteInt("time_revive", 45);
            noOrder = SonatSDKAdapter.GetRemoteBool("no_order", false);
            maxPopupMO = SonatSDKAdapter.GetRemoteInt("max_popup_mo", 1);
            coinRevive = SonatSDKAdapter.GetRemoteInt("coin_revive", 150);
            shuffleGrills = SonatSDKAdapter.GetRemoteBool("shuffle_grills", true);
            //noRewarded = SonatSDKAdapter.GetRemoteBool("no_rewarded", false);
            swapItem = SonatSDKAdapter.GetRemoteBool("swap_item", false);
            bonusTime = SonatSDKAdapter.GetRemoteBool("bonus_time", false);
            shuffleItemIds = SonatSDKAdapter.GetRemoteBool("shuffle_item_ids", false);

            LevelReplayData levelReplayDataDefault = new LevelReplayData();
            levelReplayDataDefault.levelReplayData.Add(6, new Dictionary<int, int>() { { 1, 0 }, { 3, 1 }, { 5, 2 } });
            levelReplayDataDefault.levelReplayData.Add(7, new Dictionary<int, int>() { { 1, 0 }, { 3, 1 }, { 5, 2 } });
            levelReplayDataDefault.levelReplayData.Add(8, new Dictionary<int, int>() { { 1, 0 }, { 3, 1 }, { 5, 2 } });
            Debug.Log("Level Replay Data: " + JsonConvert.SerializeObject(levelReplayDataDefault));
            levelReplayData = SonatSDKAdapter.GetRemoteConfig("level_replay_data", levelReplayDataDefault);
        }

        public static int GetInt(string key, int defaultValue)
        {
            return SonatSDKAdapter.GetRemoteInt(key, defaultValue);
        }
    }

    public class LevelReplayData
    {
        public Dictionary<int, Dictionary<int, int>> levelReplayData = new Dictionary<int, Dictionary<int, int>>();

        public int GetCategory(int level, int startCount)
        {
            int category = 0;
            if (levelReplayData.TryGetValue(level, out Dictionary<int, int> replayData))
            {
                foreach (var data in replayData)
                {
                    if (data.Key > startCount) break;
                    category = data.Value;
                }
            }

            return category;
        }
    }
}