using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection
{
    public class UICard : MonoBehaviour
    {
        [SerializeField, ReadOnly] private CardType cardType;
        [SerializeField, ReadOnly] private int numCard;
        [SerializeField] private TMP_Text txtMainName;
        [SerializeField] private TMP_Text txtNameOnBack;
        [SerializeField] private TMP_Text txtNumCard;
        [SerializeField] private FixedImageRatio cardImage;
        [SerializeField] private GameObject tagNew;
        [SerializeField] private UIStarGroup starGroup;
        [SerializeField] private GameObject framePs, goldFrame;

        [Space]
        [Header("Enable / Disable Objects")]
        private bool isEnable = true;
        [SerializeField] private GameObject enableObj;
        [SerializeField] private GameObject disableObj;

        [Space]
        [SerializeField] private bool useParticle = false;
        [SerializeField, ShowIf("useParticle")] private CardParticle cardParticle;
        [SerializeField, ShowIf("useParticle")] private ParticleSystem psDisappear;

        private readonly Service<CardCollectionService> _cardCollectionService = new();
        private CardConfigSO _cardConfig;

        public CardType CardType => cardType;
        public int NumCard => numCard;
        public bool IsNew => _isNew;

        private bool _isNew = false;
        public void Setup(CardType cardType)
        {
            this.cardType = cardType;

            cardImage.SetSpriteAsync(_cardConfig.GetCardSpritePath()).Forget();

            _cardConfig = _cardCollectionService.Instance.GetCardConfig(cardType);

            //txtMainName.text = _cardConfig.cardName;

            //cardImage.sprite = _cardConfig.sprite;

            //await UniTask.Yield();

            txtMainName.GetComponent<I2.Loc.Localize>().SetTerm(_cardConfig.cardName);

            txtMainName.SetMaterial(_cardConfig.textColorIndex);

            //txtNameOnBack.text = _cardConfig.cardName;

            txtNameOnBack.GetComponent<I2.Loc.Localize>().SetTerm(_cardConfig.cardName);

            // set star
            starGroup.Setup(_cardConfig.star);

            if (useParticle)
            {
                cardParticle.SetData(_cardConfig.star);
            }

            framePs.SetActive((int)cardType % 9 == 8);

            goldFrame.SetActive(((int)cardType % 9 == 7 || (int)cardType % 9 == 6));
        }

        public void SetData(int quantity, bool isNew)
        {
            _isNew = isNew;
            numCard = quantity;

            tagNew.SetActive(isNew);
            txtNumCard.text = quantity.ToString();

            SetEnable(quantity > 0);
        }

        public void UpdateData()
        {
            var isNew = _cardCollectionService.Instance.IsNewCardButNotSeen(cardType);
            var numCard = _cardCollectionService.Instance.GetNumCard(cardType);
            txtNumCard.text = numCard.ToString();

            // check Active
            isEnable = numCard > 0;
            SetEnable(isEnable);

            tagNew.SetActive(isEnable && isNew);
        }

        private void SetEnable(bool isEnable)
        {
            enableObj.SetActive(isEnable);
            disableObj.SetActive(!isEnable);

            starGroup.SetData(isEnable);
        }

        public void OnClick()
        {
            //Debug.Log($"OnClick Card {cardType}");
            if (isEnable)
            {
                SetEnable(false);
                PanelManager.Instance.OpenPanel<PopupCard>(new UIData()
                    .Add("cardType", cardType)
                    .Add("position", transform.position)
                    .Add("onCompleteClose", (Action)(() =>
                        {
                            UpdateData();
                        }))
                );
            }

        }

        public void PlayParticle()
        {
            if (useParticle)
            {
                cardParticle.PlayPSAppear();
            }
        }

        public void PlayParticleDisappear()
        {
            if (useParticle)
            {
                psDisappear.Play();
            }
        }
    }
}
