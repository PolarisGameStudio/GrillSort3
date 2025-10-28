using Unity.VisualScripting;
using UnityEngine;

namespace MyGame.SkewerJam.Scripts.SO.Configs
{
    [CreateAssetMenu(fileName = "ItemAnimationConfigSO_SkewerJam", menuName = "MyGame/SkewerJam/Configs/Animations/ItemAnimationConfigSO_SkewerJam")]
    public class ItemAnimationConfigSO_SkewerJam : ScriptableObject
    {
        public Vector3 scaleDown = new Vector3(0.8f, 1.1f, 1f);
        public float scaleDuration = 0.05f;
        public float speed = 0.8f;
        public float minDuration = 0.5f;
        public float thresholdX = 0.1f;
        public float thresholdY = 0.1f;
        public AnimationCurve[] curveX;
        public AnimationCurve[] curveY;

        public AnimationCurve[] curveX_Undo;
        public AnimationCurve[] curveY_Undo;

        public float GetMoveDuration(float distance)
        {
            return Mathf.Max(distance / speed, minDuration);
        }

        public AnimationCurve GetMoveCurveX(float distanceX)
        {
            return curveX[Mathf.Abs(distanceX) > thresholdX ? 1 : 0];
        }

        public AnimationCurve GetMoveCurveY(float distanceY)
        {
            return curveY[Mathf.Abs(distanceY) > thresholdY ? 1 : 0];
        }

        public AnimationCurve GetMoveCurveX_Undo(float distanceX)
        {
            return curveX_Undo[Mathf.Abs(distanceX) > thresholdX ? 1 : 0];
        }

        public AnimationCurve GetMoveCurveY_Undo(float distanceY)
        {
            return curveY_Undo[Mathf.Abs(distanceY) > thresholdY ? 1 : 0];
        }
    }
}