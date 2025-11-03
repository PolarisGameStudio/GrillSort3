// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Cysharp.Threading.Tasks;
// using MyGame.Modules.CardCollection.Animation;
// using Newtonsoft.Json;
// using Sonat.Enums;
// using SonatFramework.Scripts.Helper;
// using SonatFramework.Scripts.UIModule;
// using SonatFramework.Systems;
// using SonatFramework.Systems.EventBus;
// using SonatFramework.Systems.InventoryManagement;
// using SonatFramework.Systems.InventoryManagement.GameResources;
// using SonatFramework.Systems.TimeManagement;
// using UnityEngine;

// namespace MyGame.Modules.CardCollection
// {
//     [CreateAssetMenu(fileName = "CardCollectionService", menuName = "Sonat Services/CardCollection/CardCollectionService")]
//     public class CardCollectionService2 : SonatServiceSo, IServiceInitialize
//     {
//         [Header("CONFIGS")]
//         public CardCollectionConfigSO config;
//         public StarExchangeConfig starExchangeConfig;

//         public string DATA_KEY => "CARD_COLLECTION";


//         // [Header("Custom Data Service")]
//         // public CardInventoryService cardInventory;

//         public event Action OnChangeData;
//         public event Action OnNewCardCountChanged;
//         public event Action OnCompleteAlbum;
//         public event Action OnCompleteCollection;

//         public int TotalCard => _collectedCardTypes.Count;
//         public int CardStar => _cardStar.Value;

//         // quản lý các thẻ mới
//         private HashSet<CardType> _collectedCardTypes = new();
//         private HashSet<CardType> _newCardTypes = new(); // card mới nhưng tạm thời chưa được xem trong album
//         private ListDataPref<int> _collectedCardsData;
//         private ListDataPref<int> _newCardData;
//         private ListDataPref<int> _completedAlbumTypes;
//         private IntDataPref _cardStar;
//         // private IntDataPref _isUnlocked;
//         private IntDataPref _cardStarExchangeIndex;
//         private IntDataPref _isCompleteCardCollection;
//         private LongDataPref _expireTime;
//         private IntDataPref _numCompleteCardCollection;

//         private Queue<RewardData> _currentRewardQueue = new();
//         private Queue<AlbumType> _completedAlbumTypesQueue = new();

//         private bool _isRunQueueRewardCard = false;
//         public bool IsRunQueueRewardCard => _isRunQueueRewardCard;

//         private readonly Service<TimeService> _timeService = new();

//         public void Initialize()
//         {
//             LoadConfig();
//             LoadData();

//             new EventBinding<AddItemEvent>(OnAddItemEvent);

//             if (IsUnlocked())
//             {
//                 CheckExpire().Forget();
//             }
//         }

//         #region Data/ Config

//         private void LoadConfig()
//         {
//             config.InitializeAsync().Forget();
//         }

//         private void LoadData()
//         {
//             // _isUnlocked = new IntDataPref($"{DATA_KEY}_isUnlocked", 1);

//             // collected cards
//             _collectedCardsData = new ListDataPref<int>($"{DATA_KEY}_collectedCards");
//             if (_collectedCardsData.Value.Count > 0)
//             {
//                 _collectedCardTypes = new HashSet<CardType>(_collectedCardsData.Value.Select(e => (CardType)e));
//             }

//             // new cards
//             _newCardData = new ListDataPref<int>($"{DATA_KEY}_newCards");
//             if (_newCardData.Value.Count > 0)
//             {
//                 _newCardTypes = new HashSet<CardType>(_newCardData.Value.Select(e => (CardType)e));
//             }

//             // card star
//             _cardStar = new IntDataPref($"{DATA_KEY}_cardStar");

//             // completed album types
//             _completedAlbumTypes = new ListDataPref<int>($"{DATA_KEY}_completedAlbumTypes");

//             // card star exchange index
//             _cardStarExchangeIndex = new IntDataPref($"{DATA_KEY}_cardStarExchangeIndex", -1);

