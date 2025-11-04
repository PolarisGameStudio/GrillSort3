using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
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
        private Queue<List<CardType>> _queueListTempCards = new();
        private Queue<AlbumType> _queueListCompletedAlbum = new();

        public event Action OnNewCardCountChanged;

        #region Init
        public override void Initialize()
        {
            base.Initialize();

            new EventBinding<AddItemEvent>(OnAddItemEvent);
            new EventBinding<HomeProcessEvent>(OnHomeProcessEvent);
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
            StarSubmodule.LoadData();
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
            var expireTime = date.AddDays(7);
            return ((DateTimeOffset)expireTime).ToUnixTimeSeconds();
        }
        #endregion

        #region Receive Card & complete album
        private void OnAddItemEvent(AddItemEvent eventData)
        {
            // datlt: Cần thêm hàng đợi khi mở các gói liên tiếp
            if (GameResourceHelper.ResourceType(eventData.resource) == GameResourceType.Card)
            {
                for (int i = 0; i < eventData.quantity; i++) // mở từng gói card packs
                {
                    var rewardData = new RewardData();
                    rewardData.resourceDatas = new()
                    {
                        new ResourceData(eventData.resource, 1)
                    };

                    // _currentRewardQueue.Enqueue(rewardData);

                    if (_isRunQueueRewardCard == false)
                    {
                        _isRunQueueRewardCard = true;
                        RunQueueRewardCard().Forget();
                    }
                }
            }
        }

        private async UniTask RunQueueRewardCard()
        {
            await UniTask.Delay(1000);
            while (_queueListTempCards.Count > 0)
            {
                var cardList = _queueListTempCards.Dequeue();

                // Debug.Log("anhnt: Dequeue reward card pack " + JsonConvert.SerializeObject(_currentRewardQueue));

                var popup = PanelManager.Instance.OpenPanelByName<PopupReceiveCardBase>("PopupReceiveCard_Immediately",
                    new UIData().Add(PopupReceiveCardBase.CARD_REWARD_KEY, cardList));

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

            RunCompleteCardCollection().Forget();
        }

        public async UniTask RunCompleteCardCollection()
        {
            if (CardInventoryModule.IsCompleteCardCollection)
            {
                var uiData = new UIData();
                uiData.Add(PopupReward.REWARD_KEY, config.rewardInSeason);
                PanelManager.Instance.OpenPanel<PopupReward>(uiData);
            }
        }
        #endregion

        #region Event unlock
        private void OnHomeProcessEvent(HomeProcessEvent eventData)
        {
            if (IsUnlocked() == false)
            {
                if (CanUnlock() == true)
                {
                    Unlock();
                    ShowTutorial().Forget();
                }
            }
        }

        private async UniTask ShowTutorial()
        {
            // Hiện tut
            if (PlayerPrefs.HasKey($"{DATA_KEY}_ShowTutorial") == false)
            {
                PlayerPrefs.SetInt($"{DATA_KEY}_ShowTutorial", 1);

                UIData uiData = new();
                uiData.Add(UIDataKey.CallBackOnClose, (Action)(() =>
                {
                    _ = HomeManager.Instance.SwitchTab(NavigationType.CardCollection, 0.25f);
                }));

                var popup = PanelManager.Instance.OpenPanelByName<Panel>("PopupTutorial_CardCollection", uiData);

                await UniTask.WaitUntil(() => popup == null || popup.gameObject.activeInHierarchy == false);
                RewardUnlockFeature();
            }
        }

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
        #endregion

        public void UnboxPackCard(ResourceData resourceData, bool noti = false)
        {
            var cardList = CardPackHelper.GetCardReward(resourceData);

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

                    var albumType = config.GetAlbumType(cardType);
                    if (CardInventoryModule.CheckCompleteAlbum(albumType))
                    {
                        _queueListCompletedAlbum.Enqueue(albumType);

                        ReceiveRewardAlbum(albumType);
                    }
                }
            }

            _queueListTempCards.Enqueue(cardList);

            if (CardInventoryModule.CheckAllAlbumComplete())
            {
                CardInventoryModule.SetCompleteCardCollection();
                ReceiveRewardCardCollection();
            }
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
    }
}