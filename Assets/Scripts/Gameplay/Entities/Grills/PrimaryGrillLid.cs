using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BoosteeManagement;
using Gameplay.LevelData;
using Manager;
using Sonat.Enums;
using SonatFramework.Systems.AudioManagement;
using UnityEngine;

namespace Gameplay.Entities.Grills
{
    public class PrimaryGrillLid : PrimaryGrill
    {
        private int itemCondition;
        [SerializeField] private SpriteRenderer iconItem;

        public override async UniTask SetData(GrillData grillData)
        {
            lockState = 1;
            GrillLidData grillLidData = (GrillLidData)grillData;
            itemCondition = grillLidData.itemCondition;
            iconItem.SetSpriteAsync(PathManager.ItemSprite((ItemId)itemCondition)).Forget();
            base.SetData(grillData);
            grillVisual.CloseGrill(false);
            SetLockItems(true);
        }

        public override void OpenGrillWhenStart()
        {
        }

        public override bool CanShuffle()
        {
            if (IsLock) return false;
            return true;
        }

        public override ShuffleLayerData GetShuffleLayerData()
        {
            if (IsLock)
                return null;
            else
            {
                return base.GetShuffleLayerData();
            }
        }

        public override List<ShuffleLayerData> GetSubsShuffleLayerData()
        {
            if (IsLock)
                return null;
            return base.GetSubsShuffleLayerData();
        }

        private void OnCollectItem(int itemId)
        {
            if (!IsLock) return;
            if (itemId == itemCondition)
            {
                Unlock();
            }
        }

        public override void Unlock()
        {
            base.Unlock();
            grillVisual.OpenGrill();
            grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);

            var rand = Random.Range((int) AudioId.Pot_lid_Close_01_Grill3, (int) AudioId.Pot_lid_Close_03_Grill3 + 1);
            var audioId = (AudioId)rand;
            MySonatFramework.GetService<AudioService>().PlaySound(audioId);
        }

        public override void OnCreateObj(params object[] args)
        {
            base.OnCreateObj(args);
            grillBaseBehaviorSO.eventSystemSO.RegisterEvents_OnCollectItem(OnCollectItem);
        }

        public override void OnReturnObj()
        {
            base.OnReturnObj();
            grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);
        }

        private void OnDisable()
        {
            grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);
        }
    }
}