//             // is complete card collection
//             _isCompleteCardCollection = new IntDataPref($"{DATA_KEY}_isCompleteCardCollection", 0);

//             _numCompleteCardCollection = new IntDataPref($"{DATA_KEY}_numCompleteCardCollection", _isCompleteCardCollection.BoolValue ? 1 : 0);

//             // expire time
//             _expireTime = new LongDataPref($"{DATA_KEY}_expireTime");
//         }

//         public void SaveData()
//         {
//             _collectedCardsData.Value = _collectedCardTypes.ToList().Select(e => (int)e).ToList();
//             _newCardData.Value = _newCardTypes.ToList().Select(e => (int)e).ToList();
//             OnChangeData?.Invoke();
//         }

//         private void ResetData()
//         {
//             _collectedCardTypes = new();
//             _newCardTypes = new(); // card mới nhưng tạm thời chưa được xem trong album
//             _collectedCardsData.Clear();
//             _newCardData.Clear();

//             _cardStar.Value = 0;
//             _cardStarExchangeIndex.Value = -1;
//             _isCompleteCardCollection.Value = 0;

//             _currentRewardQueue = new();
//             _completedAlbumTypesQueue = new();
//             _completedAlbumTypes.Clear();

//             _isRunQueueRewardCard = false;

//             SetExpireTime();
//             SaveData();
//         }

//         public bool IsUnlocked()
//         {
//             return true;
//             // return _isUnlocked.Value == 1;
//         }

//         public bool CanUnlock()
//         {
//             var level = MySonatFramework.userDataService.GetLevel();
//             return level >= config.unlockLevel;
//         }

//         public void Unlock()
//         {
//             // _isUnlocked.Value = 1;
//             ResetData();
//             CheckExpire().Forget();
//         }

//         #endregion

//         private async UniTask CheckExpire()
//         {
//             if (_expireTime.Value > 0)
//             {
//                 var remainTime = GetRemainTime();
//                 if (remainTime > 0)
//                 {
//                     await UniTask.Delay(TimeSpan.FromSeconds(remainTime));
//                 }
//             }

//             ResetData();
//             CheckExpire().Forget();
//         }

//         private void SetExpireTime()
//         {
//             // đến ngày cuối cùng của tháng thứ 3
//             var date = _timeService.Instance.GetCurrentTime();
//             var newDate = date.AddMonths(config.durationMonth - 1);

//             var lastDayOfMonth = new DateTime(newDate.Year, newDate.Month, DateTime.DaysInMonth(newDate.Year, newDate.Month), 23, 59, 59);
//             _expireTime.Value = ((DateTimeOffset)lastDayOfMonth).ToUnixTimeSeconds() + 1;
//         }

//         public long GetRemainTime()
//         {
//             var currentTime = _timeService.Instance.GetUnixTimeSeconds();
//             return _expireTime.Value - currentTime;
//         }


//         private void OnAddItemEvent(AddItemEvent eventData)
//         {
//             // BUG: Cần thêm hàng đợi khi mở các gói liên tiếp
//             if (GameResourceHelper.ResourceType(eventData.resource) == GameResourceType.Card)
//             {
//                 for (int i = 0; i < eventData.quantity; i++) // mở từng gói card packs
//                 {
//                     var rewardData = new RewardData();
//                     rewardData.resourceDatas = new()
//                     {
//                         new ResourceData(eventData.resource, 1)
//                     };

//                     _currentRewardQueue.Enqueue(rewardData);
//                     Debug.Log("anhnt: Enqueue reward card pack " + JsonConvert.SerializeObject(_currentRewardQueue));
//                     if (_isRunQueueRewardCard == false)
//                     {
//                         _isRunQueueRewardCard = true;
//                         RunQueueRewardCard().Forget();
//                     }
//                 }
//             }
//         }

//         private async UniTask RunQueueRewardCard()
//         {
//             await UniTask.Delay(1000);
//             while (_currentRewardQueue.Count > 0)
//             {
//                 var rewardData = _currentRewardQueue.Dequeue();

