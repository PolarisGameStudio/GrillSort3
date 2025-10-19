using SonatFramework.Scripts.Utils;
using Spine.Unity;
using UnityEngine;

namespace MyGame.UI
{
    [RequireComponent(typeof(SkeletonGraphic))]
    public class UILoopAnimation : MonoBehaviour
    {
        [SerializeField] private string animationName1;
        [SerializeField] private string animationName2;

        [SerializeField] private float delay;
        private SkeletonGraphic skeletonGraphic;

        private void OnEnable()
        {
            skeletonGraphic = GetComponent<SkeletonGraphic>();


            LoopAnimation();

        }

        private void LoopAnimation()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, animationName1, true);
            SonatUtils.DelayCall(delay, () =>
            {
                skeletonGraphic.AnimationState.SetAnimation(0, animationName2, false).Complete += (track) =>
                {
                    LoopAnimation();
                };
            }, this);
        }


    }
}
