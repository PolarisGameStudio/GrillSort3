using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BoosteeManagement;
using Gameplay.Entities;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.SettingsManagement.Vibation;
using UnityEngine;
using static PopupUnlockInGame;

namespace MyGame.SkewerJam.Objects.Entities
{
    public class WaitingGrill : GrillBase
    {
        [SerializeField] private WaitingGrillVisual waitingGrillVisual;
        public WaitingGrillVisual Visual => waitingGrillVisual;


        private bool isActive = false;
        public bool IsActive => isActive;


        #region Implementations
        public override EntityType entityType => EntityType.PrimaryGrill;

        public override void ChangeItem(global::SlotBase slot, int newId)
        {

        }

        public override bool CheckItemWithId(int id)
        {
            return false;
        }

        public override Vector3 DestroyItem(int id)
        {
            return Vector3.zero;
        }

        public override ShuffleLayerData GetMagnetLayerData()
        {
            return null;
        }

        public override ShuffleLayerData GetShuffleLayerData()
        {
            return null;
        }

        public override void SetShuffleLayerData(LayerData layerData)
        {

        }
        #endregion

        public void SetActive(bool isActive)
        {
            this.isActive = isActive;
            waitingGrillVisual.SetActive(isActive);
        }

        #region Interactions
        private void Update()
        {
            // var popup = PanelManager.Instance.GetPanel<PopupUnlock_SkewerJam>();
            if (GameController.Instance.CheckBlockUI() == false && isActive == false && Input.GetMouseButtonDown(0))
            {
                var hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                foreach (var hit in hits)
                {
                    if (hit.gameObject == gameObject)
                    {
                        OpenPopupUnlock();
                        break;
                    }
                }
            }
        }


        private void OpenPopupUnlock()
        {
            var uiData = new UIData();
            uiData.Add("SelectedObjectType", SelectedObjectType.Plate);
            uiData.Add("Price", GameController.Instance.GameConfig.unlockPlatePrice);
            uiData.Add("OnSuccess", (Action)(() =>
            {
                var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
                waitingGrillManager.Unlock(this);
                EventBus<UseBoosterEvent>.Raise(new UseBoosterEvent() { booster = GameResource.BuffAddPlate });
            }));
            PanelManager.Instance.OpenPanel<PopupUnlockInGame>(uiData);
        }
        #endregion

        public async UniTask PlayUnlock(bool addLockedWaitingGrill = true)
        {
            SetActive(true);

            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            if (addLockedWaitingGrill)
            {
                var lockedWaitingGrill = await waitingGrillManager.AddWaitingGrill(false);
                lockedWaitingGrill.transform.localScale = Vector3.zero;
                await waitingGrillManager.AlignObjects(() =>
                {
                    lockedWaitingGrill.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutSine);
                });
            }
            else
            {
                await waitingGrillManager.AlignObjects(() =>
                {
                });
            }
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public void ClearItem()
        {
            var item = GetSlot(0).GetItem();

            if (item != null)
            {
                transform.DOShakePosition(0.5f, 0.1f, 25, 90).SetDelay(0.5f);
                item.transform.DOScale(0, 0.75f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    GetSlot(0).SetItem(null);
                    MySonatFramework.GetService<VibrationService>().Vibrate(50);
                    MyGame.SkewerJam.Gameplay.GameFactory.Instance.ReturnEntity(item);
                }).SetDelay(1f);
            }
        }
    }

}