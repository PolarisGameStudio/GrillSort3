using System.Linq;
using MyGame.Modules.ProfileInGame.Data;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameInventoryModule", menuName = "MyGame/Features/ProfileInGame/ProfileInGameInventoryModule")]
    public class ProfileInGameInventoryModule : BaseInventoryModule<ProfileInGameService, ProfileInGameConfigSO>
    {
        public override string DATA_KEY => "PROFILE_IN_GAME_INVENTORY_MODULE";

        private ListDataPref<int> _listPreviousLevelPRs;
        private ClassDataPref<LevelInfoData> _currentLevelInfoData;

        public override void LoadData()
        {
            _listPreviousLevelPRs = new ListDataPref<int>($"{DATA_KEY}_ListPreviousLevelPRs");
        }

        private void LoadCurrentLevelInfoData(int level)
        {
            var curLevel = MySonatFramework.userDataService.GetLevel();
            if (_currentLevelInfoData == null)
            {
                _currentLevelInfoData = new ClassDataPref<LevelInfoData>($"{DATA_KEY}_LevelInfoData", new LevelInfoData(curLevel, config.defaultPerformanceRate));
            }

            if (level != curLevel)
            {
                _currentLevelInfoData.Value = new LevelInfoData(curLevel, config.defaultPerformanceRate);
            }
        }

        public override void ResetData()
        {
            _listPreviousLevelPRs.Clear();

            _currentLevelInfoData = new ClassDataPref<LevelInfoData>($"{DATA_KEY}_LevelInfoData", new LevelInfoData(0, config.defaultPerformanceRate));
            _currentLevelInfoData.Clear();
            _currentLevelInfoData = null;
        }

        public int GetPR()
        {
            if (_currentLevelInfoData != null)
            {
                return _currentLevelInfoData.Value.startPr;
            }
            else
            {
                _currentLevelInfoData = new ClassDataPref<LevelInfoData>($"{DATA_KEY}_LevelInfoData", new LevelInfoData(0, config.defaultPerformanceRate));
                return _currentLevelInfoData.Value.startPr;
            }
        }


        public void UpdateLevelStarted(int level)
        {
            LoadCurrentLevelInfoData(level);

            var temp = _currentLevelInfoData.Value;
            temp.startCount += 1;
            if (temp.startCount > 1)
            {
                temp.winStreak = 0;
            }
            _currentLevelInfoData.Value = temp;

        }

        public void UpdateLevelEnded(bool success)
        {
            if (success)
            {
                var pr = ComputePR();
                if (_listPreviousLevelPRs.Value.Count >= config.accumulatedLevelCount)
                {
                    _listPreviousLevelPRs.RemoveAt(0);
                }
                _listPreviousLevelPRs.Add(pr);

                // UPDATE current level info data
                var temp = _currentLevelInfoData.Value;
                var nextPr = config.defaultPerformanceRate;
                var checkStartCollectData = _listPreviousLevelPRs.Value.Count >= config.accumulatedLevelCount;
                if (checkStartCollectData)
                {
                    nextPr = Mathf.RoundToInt(GetNextPr());
                    nextPr = Mathf.Max(nextPr, 0);
                }
                temp.NextLevel(nextPr);
                _currentLevelInfoData.Value = temp;
            }
        }

        public void UpdateReviveUsed()
        {
            var temp = _currentLevelInfoData.Value;
            temp.countRevive += 1;
            _currentLevelInfoData.Value = temp;
        }

        public void UpdateBoosterUsed(GameResource booster)
        {
            var temp = _currentLevelInfoData.Value;
            if (!temp.resourceData.ContainsKey(booster))
            {
                temp.resourceData.Add(booster, 0);
            }
            temp.resourceData[booster] += 1;
            _currentLevelInfoData.Value = temp;
        }

        public int ComputePR()
        {
            var data = _currentLevelInfoData.Value;
            var startPr = data.startPr;
            var winStreak = data.winStreak;

            var logicOrderHandler = GameController.Instance.GameLogicHandler.OrderManager.LogicOrderHandler;
            var logicOrderData = logicOrderHandler.LogicOrderData;



            return startPr
                + winStreak * config.winStreakPRConfigSO.GetPRByWinStreak(winStreak)
                + data.countRevive * config.revivePRConfigSO.GetPR()
                + data.resourceData.Sum(x => x.Value * config.boosterPRConfigSO.GetPRByBooster(x.Key))
                + config.startCountPRConfigSO.GetPRByStartCount(logicOrderData.level, logicOrderData.difficulty, logicOrderData.difficultyValue, data.startCount);
        }

        public float GetNextPr()
        {
            var sum = _listPreviousLevelPRs.Value.Sum();
            var count = _listPreviousLevelPRs.Value.Count;
            return sum * 1.0f / count;
        }
    }
}