//                 Debug.Log("anhnt: Dequeue reward card pack " + JsonConvert.SerializeObject(_currentRewardQueue));

//                 var popup = PanelManager.Instance.OpenPanelByName<PopupReceiveCardBase>("PopupReceiveCard_Immediately",
//                     new UIData().Add(PopupReceiveCardBase.REWARD_DATA_KEY, rewardData));

//                 await UniTask.WaitUntil(() => (popup == null || popup.gameObject.activeInHierarchy == false));
//             }

//             _isRunQueueRewardCard = false;
//         }

//         // hard code
//         public async UniTask<bool> RunQueueCompleteAlbum()
//         {
//             var open = false;
//             await UniTask.Delay(1000);
//             while (_completedAlbumTypesQueue.Count > 0)
//             {
//                 await UniTask.Delay(300);
//                 var popupReceiveCard = PanelManager.Instance.GetPanel<PopupReceiveCard_Immediately>();
//                 var popupReward = PanelManager.Instance.GetPanel<PopupReward>();
//                 await UniTask.WaitUntil(() => (popupReceiveCard == null || popupReceiveCard.gameObject.activeInHierarchy == false)
//                                               && (popupReward == null || popupReward.gameObject.activeInHierarchy == false));


//                 var albumType = _completedAlbumTypesQueue.Dequeue();

//                 var popup = PanelManager.Instance.OpenPanel<PopupCompleteAlbum>(new UIData().Add("AlbumType", albumType));
//                 open = true;
//                 await UniTask.WaitUntil(() => (popup == null || popup.gameObject.activeInHierarchy == false));
//             }

//             if (_isCompleteCardCollection.Value == 1)
//             {
//                 var popupReward = PanelManager.Instance.GetPanel<PopupReward>();
//                 await UniTask.WaitUntil(() => ((popupReward == null || popupReward.gameObject.activeInHierarchy == false)));

//                 _isCompleteCardCollection.Value = 0;
//                 var reward = config.rewardInSeason;
//                 MySonatFramework.inventoryService.AddReward(reward, new EarnResourceLogData
//                 {
//                     spendType = "card_collection",
//                     spendId = "card_collection",
//                     isFirstBuy = false,
//                     source = "non_iap"
//                 });
//                 UIData uiData = new UIData();
//                 uiData.Add("Title", "REWARD!");
//                 uiData.Add("Reward", reward);
//                 uiData.Add("x2", false);
//                 PanelManager.Instance.OpenPanelByName<PopupReward>("PopupRewardCardCollection", uiData);
//             }

//             return open;
//         }

//         #region Get Data
//         public int GetNumCardInAlbum(AlbumType albumType)
//         {
//             int numCard = 0;
//             var albumConfig = config.GetAlbumConfig(albumType);
//             foreach (var cardType in albumConfig.cards)
//             {
//                 if (GetNumCard(cardType) > 0)
//                 {
//                     numCard++;
//                 }
//             }

//             return numCard;
//         }

//         public int GetNumCard(CardType cardType)
//         {
//             if (_collectedCardTypes.Contains(cardType))
//             {
//                 return 1;
//             }

//             return 0;
//             // return cardInventory.GetCard(cardType);
//         }

//         #endregion

//         #region Xử lý card mới

//         public int GetNewCardCount(AlbumType albumType = AlbumType.None)
//         {
//             if (albumType == AlbumType.None)
//             {
//                 return _newCardTypes.Count;
//             }
//             else
//             {
//                 int numCard = 0;
//                 foreach (var cardType in config.GetAlbumConfig(albumType).cards)
//                 {
//                     if (IsNewCardButNotSeen(cardType))
//                     {
//                         numCard++;
//                     }
//                 }

//                 return numCard;
//             }
//         }

//         public void RemoveNewCard(CardType cardType)
//         {
//             _newCardTypes.Remove(cardType);

//             SaveData();
//             OnNewCardCountChanged?.Invoke();
//         }

