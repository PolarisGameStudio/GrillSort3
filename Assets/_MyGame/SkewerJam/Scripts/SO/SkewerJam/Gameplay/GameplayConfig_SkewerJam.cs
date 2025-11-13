using System;
using System.Collections.Generic;
using Sonat.Enums;
using SonatFramework.Scripts.Gameplay;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.SkewerJam.Scripts.SO.SkewerJam.Gameplay
{
    [CreateAssetMenu(menuName = "MyGame/SkewerJam/Game Config", fileName = "GameplayConfig_SkewerJam")]
    public class GameplayConfig_SkewerJam : GamePlayConfig
    {
        [Header("Unlock")]
        public ResourceData unlockTrayPrice;
        public ResourceData unlockPlatePrice;
        public ResourceData unlockOctoChefPrice;
        public ResourceData skipBombPrice;
        public ResourceData unlockLockObstaclePrice;

        [Header("Level Difficulty")]
        public List<DifficultyAndMaxValue> listDifficultyAndMaxValues;
    }



    [Serializable]
    public class DifficultyAndMaxValue
    {
        public LevelDifficulty difficulty;
        public int maxValue;
    }
}