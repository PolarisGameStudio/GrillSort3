using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    public class UICard : MonoBehaviour
    {
        [SerializeField, ReadOnly] private CardType cardType;
        // [SerializeField, ReadOnly] private int numCard;
        [SerializeField] private TMP_Text txtMainName;
        [SerializeField] private TMP_Text txtNameOnBack;
        [SerializeField] private TMP_Text txtNumCard;
        [SerializeField] private FixedImageRatio cardImage;
        [SerializeField] private GameObject tagNew;
        [SerializeField] private UIStarGroup starGroup;
        [SerializeField] private GameObject specialPs, goldFrame;

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
        // public int NumCard => numCard;
        public bool IsNew => _isNew;

        private bool _isNew = false;
        public void Setup(CardType cardType)
        {
            this.cardType = cardType;


            _cardConfig = _cardCollectionService.Instance.GetConfig().GetCardConfig(cardType);
            // var albumType = _cardCollectionService.Instance.config.GetAlbumType(cardType);
            // var _albumConfig = _cardCollectionService.Instance.config.GetAlbumConfig(albumType);
            cardImage.SetSpriteAsync(_cardConfig.GetCardSpritePath()).Forget();

            txtMainName.text = _cardConfig.cardName;
            // txtMainName.GetComponent<I2.Loc.Localize>().SetTerm(_cardConfig.cardName);
            txtMainName.SetMaterial(_cardConfig.GetTextColor());

            txtNameOnBack.text = _cardConfig.cardName;

            // txtNameOnBack.GetComponent<I2.Loc.Localize>().SetTerm(_cardConfig.cardName);

            // set star
            starGroup.Setup(_cardConfig.star);

            if (useParticle)
            {
                cardParticle.SetData(_cardConfig.star);
            }

            var specialFrame = _cardCollectionService.Instance.GetConfig().numStarShowSpecialFrame;
            var listGoldFrame = _cardCollectionService.Instance.GetConfig().listNumStarShowGoldFrame;
            specialPs.SetActive(_cardConfig.star == specialFrame);
            goldFrame.SetActive(listGoldFrame.Contains(_cardConfig.star));
        }

        public void SetData(int quantity, bool isNew)
        {
            _isNew = isNew;
            // numCard = quantity;

            tagNew.SetActive(isNew);
            txtNumCard.text = quantity.ToString();

            SetEnable(quantity > 0);
        }

        public void UpdateData()
        {
            var inventoryModule = _cardCollectionService.Instance.CardInventoryModule;
            var isNew = inventoryModule.IsNewCard(cardType);
            // var numCard = _cardCollectionService.Instance.CardSubmodule.GetNumCard(cardType);
            // txtNumCard.text = numCard.ToString();

            // check Active
            // isEnable = numCard > 0;
            isEnable = inventoryModule.CheckExistCollectedCard(cardType);
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
                    .Add(PopupCard.CARD_TYPE_KEY, cardType)
                    .Add(PopupCard.POSITION_KEY, transform.position)
                    .Add(PopupCard.ON_COMPLETE_CLOSE_KEY, (Action)(() =>
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
