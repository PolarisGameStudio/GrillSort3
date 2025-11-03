using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyGame.Modules.CardCollection.Animation;
using Newtonsoft.Json;
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
        public CardSubmodule CardSubmodule;
        public AlbumSubmodule AlbumSubmodule;
        public StarSubmodule StarSubmodule;

        private Queue<RewardData> _currentRewardQueue = new();
        private bool _isRunQueueRewardCard = false;
        public bool IsRunQueueRewardCard => _isRunQueueRewardCard;

        private Queue<AlbumType> _completedAlbumTypesQueue = new();
        private bool _isRunQueueCompleteAlbum = false;
        public bool IsRunQueueCompleteAlbum => _isRunQueueCompleteAlbum;

        // private IntDataPref _cardStarExchangeIndex;
        // private IntDataPref _isCompleteCardCollection;
        // private IntDataPref _numCompleteCardCollection;
        // private IntDataPref _cardStar;

        public int CardStar => StarSubmodule.NumberStar;
        public int TotalCard => CardSubmodule.TotalCard;
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

            CardSubmodule.LoadData();
            AlbumSubmodule.LoadData();
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

        private void OnAddItemEvent(AddItemEvent eventData)
        {
            // BUG: Cần thêm hàng đợi khi mở các gói liên tiếp
            if (GameResourceHelper.ResourceType(eventData.resource) == GameResourceType.Card)
            {
                for (int i = 0; i < eventData.quantity; i++) // mở từng gói card packs
                {
                    var rewardData = new RewardData();
                    rewardData.resourceDatas = new()
                    {
                        new ResourceData(eventData.resource, 1)
                    };

                    _currentRewardQueue.Enqueue(rewardData);
                    Debug.Log("anhnt: Enqueue reward card pack " + JsonConvert.SerializeObject(_currentRewardQueue));
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
            while (_currentRewardQueue.Count > 0)
            {
                var rewardData = _currentRewardQueue.Dequeue();

                Debug.Log("anhnt: Dequeue reward card pack " + JsonConvert.SerializeObject(_currentRewardQueue));

                var popup = PanelManager.Instance.OpenPanelByName<PopupReceiveCardBase>("PopupReceiveCard_Immediately",
                    new UIData().Add(PopupReceiveCardBase.REWARD_DATA_KEY, rewardData));

                await UniTask.WaitUntil(() => (popup == null || popup.gameObject.activeInHierarchy == false));
            }

            _isRunQueueRewardCard = false;
        }

        public async UniTask<bool> RunQueueCompleteAlbum()
        {
            // var open = false;
            // await UniTask.Delay(1000);
            // while (_completedAlbumTypesQueue.Count > 0)
            // {
            //     await UniTask.Delay(300);
            //     var popupReceiveCard = PanelManager.Instance.GetPanel<PopupReceiveCard_Immediately>();
            //     var popupReward = PanelManager.Instance.GetPanel<PopupReward>();
            //     await UniTask.WaitUntil(() => (popupReceiveCard == null || popupReceiveCard.gameObject.activeInHierarchy == false)
            //                                   && (popupReward == null || popupReward.gameObject.activeInHierarchy == false));


            //     var albumType = _completedAlbumTypesQueue.Dequeue();

            //     var popup = PanelManager.Instance.OpenPanel<PopupCompleteAlbum>(new UIData().Add("AlbumType", albumType));
            //     open = true;
            //     await UniTask.WaitUntil(() => (popup == null || popup.gameObject.activeInHierarchy == false));
            // }

            // if (_isCompleteCardCollection.Value == 1)
            // {
            //     var popupReward = PanelManager.Instance.GetPanel<PopupReward>();
            //     await UniTask.WaitUntil(() => ((popupReward == null || popupReward.gameObject.activeInHierarchy == false)));

            //     _isCompleteCardCollection.Value = 0;
            //     var reward = config.rewardInSeason;
            //     MySonatFramework.inventoryService.AddReward(reward, new EarnResourceLogData
            //     {
            //         spendType = "card_collection",
            //         spendId = "card_collection",
            //         isFirstBuy = false,
            //         source = "non_iap"
            //     });
            //     UIData uiData = new UIData();
            //     uiData.Add("Title", "REWARD!");
            //     uiData.Add("Reward", reward);
            //     uiData.Add("x2", false);
            //     PanelManager.Instance.OpenPanelByName<PopupReward>("PopupRewardCardCollection", uiData);
            // }

            // return open;
            return false;
        }

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

            TryPlayCompleteAlbum().Forget();
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
            uiData.Add(PopupReward.KEY_REWARD, reward);
            PanelManager.Instance.OpenPanel<PopupReward>(uiData);
        }

        private async UniTask TryPlayCompleteAlbum()
        {
            // Hiện popup complete album
            var openCompleteAlbum = await RunQueueCompleteAlbum();
        }

        public int GetNumberCollectedCardInAlbum(AlbumType albumType)
        {
            var albumConfig = config.GetAlbumConfig(albumType);
            return albumConfig.cards.Count(cardType => CardSubmodule.CollectedCardTypes.Contains(cardType));
        }

        public int GetNumberNewCardInAlbum(AlbumType albumType = AlbumType.None)
        {
            if (albumType == AlbumType.None)
            {
                return CardSubmodule.NewCardTypes.Count;
            }
            else
            {
                return CardSubmodule.NewCardTypes.Count(cardType => config.GetAlbumType(cardType) == albumType);
            }
        }
    }
}