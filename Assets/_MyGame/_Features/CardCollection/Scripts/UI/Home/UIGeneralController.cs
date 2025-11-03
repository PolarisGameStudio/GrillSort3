using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection.Home
{
    public class UIGeneralController : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private Slider cardSlider;
        [SerializeField] private UIBubbleReward bubbleReward;
        [SerializeField] private GameObject tickObj;
        [SerializeField] private TMP_Text txtTotalCard;
        [SerializeField] private UIRewardGroup rewardGroup;
        [SerializeField] private UITimeCounter timeCounter;

        private readonly Service<CardCollectionService> _cardCollectionService = new();
        private bool _isInit = false;

        private void OnEnable()
        {
            if (_isInit == false)
            {
                _isInit = true;
                var reward = _cardCollectionService.Instance.config.rewardInSeason;
                rewardGroup.SetData(reward);
                bubbleReward.SetReward(reward);
            }

            UpdateData();
        }

        private void OnDisable()
        {

        }

        public void UpdateData()
        {
            var cardModule = _cardCollectionService.Instance.CardSubmodule;
            var totalCard = cardModule.TotalCard;
            var maxCard = _cardCollectionService.Instance.config.GetNumCard();

            bubbleReward.gameObject.SetActive(totalCard < maxCard);
            tickObj.SetActive(totalCard >= maxCard);
            cardSlider.value = totalCard * 1.0f / maxCard;
            txtTotalCard.text = $"{totalCard}/{maxCard}";

            timeCounter.SetData(_cardCollectionService.Instance.GetRemainTime(), null);

        }
    }
}