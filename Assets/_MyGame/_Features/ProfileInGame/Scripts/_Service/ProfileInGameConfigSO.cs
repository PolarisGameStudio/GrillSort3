using System;
using System.Collections.Generic;
using System.Linq;
using MyGame.Modules.ProfileInGame.Config;
using MyGame.SkewerJam.Scripts.SO.SkewerJam.Gameplay;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameConfigSO", menuName = "MyGame/Features/ProfileInGame/ProfileInGameConfigSO")]
    public class ProfileInGameConfigSO : MyServiceConfigSO
    {
        public int defaultPerformanceRate = 1500;
        public int accumulatedLevelCount = 5;
        public int daysToReset = 3;

        [Space(10)]
        [Header("Player Rank Configs")]
        public List<PlayerRankConfig> listPlayerRankConfigs;
        public GameplayConfig_SkewerJam gameplayConfigSO;

        [Space(10)]
        [Header("PR Configs")]
        public StartCountPRConfigSO startCountPRConfigSO;
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

        public int GetMaxValue(LevelDifficulty difficulty)
        {
            var difficultyAndMaxValue = gameplayConfigSO.listDifficultyAndMaxValues.FirstOrDefault(x => x.difficulty == difficulty);
            if (difficultyAndMaxValue == null)
            {
                difficultyAndMaxValue = gameplayConfigSO.listDifficultyAndMaxValues.Last();
            }
            return difficultyAndMaxValue.maxValue;
        }
    }

    [Serializable]
    public class PlayerRankConfig : IComparable<PlayerRankConfig>
    {
        [GUIColor(0, 1, 0)]
        [ReadOnly]
        public int rank;
        public int prMileStone;
        public int changeDifficultyValue = 1;

        public int CompareTo(PlayerRankConfig other)
        {
            return prMileStone.CompareTo(other.prMileStone);
        }
    }
}