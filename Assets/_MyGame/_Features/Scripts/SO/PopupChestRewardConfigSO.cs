using UnityEngine;

namespace MyGame.Modules.Scripts.SO
{
    [CreateAssetMenu(fileName = "PopupChestRewardConfigSO", menuName = "MyGame/SkewerJam/Features/PopupChestRewardConfigSO")]
    public class PopupChestRewardConfigSO : ScriptableObject
    {
        
        public float delayMovePs = 0.7f;

        [Header("Move Item")]
        public float delaySpawnItem = 0.4f;
        public float durationMoveItem = 0.38f;
        public float delayMoveItem = 0.1f;
        public float durationFadeItem = 0.1f;
        public AnimationCurve curveMoveItemX;
        public AnimationCurve curveMoveItemY;
        public AnimationCurve curveMoveItemScale;
        public float delayMovePsAfter = 0.685f;

        [Header("Scale Out Item")]
        public float delayScaleOutItem = 0f;
        public float durationScaleOutItem = 0.35f;
        public AnimationCurve curveScaleOutItem;
        public float delayItemOut = 0.1f;
        public float delayPsItemOut = 0.2f;

        [Header("Pumpkin out")]
        public float delayPumpkinOut = 0.6f;
        public float durationPumpkinOut = 0.45f;
        public AnimationCurve curvePumpkinOut;

        [Header("Close")]
        public float delayClose = 0.5f;
    }
}