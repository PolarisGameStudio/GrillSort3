using System;
using System.Collections.Generic;
using System.Linq;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardSubmodule", menuName = "MyGame/SkewerJam/Features/CardCollection/CardSubmodule")]
    public class CardSubmodule : ScriptableObject
    {
        public const string DATA_KEY = "CARD_COLLECTION_CARD_SUBMODULE";
        [SerializeField] private CardCollectionConfigSO config;
        [SerializeField] private CardInventoryModule cardInventoryModule;

        [Space]
        [Header("Normal Random")]
        [SerializeField] private List<float> listProbabilityCardStars;

        [Space]
        [Header("Pack 6")]
        [SerializeField] private List<Pack6Config> listPack6Configs;

        [Space]
        [Header("Special Random")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float probabilityNewCardInPackSpecial = 0.8f;

        private IntDataPref _countPack6;

        public void LoadData()
        {
            _countPack6 = new IntDataPref($"{DATA_KEY}_countPack6", 0);
        }

        public void ResetData()
        {
            _countPack6.Value = 0;
        }

        public List<CardType> GetCardReward(GameResource gameResource)
        {
            var packType = gameResource;
            int quantity = CardPackHelper.GetNumCardInPack(packType);

            switch (gameResource)
            {
                case GameResource.Card_Randomx1:
                case GameResource.Card_Randomx2:
                case GameResource.Card_Randomx3:
                case GameResource.Card_Randomx4:
                case GameResource.Card_Randomx5:
                    return GetNormalRandomCardType(quantity);
                case GameResource.Card_Randomx6:
                    return GetRandomx6(quantity);
                case GameResource.Card_Special:
                    return GetRandomSpecial(quantity);
            }

            return new List<CardType>() { CardType.Card_0_0 };
        }

        private List<CardType> GetNormalRandomCardType(int number)
        {
            var listSelectedCards = new List<CardType>();

            var cardDataToRandom = CardPackHelper.GetDictCardTypeToRandom();
            var dictCard = cardDataToRandom.dictCard;
            var listAllCards = cardDataToRandom.listAllCards;

            for (int i = 0; i < number; i++)
            {
                var randomStar = GetRandomStar();
                var isNewCard = GetRandomIsNewCard();

                var randoms = new List<CardType>();
                if (isNewCard && dictCard[randomStar].listNewCards.Count > 0)
                {
                    randoms = dictCard[randomStar].listNewCards;
                }
                else
                {
                    randoms = dictCard[randomStar].listOldCards;
                }

                randoms.RemoveAll(e => listSelectedCards.Contains(e));
                if (randoms.Count > 0)
                {
                    var randCard = randoms.Rand();
                    listSelectedCards.Add(randCard);
                }
                else
                {
                    // backup random in all cards
                    Debug.Log($"<color=red>[CardSubmodule] datlt: GetNormalRandomCardType: randoms is empty</color>");
                    listAllCards.RemoveAll(e => listSelectedCards.Contains(e));

                    var randomInAllCards = listAllCards.Rand();
                    listSelectedCards.Add(randomInAllCards);
                }
            }
            return listSelectedCards;
        }

        private int GetRandomStar()
        {
            var random = UnityEngine.Random.Range(0.0f, 1.0f);
            var sum = 0.0f;
            for (int i = 0; i < listProbabilityCardStars.Count; i++)
            {
                sum += listProbabilityCardStars[i];
                if (random <= sum)
                {
                    return i + 1;
                }
            }
            return 1;
        }

        private bool GetRandomIsNewCard()
        {
            var random = UnityEngine.Random.Range(0.0f, 1.0f);
            return random < 0.2f;
        }

        #region GetRandomx6
        private List<CardType> GetRandomx6(int number)
        {
            // - Luôn có ít nhất 1 card từ 3 sao trở lên

            // Nếu user mở 2 pack 6 liên tục mà không có new card, pack 6 thứ 3 có 33% tỷ lệ ra new card
            // Nếu vẫn không ra new card, lần tiếp theo đó tỷ lệ tăng lên 66%
            // Nếu vẫn không ra, lần thứ 5 mở pack6 100% ra new card

            var listSelectedCards = new List<CardType>();

            var cardDataToRandom = CardPackHelper.GetDictCardTypeToRandom();
            var dictCard = cardDataToRandom.dictCard;
            var listAllCards = cardDataToRandom.listAllCards;

            _countPack6.Value += 1;

            // Lấy luôn 1 thẻ >= 4 sao
            var card4Star = GetRandomCardByStar(cardDataToRandom, 3);
            listSelectedCards.Add(card4Star);

            if (cardInventoryModule.CheckExistCollectedCard(card4Star) == false) _countPack6.Value = 0;

            // Lấy nốt số thẻ còn lại
            for (int i = 0; i < number - 1; i++)
            {
                // ưu tiên tỉ lệ new card hơn
                var randomStar = GetRandomStar();
                var isNewCard = GetRandomIsNewCardInPack6();

                var randoms = new List<CardType>();
                if (isNewCard)
                {
                    // lấy bằng được new card thì thoi
                    for (int j = 0; j < config.MaxStar; j++)
                    {
                        var jStar = (randomStar + j) % config.MaxStar + 1;
                        if (dictCard[jStar].listNewCards.Count > 0)
                        {
                            randoms = dictCard[jStar].listNewCards;
                            break;
                        }
                    }
                    _countPack6.Value = 0;
                }
                else
                {
                    randoms = dictCard[randomStar].listOldCards;
                }

                randoms.RemoveAll(e => listSelectedCards.Contains(e));
                if (randoms.Count > 0)
                {
                    var rand = randoms.Rand();
                    listSelectedCards.Add(rand);
                }
                else
                {
                    Debug.Log($"<color=red>[CardSubmodule] datlt: GetNormalRandomCardType: randoms is empty</color>");
                    listAllCards.RemoveAll(e => listSelectedCards.Contains(e));
                    var randomInAllCards = listAllCards.Rand();
                    listSelectedCards.Add((CardType)randomInAllCards);
                }
            }


            return listSelectedCards;

        }

        private CardType GetRandomCardByStar(CardDataToRandom cardDataToRandom, int star)
        {
            var listCards = new List<CardType>();
            for (int i = star; i <= config.MaxStar - 1; i++) // không lấy vào card 6 sao (special card)
            {
                listCards.AddRange(cardDataToRandom.dictCard[i].listNewCards);
                listCards.AddRange(cardDataToRandom.dictCard[i].listOldCards);
            }
            return listCards.Rand();
        }

        private bool GetRandomIsNewCardInPack6()
        {
            var p = 0.0f;

            for (int i = listPack6Configs.Count - 1; i >= 0; i--)
            {
                if (_countPack6.Value >= listPack6Configs[i].numberPack6)
                {
                    p = listPack6Configs[i].probabilityNewCard;
                    break;
                }
            }

            if (p == 0.0f) return GetRandomIsNewCard();
            return UnityEngine.Random.Range(0.0f, 1.0f) < p;
        }
        #endregion

        #region GetRandomSpecial
        private List<CardType> GetRandomSpecial(int number)
        {
            var listSelectedCards = new List<CardType>();

            var cardDataToRandom = CardPackHelper.GetDictCardTypeToRandom();
            var dictCard = cardDataToRandom.dictCard;
            var listAllCards = cardDataToRandom.listAllCards;

            var firstCard = ForceSpecialCard(cardDataToRandom, 3);
            listSelectedCards.Add(firstCard);

            var collectNewCard = cardInventoryModule.CheckExistCollectedCard(firstCard) == false;

            for (int i = 0; i < number - 1; i++)
            {
                var randomStar = GetRandomStar();
                var isNewCard = GetRandomIsNewCard();
                if (collectNewCard == false)
                {
                    isNewCard = GetRandomIsNewInPackSpecial();
                }

                var randoms = new List<CardType>();
                if (isNewCard)
                {
                    // lấy bằng được new card thì thoi
                    for (int j = 0; j < config.MaxStar; j++)
                    {
                        var jStar = (randomStar + j) % config.MaxStar + 1;
                        if (dictCard[jStar].listNewCards.Count > 0)
                        {
                            randoms = dictCard[jStar].listNewCards;
                            break;
                        }
                    }
                    _countPack6.Value = 0;
                    collectNewCard = true;
                }
                else
                {
                    randoms = dictCard[randomStar].listOldCards;
                }

                randoms.RemoveAll(e => listSelectedCards.Contains(e));
                if (randoms.Count > 0)
                {
                    var rand = randoms.Rand();
                    listSelectedCards.Add(rand);
                }
                else
                {
                    Debug.Log($"<color=red>[CardSubmodule] datlt: GetNormalRandomCardType: randoms is empty</color>");
                    listAllCards.RemoveAll(e => listSelectedCards.Contains(e));
                    var randomInAllCards = listAllCards.Rand();
                    listSelectedCards.Add((CardType)randomInAllCards);
                }
            }

            return listSelectedCards;
        }

        private CardType ForceSpecialCard(CardDataToRandom cardDataToRandom, int star)
        {
            //- Uu tien special card
            var listCards = new List<CardType>();
            var config = MySonatFramework.GetService<CardCollectionService>().GetConfig();

            if (cardDataToRandom.dictCard[config.MaxStar].listNewCards.Count > 0)
            {
                return cardDataToRandom.dictCard[config.MaxStar].listNewCards.Rand();
            }

            //- Đảm bảo 100% có 1 new card hoặc card có >=4 sao trở lên
            for (int i = 1; i <= config.MaxStar; i++)
            {
                if (i >= star)
                {
                    listCards.AddRange(cardDataToRandom.dictCard[i].listNewCards);
                    listCards.AddRange(cardDataToRandom.dictCard[i].listOldCards);
                }
                else
                {
                    listCards.AddRange(cardDataToRandom.dictCard[i].listNewCards);
                }
            }
            return listCards.Rand();
        }

        private bool GetRandomIsNewInPackSpecial()
        {
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            return rand < probabilityNewCardInPackSpecial;
        }
        #endregion
    }

    [Serializable]
    public class Pack6Config
    {
        public int numberPack6;

        [Range(0.0f, 1.0f)]
        public float probabilityNewCard;
    }
}
