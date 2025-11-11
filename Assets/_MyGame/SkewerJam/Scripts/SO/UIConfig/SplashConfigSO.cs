using UnityEngine;

namespace MyGame.SkewerJam.UI.Loading
{
    [CreateAssetMenu(fileName = "SplashConfig", menuName = "MyGame/SkewerJam/Loading/SplashConfigSO")]
    public class SplashConfigSO : ScriptableObject
    {
        [Header("Slider")]
        public float minSliderLoadingTime = 2;
        public float delayStartSlider = 0.25f;
        public float startSpeed = 0.5f;
        public float loadingSpeed = 0.01f;
        public float endSpeed = 1f;

        [Space]
        [Header("Delay")]
        public float delayCompleteGameplay = 0.5f;
        public float delayCompleteHome = 0f;
    }
}