using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameService", menuName = "MyGame/Features/ProfileInGame/ProfileInGameService")]
    public class ProfileInGameService : MyService<ProfileInGameConfigSO>
    {
        public override string DATA_KEY => "PROFILE_IN_GAME";
        public ProfileInGameInventoryModule inventoryModule;

        public int pr => inventoryModule.GetPR();
        #region Init
        protected override void Init()
        {
            new EventBinding<LevelStartedEvent>(OnLevelStarted);
        }

        protected override void LoadConfig()
        {

        }

        protected override void LoadData()
        {
            inventoryModule.LoadData();
        }

        protected override void ResetData()
        {
            inventoryModule.ResetData();

        }
        #endregion

        #region Event Handlers
        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            // do nothing
        }
        #endregion

        public int GetRankPR()
        {
            if (pr <= 0)
            {
                return 0;
            }

            for (int i = 0; i < config.listPlayerRankConfigs.Count; i++)
            {
                if (pr >= config.listPlayerRankConfigs[i].prMileStone)
                {
                    return config.listPlayerRankConfigs[i].rank;
                }
            }

            return config.listPlayerRankConfigs.Count - 1;
        }
    }
}
