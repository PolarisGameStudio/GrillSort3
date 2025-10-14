using DG.Tweening;
using SonatFramework.Scripts.Utils;
using UnityEngine;

namespace SkewerJam.Gameplay.Effect
{
    public class BoosterAnim_BoosterAddPlate : BoosterAnim
    {
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
            base.SetData(position);

            SonatUtils.DelayCall(delay, () =>
            {
                transform.DOMoveX(targetPosition.x, moveDuration).SetEase(moveCurveX);
                transform.DOMoveY(targetPosition.y, moveDuration).SetEase(moveCurveY);
                transform.DOScale(targetScale, scaleDuration).SetEase(scaleCurve);
            }, this);
        }
    }
}