using Cysharp.Threading.Tasks;
using Sonat.Enums;
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
            MySonatFramework.audioService.StopMusic();
            SonatUtils.DelayCall(configSO.delaySoundAppearAnim, () =>
            {
                MySonatFramework.audioService.PlaySound(AudioId.Card_collection_Appear_open_Grill_sort);
            });
            packAnim.Play(packIndex, false, () =>
            {
                // play particle
                SonatUtils.DelayCall(configSO.delayAppearPs, () =>
                {
                    psAppear.Play();
                    MySonatFramework.audioService.PlaySound(AudioId.Card_Appear_Grill_sort);
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

        public override void Close()
        {
            base.Close();
            MySonatFramework.audioService.PlayMusic(AudioId.BGM_Home_Default_Grill3);
        }
    }
}