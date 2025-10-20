using System.Collections;
using System.Collections.Generic;
using SonatFramework.Scripts.Utils;
using Spine.Unity;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonAnimation;
    [SerializeField] private float delay = 1f;
    [SerializeField] private string animationName = "idle";
    [SerializeField] private bool loop = true;

    private void OnEnable()
    {
        // gameObject.SetActive(false);
        skeletonAnimation.enabled = false;
        SonatUtils.DelayCall(delay, () =>
        {
            skeletonAnimation.enabled = true;
            skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
        }, this);
    }
}
