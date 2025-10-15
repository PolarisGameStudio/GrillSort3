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
        public static int itemBombLimit;
        public static bool popupUnlockTray;
        public static int numberItemCanMergeAfterShuffle = 5;
        public static int levelShowInterLose;
        public static int levelShowInterWin;
        public static int levelShowInterReplay;
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


        public static int showRewardAdsLevel;

        public static void LoadData()
        {
            showRewardAdsLevel = SonatSDKAdapter.GetRemoteInt("show_reward_ads_level", 5);
            levelForceHome = SonatSDKAdapter.GetRemoteInt("level_force_home", 5);

            levelShowInterLose = SonatSDKAdapter.GetRemoteInt("level_show_inter_lose", 3);
            levelShowInterReplay = SonatSDKAdapter.GetRemoteInt("level_show_inter_replay", 3);


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