using UnityEngine;

namespace MyGame.Modules.CardCollection.Animation
{
    [CreateAssetMenu(fileName = "PopupReceiveCardImmediatelyConfigSO", menuName = "MyGame/SkewerJam/Features/CardCollection/AnimConfigs/PopupReceiveCardImmediatelyConfigSO")]
    public class PopupReceiveCardImmediatelyConfigSO : ScriptableObject
    {
        public bool forceHideAnim = false;
        public float animLifeTime = 1.95f;
        public float delayAppearPs = 0.05f;
        public float delayBeforePlayCardParticle = 0.35f;
        public float delaySoundAppearAnim = 0.5f;
    }
}