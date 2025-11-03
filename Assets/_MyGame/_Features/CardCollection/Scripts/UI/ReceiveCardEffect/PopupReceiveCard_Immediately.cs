using Cysharp.Threading.Tasks;
using SonatFramework.Scripts.Utils;
using UnityEngine;

namespace MyGame.Modules.CardCollection.Animation
{
    public class PopupReceiveCard_Immediately : PopupReceiveCardBase
    {
        [Header("Immediately")]
        [SerializeField] private PackAnimation packAnim;
        [SerializeField] private ParticleSystem psAppear;
        [SerializeField] private PopupReceiveCardImmediatelyConfigSO configSO;

        protected override async UniTask PlayAppearAnimation()
        {
            // Ẩn các thẻ trước
            for (int i = 0; i < uiCards.Count; i++)
            {
                uiCards[i].gameObject.SetActive(false);
            }

            var packIndex = CardPackHelper.GetPackIndex(uiCards.Count);
            packAnim.Play(packIndex, false, () =>
            {
                // play particle
                SonatUtils.DelayCall(configSO.delayAppearPs, () =>
                {
                    psAppear.Play();
                });

                // hiện các thẻ
                for (int i = 0; i < uiCards.Count; i++)
                {
                    uiCards[i].gameObject.SetActive(true);
                    isCompleteAppearCard = true;
                    int idx = i;
                    SonatUtils.DelayCall(configSO.delayBeforePlayCardParticle, () =>
                    {
                        uiCards[idx].PlayParticle();
                    });
                }
            }, configSO.animLifeTime, configSO.forceHideAnim);
        }
    }
}