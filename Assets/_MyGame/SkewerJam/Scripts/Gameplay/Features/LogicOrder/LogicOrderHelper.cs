using MyGame.Modules.ProfileInGame;
using Sonat.Enums;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public static class LogicOrderHelper
    {
        public static (LevelDifficulty difficulty, int difficultyValue) GetDynamicDifficultyAndValue(LevelDifficulty difficulty, int difficultyValue, int changeDifficultyValue)
        {
            var config = MySonatFramework.GetService<ProfileInGameService>().GetConfig();
            var maxValue = config.GetMaxValue(difficulty);
            var newValue = difficultyValue + changeDifficultyValue;

            while (newValue > maxValue)
            {
                newValue = newValue - maxValue;
                difficulty += 1;
                if (difficulty >= LevelDifficulty.MAX)
                {
                    newValue = maxValue;
                    difficulty = LevelDifficulty.MAX - 1;
                    break;
                }

                maxValue = config.GetMaxValue(difficulty);
            }

            while (newValue < 0)
            {
                var preMaxValue = config.GetMaxValue(difficulty - 1);
                newValue = preMaxValue + newValue;
                difficulty -= 1;

                if (difficulty < LevelDifficulty.Easy)
                {
                    newValue = 0;
                    difficulty = LevelDifficulty.Easy;
                    break;
                }

                maxValue = config.GetMaxValue(difficulty);
            }

            return (difficulty, newValue);
        }
    }
}