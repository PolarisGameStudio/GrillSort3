using System;
using System.Collections.Generic;
using MyGame.Modules.ProfileInGame.Data;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameInventoryModule", menuName = "MyGame/Features/ProfileInGame/ProfileInGameInventoryModule")]
    public class ProfileInGameInventoryModule : BaseInventoryModule<ProfileInGameService, ProfileInGameConfigSO>
    {
        public override string DATA_KEY => "PROFILE_IN_GAME_INVENTORY_MODULE";

        private IntDataPref pr;
        private Queue<LevelInfoData> levelInfoDataQueue;

        public override void LoadData()
        {
            pr = new IntDataPref($"{DATA_KEY}_PR");
            levelInfoDataQueue = new Queue<LevelInfoData>();

            var level = MySonatFramework.userDataService.GetLevel();
            for (int i = level - config.accumulatedLevelCount; i < level; i++)
            {
                var levelInfoData = new LevelInfoData();
                levelInfoData.level = i;
                levelInfoData.loseStreak = 0;
                levelInfoData.countRevive = 0;
                levelInfoData.resourceData = new Dictionary<GameResource, int>();
            }
        }

        public override void ResetData()
        {
            pr.Value = config.defaultPerformanceRate;
            levelInfoDataQueue.Clear();
        }

        public int GetPR()
        {
            return 0;
        }
    }
}