using System;
using System.Collections.Generic;
using MyGame.Modules.ProfileInGame.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameConfigSO", menuName = "MyGame/Features/ProfileInGame/ProfileInGameConfigSO")]
    public class ProfileInGameConfigSO : MyServiceConfigSO
    {
        public int defaultPerformanceRate = 1500;
        public int accumulatedLevelCount = 5;

        [Space(10)]
        [Header("Player Rank Configs")]
        public List<PlayerRankConfig> listPlayerRankConfigs;

        [Space(10)]
        [Header("PR Configs")]
        public LoseStreakPRConfigSO loseStreakPRConfigSO;
        public WinStreakPRConfigSO winStreakPRConfigSO;
        public BoosterPRConfigSO boosterPRConfigSO;
        public RevivePRConfigSO revivePRConfigSO;

        #region OnValidate
        public void OnValidate()
        {
            if (listPlayerRankConfigs.Count > 0)
            {
                listPlayerRankConfigs[0].rank = 0;
                listPlayerRankConfigs[0].prMileStone = 0;
            }
            foreach (var playerRankConfig in listPlayerRankConfigs)
            {
                playerRankConfig.rank = listPlayerRankConfigs.IndexOf(playerRankConfig);
            }
        }
        #endregion
    }

    [Serializable]
    public class PlayerRankConfig
    {
        [GUIColor(0, 1, 0)]
        [ReadOnly]
        public int rank;
        public int prMileStone;
        public int changeDifficultyValue = 1;
    }
}