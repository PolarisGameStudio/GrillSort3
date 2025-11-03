using UnityEngine;

namespace MyGame.Modules.CardCollection.ReceiveCardEffect.Configs
{
    [CreateAssetMenu(fileName = "PopupReceiveCardBaseConfigSO", menuName = "MyGame/SkewerJam/Features/CardCollection/AnimConfigs/PopupReceiveCardBaseConfigSO")]
    public class PopupReceiveCardBaseConfigSO : ScriptableObject
    {
        public float scaleDownCardDuration = 0.3f;
        public float delayBeforeWidgetMoveIn = 0.3f;
        public float delayBeforeWidgetMoveOut = 0.3f;
        public float delayBeforeFirstFlyStar = 0.3f;
        public float delayBetweenFlyStar = 0.1f;
        public float delayBeforeHideNewCard = 0.3f;
    }
}