//         public bool IsNewCard(CardType cardType)
//         {
//             return _collectedCardTypes.Contains(cardType) == false;
//         }

//         public bool IsNewCardButNotSeen(CardType cardType)
//         {
//             return _newCardTypes.Contains(cardType);
//         }

//         #endregion


//         public void AddCard(CardType cardType, int numCard)
//         {
//             // cardInventory.AddCard(cardType, numCard);

//             var count = _collectedCardTypes.Count;
//             _collectedCardTypes.Add(cardType);

//             // check complete album
//             var albumType = config.GetAlbumType(cardType);
//             if (_completedAlbumTypes.Contains((int)albumType) == false && CheckCompleteAlbum(albumType))
//             {
//                 Debug.Log("anhnt: complete album");
//                 CompleteAlbum(albumType);
//             }

//             // check new card
//             if (count != _collectedCardTypes.Count)
//             {
//                 _newCardTypes.Add(cardType);
//                 SaveData();
//                 OnNewCardCountChanged?.Invoke();
//             }
//         }

//         private void CompleteAlbum(AlbumType albumType)
//         {
//             if (_completedAlbumTypes.Contains((int)albumType) == false)
//             {
//                 _completedAlbumTypes.Add((int)albumType);
//             }

//             if (_completedAlbumTypesQueue.Contains(albumType) == false)
//             {
//                 _completedAlbumTypesQueue.Enqueue(albumType);
//             }


//             if (CheckCompleteCardCollection())
//             {
//                 _isCompleteCardCollection.Value = 1;
//                 _numCompleteCardCollection.Value++;
//                 OnCompleteCollection?.Invoke();
//             }

//             OnCompleteAlbum?.Invoke();
//             SaveData();
//         }


//         private bool CheckCompleteCardCollection()
//         {
//             foreach (var albumType in config.albums)
//             {
//                 if (_completedAlbumTypes.Contains((int)albumType.type) == false)
//                 {
//                     return false;
//                 }
//             }

//             return true;
//         }

//         public int GetCountCompleteAlbum()
//         {
//             return _completedAlbumTypes.Value.Count;
//         }

//         public int GetCountCompleteCollection()
//         {
//             return _numCompleteCardCollection.Value;
//         }

//         public void AddCardStar(int numStar)
//         {
//             _cardStar.Value += numStar;
//             SaveData();
//         }

//         public bool ExchangeCardStarToReward(int index)
//         {
//             //if (_cardStarExchangeIndex.Value >= index)
//             //{
//             //    PopupToast.Cretate("You have already exchanged this reward");
//             //    return false;
//             //}
//             var milestone = starExchangeConfig.milestones[index];
//             var neededStar = milestone.star;
//             var reward = milestone.reward;

//             if (CardStar < neededStar)
//             {
//                 PopupToast.Cretate("You don't have enough stars");
//                 return false;
//             }

//             _cardStar.Value -= neededStar;
//             _cardStarExchangeIndex.Value = index;
//             SaveData();

//             var logData = new EarnResourceLogData
//             {
//                 spendType = "card_collection",
//                 spendId = "card_collection",
//                 isFirstBuy = false,
//                 source = "non_iap"
//             };
//             MySonatFramework.inventoryService.AddReward(reward, logData);

//             UIData uiData = new UIData();
//             uiData.Add("Title", "REWARD!");
//             uiData.Add("Reward", reward);
//             uiData.Add("x2", false);
//             PanelManager.Instance.OpenPanel<PopupReward>(uiData);

//             return true;
//         }

//         public bool CheckCompleteAlbum(AlbumType albumType)
//         {
//             var albumConfig = config.GetAlbumConfig(albumType);
//             foreach (var cardType in albumConfig.cards)
//             {
//                 if (_collectedCardTypes.Contains(cardType) == false) return false;
//             }

//             return true;
//         }

//         internal bool CheckHasClaimChest(int idGift)
//         {
//             return _cardStar.Value >= starExchangeConfig.milestones[idGift].star;
//         }
//     }
// }