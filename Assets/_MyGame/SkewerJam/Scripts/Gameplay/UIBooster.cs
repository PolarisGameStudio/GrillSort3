using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Templates.UI.ScriptBase;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Booster
{
    public class UIBooster : UIBoosterBase
    {
        public override void OnOutOfBooster()
        {
            UIData uiData = new UIData();
            uiData.Add("booster_config", config);
            PanelManager.Instance.OpenPanelByName<PopupBuyBoosterBase>("PopupBuyBooster_SkewerJam", uiData);
        }

        protected override void BoosterLockFeedback()
        {
            base.BoosterLockFeedback();
            
            PopupToast.Cretate("Unlock at level " + config.levelUnlock);
        }

        public override void UseBooster()
        {
            base.UseBooster();

            var boosterType = config.booster;
            UseBoosterAsync(boosterType).Forget();
        }

        private async UniTask UseBoosterAsync(GameResource boosterType)
        {
            var gameLogicHanlder = GameController.Instance.GameLogicHandler;
            await gameLogicHanlder.BoosterManager.UseBooster(boosterType);
            OnUseBoosterSuccess();
        }
    }
}
