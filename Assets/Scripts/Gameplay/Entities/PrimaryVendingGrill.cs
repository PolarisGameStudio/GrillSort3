using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.BoosteeManagement;
using Gameplay.Entities.GrillScripts;
using Gameplay.LevelData;
using UnityEngine;

namespace Gameplay.Entities
{
    public class PrimaryVendingGrill : PrimaryGrill
    {
        private int numLayer;
        private GrillVendingVisual grillVendingVisual => grillVisual as GrillVendingVisual;

        public override async UniTask SetData(GrillData grillData)
        {
            base.SetData(grillData);
            numLayer = grillData.layer.Count;
            lockState = 0;
        }

        public override bool CanShuffle()
        {
            return false;
        }

        public override ShuffleLayerData GetShuffleLayerData()
        {
            return null;
        }

        public override List<ShuffleLayerData> GetSubsShuffleLayerData()
        {
            return null;
        }

        public override SlotBase GetNearestSlot(Vector3 position)
        {
            if (IsLock) return null;
            return base.GetNearestSlot(position);
        }

        protected override void UpdateSubGrills()
        {
            if (numLayer <= 0) return;
            numLayer--;
            grillVendingVisual.UpdateLayer(numLayer);
            if (numLayer == 0)
            {
                SetLockItems(true);
                grillVisual.CloseGrill();
                // // SonatUtils.DelayCall(0.5f, grillVisual.CloseGrill, this);
                // GameplayController.OnActionLockGrill?.Invoke(this);
            }

            base.UpdateSubGrills();
        }

        public override void RemoveSubGrill(SubGrill subGrill)
        {
            base.RemoveSubGrill(subGrill);
            numLayer = subGrills.Count + 1;
            grillVendingVisual.SetLayer(numLayer);
            if (numLayer == 0)
            {
                SetLockItems(true);
                grillVisual.CloseGrill();
                // // SonatUtils.DelayCall(0.5f, grillVisual.CloseGrill, this);
                // GameplayController.OnActionLockGrill?.Invoke(this);
            }
        }

        protected override async UniTask<SubGrill> CreateSubGrill(int layer)
        {
            Vector3 pos = subContainer.position + Vector3.up * layer * 0.035f + Vector3.back * layer * 0.03f + subOffset;
            string subGrillName = "SubGrillVending";
            return await grillBaseBehaviorSO.gameFactorySO.CreateItem<SubGrill>(subGrillName, pos, subContainer);
        }

        public override bool CreateSpecialItem(int itemId, ItemType itemType)
        {
            return false;
        }


        public override void CheckSubGrills(bool forceShowFirst = true)
        {
            if (numLayer <= 0) return;
            base.CheckSubGrills(forceShowFirst);
            numLayer = subGrills.Count + 1;
            grillVendingVisual.SetLayer(numLayer);
            if (numLayer == 0)
            {
                SetLockItems(true);
                grillVisual.CloseGrill();
                //  // SonatUtils.DelayCall(0.5f, grillVisual.CloseGrill, this);
                // GameplayController.OnActionLockGrill?.Invoke(this);
            }
        }

        public override void Unlock()
        {
            base.Unlock();
            grillVendingVisual.Unlock();
        }

        public override async UniTask UndoUpdateSubGrills(GrillData preGrillData)
        {
            var currentGrillData = GetGrillData();

            var isClosed = currentGrillData.layer.Count == 0 || slots.All(slot => slot.isEmpty());
            if (currentGrillData.layer.Count != preGrillData.layer.Count || isClosed)
            {
                // ẩn subgrill hiện tại
                if (subGrills != null && subGrills.Count > 0)
                    subGrills[0].Hide();

                if (isClosed)
                {
                    grillVendingVisual.OpenGrill(true, true);
                }
                else
                {
                    // tạo subgrill mới để undo
                    var subDataToUndo = currentGrillData.layer[0];
                    await CreateSubGrillToUndo(subDataToUndo);
                }
                numLayer = subGrills.Count + 1;
                grillVendingVisual.SetLayer(numLayer);

            }
        }
    }
}