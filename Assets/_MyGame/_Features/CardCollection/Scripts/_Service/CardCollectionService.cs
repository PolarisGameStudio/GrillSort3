using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Helper;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Systems.TimeManagement;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardCollectionService", menuName = "MyGame/SkewerJam/Features/CardCollection/CardCollectionService")]

    public class CardCollectionService : BaseExpireService
    {
        public override string DATA_KEY => "CARD_COLLECTION";

        [Header("CONFIGS")]
        public CardCollectionConfigSO config;

        [Space(10)]
        [Header("SubModules")]
        public CardInventoryModule CardInventoryModule;
        public CardSubmodule CardSubmodule;
        public AlbumSubmodule AlbumSubmodule;
        public StarSubmodule StarSubmodule;


        private bool _isRunQueueRewardCard = false;
        private Queue<TempCardRewardData> _queueListTempCards = new();
        private Queue<AlbumType> _queueListCompletedAlbum = new();

        public event Action OnNewCardCountChanged;

        #region Init
        public override void Initialize()
        {
            base.Initialize();

            new EventBinding<AddItemEvent>(OnAddItemEvent);

            SonatUtils.ExecuteNextFrame(() =>
            {
                LoadImageAsync().Forget();
            });


        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            config.InitializeAsync();
        }

        protected override void LoadData()
        {
            base.LoadData();

            CardInventoryModule.LoadData();
            CardSubmodule.LoadData();
            StarSubmodule.LoadData();
        }

        protected override void ResetData()
        {
            base.ResetData();
            CardInventoryModule.ResetData();
            CardSubmodule.ResetData();
            StarSubmodule.ResetData();

        }

        private async UniTask LoadImageAsync()
        {
            foreach (var album in config.albums)
            {
                var albumSprite = await AddressableManager.LoadSpriteAsync(album.GetAlbumSpritePath());
                var albumBackgroundSprite = await AddressableManager.LoadSpriteAsync(album.GetAlbumBackgroundSpritePath());
                var albumBorderSprite = await AddressableManager.LoadSpriteAsync(album.GetAlbumBorderSpritePath());
            }

            await UniTask.DelayFrame(1);

            // foreach (var card in config.cards)
            // {
            //     var cardSprite = await AddressableManager.LoadSpriteAsync(card.GetCardSpritePath());
            // }
        }

        public override bool CanUnlock()
        {
            var level = MySonatFramework.userDataService.GetLevel();
            return level >= config.unlockLevel;
        }

        protected override long GetNextExpireTime()
        {
            // đến ngày cuối cùng của tháng thứ 3
            var date = MySonatFramework.GetService<TimeService>().GetCurrentTime();
            // Lấy ngày cuối cùng của tháng thứ 3 (tháng hiện tại + 2)
            int year = date.Year;
            int month = date.Month + 2;
            if (month > 12)
            {
                month -= 12;
                year += 1;
            }
            int lastDay = DateTime.DaysInMonth(year, month);
            var expireTime = new DateTime(year, month, lastDay, 23, 59, 59, DateTimeKind.Utc);

            return ((DateTimeOffset)expireTime).ToUnixTimeSeconds() + 1;
        }

        protected override void ProgressUnlockFeature()
        {
            RunCompleteCardCollection();
        }

        protected override async UniTask TryShowTutorial()
        {
            // Hiện tut
            if (PlayerPrefs.HasKey($"{DATA_KEY}_ShowTutorial") == false)
            {
                HomeManager.Instance.BlockUIManager.RegisterBlockUI(nameof(CardCollectionService));
                PlayerPrefs.SetInt($"{DATA_KEY}_ShowTutorial", 1);

                UIData uiData = new();
                uiData.Add(UIDataKey.CallBackOnClose, (Action)(() =>
                {
                    _ = HomeManager.Instance.SwitchTab(NavigationType.CardCollection);
                }));

                await UniTask.Delay(1000);
                var popup = PanelManager.Instance.OpenPanelByName<Panel>("PopupTutorial_CardCollection", uiData);

                await UniTask.WaitUntil(() => popup == null || popup.gameObject.activeInHierarchy == false);
                await UniTask.Delay(1000);
                RewardUnlockFeature();
                HomeManager.Instance.BlockUIManager.DeregisterBlockUI(nameof(CardCollectionService));
            }
        }
        #endregion

        #region Receive Card & complete album
        private void OnAddItemEvent(AddItemEvent eventData)
        {
            // datlt: Cần thêm hàng đợi khi mở các gói liên tiếp
            if (GameResourceHelper.ResourceType(eventData.resource) == GameResourceType.Card)
            {
                if (_isRunQueueRewardCard == false)
                {
                    _isRunQueueRewardCard = true;
                    RunQueueRewardCard().Forget();
                }
            }
        }

        private async UniTask RunQueueRewardCard()
        {
            await UniTask.Delay(1000);
            while (_queueListTempCards.Count > 0)
            {
                var tempCardRewardData = _queueListTempCards.Dequeue();

                var uiData = new UIData();
                uiData.Add(PopupReceiveCardBase.CARD_REWARD_KEY, tempCardRewardData.cardList);
                uiData.Add(PopupReceiveCardBase.RECENTLY_NEW_CARD_LIST_KEY, tempCardRewardData.recentlyNewCardList);
                uiData.Add(PopupReceiveCardBase.PACK_RESOURCE_KEY, tempCardRewardData.packResource);
                var popup = PanelManager.Instance.OpenPanelByName<PopupReceiveCardBase>("PopupReceiveCard_Immediately", uiData);

                await UniTask.WaitUntil(() => (popup == null || popup.gameObject.activeInHierarchy == false));
            }

            _isRunQueueRewardCard = false;

            RunQueueCompleteAlbum().Forget();
        }

        public async UniTask RunQueueCompleteAlbum()
        {
            while (_queueListCompletedAlbum.Count > 0)
            {
                await UniTask.Delay(300);
                var popupReceiveCard = PanelManager.Instance.GetPanel<PopupReceiveCardBase>();
                var popupReward = PanelManager.Instance.GetPanel<PopupReward>();
                await UniTask.WaitUntil(() => (popupReceiveCard == null || popupReceiveCard.gameObject.activeInHierarchy == false)
                                              && (popupReward == null || popupReward.gameObject.activeInHierarchy == false));


                var albumType = _queueListCompletedAlbum.Dequeue();

                var popup = PanelManager.Instance.OpenPanel<PopupCompleteAlbum>(new UIData().Add(PopupCompleteAlbum.ALBUM_TYPE_KEY, albumType));

                await UniTask.WaitUntil(() => (popup == null || popup.gameObject.activeInHierarchy == false));
            }

            await UniTask.Delay(300);
            var popupRewardAlbum = PanelManager.Instance.GetPanel<PopupReward>();
            await UniTask.WaitUntil(() => (popupRewardAlbum == null || popupRewardAlbum.gameObject.activeInHierarchy == false));
            RunCompleteCardCollection().Forget();
        }

        public async UniTask RunCompleteCardCollection()
        {
            if (CardInventoryModule.CompletedCardCollection == false && CardInventoryModule.CheckAllAlbumComplete())
            {
                var sceneService = MySonatFramework.GetService<SceneService>();
                if (sceneService.GetCurrentGamePlacement() == GamePlacement.Home)
                {
                    await HomeManager.Instance.SwitchTab(NavigationType.CardCollection);
                }

                await UniTask.Delay(500);
                CardInventoryModule.SetCompleteCardCollection(true);
                ReceiveRewardCardCollection();

                var uiData = new UIData();
                uiData.Add(PopupRewardChest.REWARD_KEY, config.rewardInSeason);
                uiData.Add(PopupRewardChest.SKIN_KEY, 3);
                PanelManager.Instance.OpenPanelByName<PopupRewardChest>("PopupRewardChest_CardCollection", uiData);
            }
        }
        #endregion

        private void RewardUnlockFeature()
        {
            RewardData reward = config.RewardUnlock;
            MySonatFramework.inventoryService.AddReward(reward, new EarnResourceLogData
            {
                spendType = "card_collection",
                spendId = "card_collection",
                isFirstBuy = false,
                source = "non_iap"
            });
            UIData uiData = new UIData();
            uiData.Add(PopupReward.REWARD_KEY, reward);
            PanelManager.Instance.OpenPanel<PopupReward>(uiData);
        }


        #region Unbox Pack Card
        public void UnboxPackCard(ResourceData resourceData, bool noti = false)
        {
            for (int i = 0; i < resourceData.quantity; i++)
            {
                var cardList = CardSubmodule.GetCardReward(resourceData.resource);
                ReceiveCards(cardList, resourceData.resource);
            }
        }

        public void ReceiveCards(List<CardType> cardList, GameResource packResource)
        {
            var recentlyNewCardList = new List<CardType>();
            StarSubmodule.SetNumberStarView();
            foreach (var cardType in cardList)
            {
                if (CardInventoryModule.CheckExistCollectedCard(cardType) == true)
                {
                    var numberStar = CardPackHelper.GetNumberStarOfCard(cardType);
                    StarSubmodule.AddCardStar(numberStar);
                }
                else
                {
                    CardInventoryModule.CollectCard(cardType);
                    recentlyNewCardList.Add(cardType);

                    var albumType = config.GetAlbumType(cardType);
                    if (CardInventoryModule.CheckCompleteAlbum(albumType))
                    {
                        _queueListCompletedAlbum.Enqueue(albumType);

                        ReceiveRewardAlbum(albumType);
                    }
                }
            }

            _queueListTempCards.Enqueue(new TempCardRewardData()
            {
                cardList = cardList,
                recentlyNewCardList = recentlyNewCardList,
                packResource = packResource
            });
        }

        private void ReceiveRewardAlbum(AlbumType albumType)
        {
            var reward = config.GetAlbumConfig(albumType).reward;
            MySonatFramework.inventoryService.AddReward(reward, new EarnResourceLogData
            {
                spendType = "card_collection_complete_album",
                spendId = albumType.ToString(),
                isFirstBuy = false,
                source = "non_iap"
            });
        }

        private void ReceiveRewardCardCollection()
        {
            var reward = config.rewardInSeason;
            MySonatFramework.inventoryService.AddReward(reward, new EarnResourceLogData
            {
                spendType = "card_collection_complete_collection",
                spendId = "card_collection",
                isFirstBuy = false,
                source = "non_iap"
            });
        }
        #endregion
    }

    public class TempCardRewardData
    {
        public GameResource packResource;
        public List<CardType> cardList;
        public List<CardType> recentlyNewCardList;
    }
}