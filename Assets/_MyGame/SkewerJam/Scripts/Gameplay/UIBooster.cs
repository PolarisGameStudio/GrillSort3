using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private Transform suggestTransform;

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

        private Sequence suggestionSequence;
        public void SetSuggest(bool suggest, bool shake = false)
        {
            suggestObj.SetActive(suggest);

            if (shake)
            {
                suggestTransform.DOKill();
                suggestionSequence = DOTween.Sequence()
                    .Append(suggestTransform.DOShakePosition(0.125f, Vector3.right * 2f, randomness: 0).SetEase(Ease.InOutCubic).SetLoops(4, LoopType.Yoyo))
                    .AppendInterval(3f)
                    .SetLoops(-1, LoopType.Restart);
            }
            else
            {
                if (suggestionSequence != null)
                {
                    suggestionSequence.Kill();
                    suggestionSequence = null;
                }
                suggestTransform.localPosition = Vector3.zero;
            }
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
