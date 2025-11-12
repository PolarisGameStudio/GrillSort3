using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement.GameResources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection.CardStarExchange
{
    public class ChestController : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] txtsStarNeed;
        [SerializeField] private UIBubbleReward bubbleReward;
        [SerializeField] private Button btnGreen;
        [SerializeField] private Button btnGray;
        [SerializeField] private Button btnClaimed;

        private readonly Service<CardCollectionService> _cardCollectionService = new();

        private int index = 0;
        private RewardData reward;

        public void SetData(int index)
        {
            this.index = index;

            var starModule = _cardCollectionService.Instance.StarSubmodule;
            var star = starModule.starExchangeConfig.milestones[index].star;
            reward = starModule.starExchangeConfig.milestones[index].reward;

            for (int i = 0; i < txtsStarNeed.Length; i++)
            {
                txtsStarNeed[i].text = star.ToString();
            }
            bubbleReward.SetReward(reward);

            UpdateUI();
        }

        public void OnClick()
        {
            var starModule = _cardCollectionService.Instance.StarSubmodule;
            var (success, message) = starModule.OnClickReceiveChest(index);
            if (success)
            {
                UpdateUI();
                var uiData = new UIData();
                uiData.Add(PopupRewardChest.REWARD_KEY, reward);
                uiData.Add(PopupRewardChest.SKIN_KEY, index);
                PanelManager.Instance.OpenPanelByName<PopupRewardChest>("PopupRewardChest_CardCollection", uiData);
            }
            else
            {
                PopupToast.Cretate(message);
            }
        }

        public void UpdateUI()
        {
            var starModule = _cardCollectionService.Instance.StarSubmodule;
            var canClick = starModule.CanReceiveChest(index);

            if (starModule.CardStarExchangeIndex >= index)
            {
                btnGreen.gameObject.SetActive(false);
                btnGray.gameObject.SetActive(false);
                btnClaimed.gameObject.SetActive(true);
            }
            else
            {
                btnClaimed.gameObject.SetActive(false);
                btnGray.gameObject.SetActive(!canClick);
                btnGreen.gameObject.SetActive(canClick);
            }
        }
    }
}
