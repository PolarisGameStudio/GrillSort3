using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Modules.CardCollection.Home;
using MyGame.Modules.CardCollection.ReceiveCardEffect.Configs;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection
{

    public abstract class PopupReceiveCardBase : Panel
    {
        public const string CARD_REWARD_KEY = "RewardData";
        public const string RECENTLY_NEW_CARD_LIST_KEY = "RecentlyNewCardList";
        [SerializeField] private UIWidgetReceiveCardStar uiWidgetCardStar;
        [SerializeField] private Button btnClaim;

        [Header("Appear Cards")]
        [SerializeField] private Transform verContainer;

        [Header("Animation")]
        [SerializeField] private PopupReceiveCardBaseConfigSO baseConfigSO;

        private readonly Service<CardCollectionService> _cardCollectionService = new();
        private readonly Service<PoolingContainerService> _poolingContainerService = new();

        protected List<CardType> cardList;
        protected List<CardType> recentlyNewCardList;

        protected List<UICard> uiCards = new();
        protected List<Transform> listTargetRoots = new();
        protected List<Transform> listHorContainers = new();
        // protected List<UICardStar> listUICardStars = new();



        protected bool isCompleteAppearCard = false;
        protected bool isCollected = false;

        #region setup
        public override void Open(UIData uiData)
        {
            base.Open(uiData);
            cardList = uiData.Get<List<CardType>>(CARD_REWARD_KEY);
            recentlyNewCardList = uiData.Get<List<CardType>>(RECENTLY_NEW_CARD_LIST_KEY);
            SetupCards();

            PlayAppearAnimation().Forget();
            isCompleteAppearCard = false;
            isCollected = false;
        }

        private void SetupCards()
        {
            foreach (Transform horContainer in verContainer)
            {
                _poolingContainerService.Instance.CleanContainer(horContainer);
            }
            _poolingContainerService.Instance.CleanContainer(verContainer);

            uiCards.Clear();
            listTargetRoots.Clear();
            listHorContainers.Clear();
            // listUICardStars.Clear();

            var size = UIHelper.GetGridSize(cardList.Count);

            int count = 0;
            foreach (var cardType in cardList)
            {
                var row = count / size.col;
                var col = count % size.col;
                count++;

                // Tạo parent row nếu chưa có
                if (listHorContainers.Count <= row)
                {
                    var horContainer = _poolingContainerService.Instance.CreateObject<Transform>(verContainer);
                    listHorContainers.Add(horContainer);
                }

                // Tạo card container trong row
                var root = _poolingContainerService.Instance.CreateObject<Transform>(listHorContainers[row]);
                listTargetRoots.Add(root);

                // Lấy component UICard
                var card = root.GetComponentInChildren<UICard>();
                card.Setup(cardType);

                // Mỗi phần tử list là 1 thẻ duy nhất ⇒ numCard luôn = 1
                if (recentlyNewCardList.Contains(cardType))
                {
                    card.SetData(1, true);
                    recentlyNewCardList.Remove(cardType);
                }
                else
                {
                    card.SetData(1, false); // có nhiều thẻ mới cùng loại thì chỉ gán tag 1 lần
                }
                uiCards.Add(card);
            }

        }
        #endregion

        protected abstract UniTask PlayAppearAnimation();

        #region Claim
        public void OnClickClaim()
        {
            if (isCollected || !isCompleteAppearCard) return;
            isCollected = true;
            PlayCollect().Forget();

            btnClaim.gameObject.SetActive(false);
        }

        private async UniTask PlayCollect()
        {
            // MySonatFramework.audioService.PlaySound(AudioId.Card_Disappear_Grill_sort);
            // biến card dư thành star
            PlayExchangeCardToStar().Forget();

            // widget xuất hiện và star bay vào
            if (CheckOldCard())
            {
                await UniTask.Delay((int)(baseConfigSO.delayBeforeWidgetMoveIn * 1000));
                uiWidgetCardStar.PlayAppearAnimation();

                await UniTask.Delay((int)(baseConfigSO.delayBeforeHideNewCard * 1000));
                // new card (card còn lại) biến mất
                DisplayCardDisappear();
            }
            else
            {
                DisplayCardDisappear();
            }
        }

        private void DisplayCardDisappear()
        {
            foreach (var card in uiCards)
            {
                if (card.IsNew)
                {
                    card.transform.DOScale(0, baseConfigSO.scaleDownCardDuration).From(1).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        card.PlayParticleDisappear();
                    });
                }
            }
        }

        private bool CheckOldCard()
        {
            return uiCards.Any(card => card.IsNew == false);
        }

        private async UniTask PlayExchangeCardToStar()
        {
            var delayMove = baseConfigSO.delayBeforeFirstFlyStar;
            for (int i = 0; i < uiCards.Count; i++)
            {
                var card = uiCards[i];
                if (card.IsNew == false)
                {
                    var addedStar = CardPackHelper.GetNumberStarOfCard(card.CardType);

                    card.transform.DOScale(0, baseConfigSO.scaleDownCardDuration).From(1).SetEase(Ease.InBack);

                    var collectEffect = await SonatSystem.GetService<PoolingServiceAsync>().CreateAsync<UICollectMultiple>(
                        "CollectResourceMultipleStar_Card",
                        PanelManager.Instance.transform,
                        CardPackHelper.GetNumberStarOfCard(card.CardType),
                        card.transform.position,
                        uiWidgetCardStar.transform,
                        (Action)(() =>
                        {
                            uiWidgetCardStar.UpdateValueView(addedStar);
                        }),
                        delayMove);
                    delayMove += baseConfigSO.delayBetweenFlyStar;
                }
            }

            await UniTask.Delay((int)((delayMove + baseConfigSO.delayBeforeWidgetMoveOut) * 1000));
            uiWidgetCardStar.PlayDisappearAnimation(() =>
            {
                Close();
            });
        }
        #endregion
    }
}