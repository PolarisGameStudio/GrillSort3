using System;
using System.Collections.Generic;
using MyGame.Modules.ProfileInGame.Config;
using Sonat.Enums;

namespace MyGame.Modules.ProfileInGame.Data
{
    [Serializable]
    public class LevelInfoData
    {
        public int level;
        public int startPr;
        public int startCount;
        public int countRevive;
        public int winStreak;
        public Dictionary<GameResource, int> resourceData;

        public LevelInfoData()
        {
            level = 0;
            startPr = 0;
            startCount = 0;
            countRevive = 0;
            resourceData = new Dictionary<GameResource, int>();
        }

        public LevelInfoData(int level, int defaultPerformanceRate)
        {
            this.level = level;
            startPr = defaultPerformanceRate;
            startCount = 0;
            countRevive = 0;
            resourceData = new Dictionary<GameResource, int>();
        }

        public void NextLevel(int newPr)
        {
            level += 1;
            startPr = newPr;
            startCount = 0;
            countRevive = 0;
            resourceData = new Dictionary<GameResource, int>();
            winStreak += 1;
        }
    }
}