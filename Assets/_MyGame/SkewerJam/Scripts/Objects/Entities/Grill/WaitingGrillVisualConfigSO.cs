using UnityEngine;

namespace MyGame.SkewerJam.Objects.Entities
{
    [CreateAssetMenu(fileName = "WaitingGrillVisualConfigSO", menuName = "MyGame/SkewerJam/Configs/Animations/WaitingGrillVisualConfigSO")]
    public class WaitingGrillVisualConfigSO : ScriptableObject
    {
        public float shakeDuration = 0.5f;
        public float yDelta = 1f;
        public AnimationCurve curve;


        [Header("Warning")]
        public float warningDuration = 0.5f;
        public Color warningColor = new Color(0.7f, 0.0f, 0.0f, 1.0f);
    }
}