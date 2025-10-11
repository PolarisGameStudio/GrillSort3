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
    }
}