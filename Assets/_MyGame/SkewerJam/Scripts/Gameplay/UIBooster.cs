using Cysharp.Threading.Tasks;
using MyGame.SkewerJam.Utils;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Templates.UI.ScriptBase;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Booster
{
    [RequireComponent(typeof(Canvas))]
    public class UIBooster : UIBoosterBase
    {
        [Header("UIBooster")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject suggestObj;

        public override void ClickBooster()
        {
            var boosterManager = GameController.Instance.GameLogicHandler.BoosterManager;
            if (GameController.Instance.CheckBlockUI())
            {
                if (boosterManager.IsForceUseBooster(boosterType) == false) return;
            }

            if (usingBooster) return;

            if (boosterService.Instance.CanUseBooster(boosterType))
            {
                var (canUse, reason) = boosterManager.CanUseBooster(boosterType);
                if (canUse)
                {
                    UseBooster();
                }
                else
                {
                    if (string.IsNullOrEmpty(reason) == false)
                    {
                        PopupToast.Cretate(reason);
                    }
                }
            }
            else
            {
                if (unlocked) OnOutOfBooster();
                else
                {
                    BoosterLockFeedback();
                }
            }
        }
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
            var success = await gameLogicHanlder.BoosterManager.UseBooster(boosterType, transform.position);

            usingBooster = false;
            if (success)
            {
                OnUseBoosterSuccess();
            }
        }

        public override void OnUseBoosterSuccess()
        {
            base.OnUseBoosterSuccess();

            var boosterManager = GameController.Instance.GameLogicHandler.BoosterManager;
            boosterManager.SetForceUseBooster(GameResource.None);

            SetSortingOrder(false);

        }

        public void SetSuggest(bool suggest)
        {
            suggestObj.SetActive(suggest);
        }

        public void SetSortingOrder(bool enable, string sortingLayerName = LayerManager.UI, int sortingOrder = 0)
        {
            if (enable)
            {
                canvas.overrideSorting = true;
                canvas.sortingLayerName = sortingLayerName;
                canvas.sortingOrder = sortingOrder;
                SetSuggest(true);
            }
            else
            {
                canvas.overrideSorting = false;
                SetSuggest(false);
            }
        }
    }
}
