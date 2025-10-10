using UnityEngine;

namespace MyGame.SkewerJam.Scripts.SO.Configs
{
    [CreateAssetMenu(fileName = "ItemAnimationConfigSO_SkewerJam", menuName = "MyGame/SkewerJam/Configs/Animations/ItemAnimationConfigSO_SkewerJam")]
    public class ItemAnimationConfigSO_SkewerJam : ScriptableObject
    {
        public Vector3 scaleDown = new Vector3(0.8f, 1.1f, 1f);
        public float scaleDuration = 0.05f;
        public bool useSpeed = false;
        public float durationMove = 0.4f;
        public float speed = 1f;
        public AnimationCurve curveX = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveY = AnimationCurve.Linear(0, 0, 1, 1);
    }
}