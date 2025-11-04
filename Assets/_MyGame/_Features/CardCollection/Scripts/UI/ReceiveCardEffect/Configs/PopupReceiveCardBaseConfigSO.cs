using UnityEngine;

namespace MyGame.Modules.CardCollection.ReceiveCardEffect.Configs
{
    [CreateAssetMenu(fileName = "PopupReceiveCardBaseConfigSO", menuName = "MyGame/SkewerJam/Features/CardCollection/AnimConfigs/PopupReceiveCardBaseConfigSO")]
    public class PopupReceiveCardBaseConfigSO : ScriptableObject
    {
        public float scaleDownCardDuration = 0.3f;
        public float delayBeforeWidgetMoveIn = 1f;
        public float delayBeforeWidgetMoveOut = 1f;
        public float delayBeforeFirstFlyStar = 1.3f;
        public float delayBetweenFlyStar = 0.15f;
        public float delayBeforeHideNewCard = 0.3f;
    }
}