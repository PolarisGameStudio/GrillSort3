using System.Collections.Generic;
using Sonat.Enums;

namespace MyGame.Modules.ProfileInGame.Data
{
    public class LevelInfoData
    {
        public int level;
        public int loseStreak;
        public int countRevive;

        public Dictionary<GameResource, int> resourceData;
    }
}