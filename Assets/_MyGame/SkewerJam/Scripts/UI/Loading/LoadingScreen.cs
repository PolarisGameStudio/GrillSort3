using SonatFramework.Scripts.Utils;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Loading
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private LogoAnimation logoAnimation;

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        public void SetTime(float time)
        {
            SonatUtils.DelayCall(time - logoAnimation.EndDuration, () =>
            {
                logoAnimation.SetCompleted();
            }, this);

            SonatUtils.DelayCall(time, () =>
            {
                gameObject.SetActive(false);
            }, this);
        }
    }
}