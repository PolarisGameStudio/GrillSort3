using System.Collections;
using System.Collections.Generic;
using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame
{
    [CreateAssetMenu(fileName = "ProfileInGameService", menuName = "MyGame/SkewerJam/Features/ProfileInGame/ProfileInGameService")]
    public class ProfileInGameService : MyService<ProfileInGameConfigSO>
    {
        public override string DATA_KEY => "PROFILE_IN_GAME";

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

        }

        protected override void ResetData()
        {


        }
        #endregion

        private void OnLevelStarted(LevelStartedEvent eventData)
        {
            // do nothing
        }
    }
}
