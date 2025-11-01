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
        [SerializeField] private GameObject tickObj;
        [SerializeField] private TMP_Text txtTotalCard;
        [SerializeField] private UIRewardGroup rewardGroup;
        [SerializeField] private UITimeCounter timeCounter;

        private readonly Service<CardCollectionService> _cardCollectionService = new();

        private void OnEnable()
        {
            var reward = _cardCollectionService.Instance.config.rewardInSeason;
            rewardGroup.SetData(reward);
        }

        private void OnDisable()
        {

        }
        public void UpdateData()
        {
            var totalCard = _cardCollectionService.Instance.TotalCard;
            var maxCard = _cardCollectionService.Instance.config.GetNumCard();

            tickObj.SetActive(totalCard >= maxCard);
            cardSlider.value = totalCard * 1.0f / maxCard;
            txtTotalCard.text = $"{totalCard}/{maxCard}";

            timeCounter.SetData(_cardCollectionService.Instance.GetRemainTime(), null);

        }
    }
}