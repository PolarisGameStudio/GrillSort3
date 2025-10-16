using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Loading
{
    public class LogoAnimation : MonoBehaviour
    {
        public float EndDuration { get; } = 0.9f;
        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField] private float delay = 0.25f;
        private Coroutine _playAnimationCoroutine;
        private bool _isCompleted = false;
        private float _delayEnd = 0;

        private void OnEnable()
        {
            skeletonGraphic.gameObject.SetActive(false);
            _isCompleted = false;
            _playAnimationCoroutine = StartCoroutine(PlayAnimation());
        }

        private void OnDisable()
        {
            StopCoroutine(_playAnimationCoroutine);
            _playAnimationCoroutine = null;
        }

        private IEnumerator PlayAnimation()
        {
            yield return new WaitForSeconds(delay);

            skeletonGraphic.gameObject.SetActive(true);
            // skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.AnimationState.SetAnimation(0, "Appear", false).Complete += (track) =>
            {
                skeletonGraphic.AnimationState.SetAnimation(0, "Idle", true);
            };

            yield return new WaitUntil(() => _isCompleted);
            yield return new WaitForSeconds(_delayEnd);

            skeletonGraphic.AnimationState.SetAnimation(0, "End", false);
        }

        public void SetCompleted()
        {
            _isCompleted = true;
        }

        public void SetDelayEnd(float delay)
        {
            _delayEnd = delay;
        }
    }
}