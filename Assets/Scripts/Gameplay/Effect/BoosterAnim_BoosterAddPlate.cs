using DG.Tweening;
using SonatFramework.Scripts.Utils;
using Spine.Unity;
using UnityEngine;

namespace SkewerJam.Gameplay.Effect
{
    public class BoosterAnim_BoosterAddPlate : BoosterAnim
    {
        [Space]
        [Header("Booster Add Plate")]
        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField] private float delay = 0.5f;
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private AnimationCurve moveCurveX;
        [SerializeField] private AnimationCurve moveCurveY;
        [SerializeField] private Vector3 targetScale;

        private Vector3 targetPosition;
        public void SetTargetPosition(Vector3 position)
        {
            targetPosition = position;
        }

        public override void SetData(Vector3 position)
        {
            // base.SetData(position);

            SonatUtils.DelayCall(delay, () =>
            {
                skeletonGraphic.AnimationState.SetAnimation(0, "Out", false);
                transform.DOMoveX(targetPosition.x, moveDuration).SetEase(moveCurveX);
                transform.DOMoveY(targetPosition.y, moveDuration).SetEase(moveCurveY);
                // transform.DOScale(targetScale, scaleDuration).SetEase(scaleCurve);
            }, this);
        }
    }
}