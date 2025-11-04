using Cysharp.Threading.Tasks;
using I2.Loc;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection
{
    public class UIAlbum : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField, ReadOnly] private AlbumType albumType;
        [SerializeField] private TMP_Text albumName;
        [SerializeField] private bool isAlbumImage = true;
        [SerializeField, ShowIf("isAlbumImage")] private Image albumImage;
        [SerializeField] private UIRewardItem rewardItem;

        [Header("Data")]
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text txtNumCard;
        [SerializeField] private CardNotificationBadge cardNotificationBadge;

        [Header("Completed")]
        [SerializeField] private GameObject completedObj;
        [SerializeField] private GameObject notCompletedObj;

        protected readonly Service<CardCollectionService> cardCollectionService = new();
        protected AlbumConfigSO albumConfig;
        public AlbumType AlbumType => albumType;

        public virtual void Setup(AlbumType albumType)
        {
            this.albumType = albumType;
            albumConfig = cardCollectionService.Instance.config.GetAlbumConfig(albumType);

            albumName.text = albumConfig.albumName;
            // albumName.GetComponent<Localize>().SetTerm(albumConfig.albumName);
            // albumName.SetMaterial(albumConfig.color);

            //albumImage.sprite = albumConfig.albumIcon;
            if (isAlbumImage)
            {
                albumImage.SetSpriteAsync(albumConfig.GetAlbumSpritePath()).Forget();
            }
            // set reward
            var reward = albumConfig.reward.resourceDatas[0];
            rewardItem.Init(reward.resource, reward.quantity);

            // bubbleReward.SetReward(albumConfig.reward);

            if (cardNotificationBadge)
            {
                cardNotificationBadge.SetData(albumType);
            }
        }

        public virtual void SetData(int numCard, int totalCard)
        {
            slider.value = numCard / (float)totalCard;

            txtNumCard.text = $"{numCard}/{totalCard}";

            // set completed
            if (completedObj != null) completedObj.SetActive(numCard >= totalCard);
            if (notCompletedObj != null) notCompletedObj.SetActive(numCard < totalCard);
        }

        public virtual void UpdateData()
        {
            var inventoryModule = cardCollectionService.Instance.CardInventoryModule;
            var numCard = inventoryModule.GetNumberCollectedCardInAlbum(albumType);
            var totalCard = albumConfig.cards.Count;
            SetData(numCard, totalCard);
        }

        public void OnClick()
        {
            //Debug.Log("OnClick Album");
            PanelManager.Instance.OpenPanel<PopupAlbum>(new UIData().Add("albumType", albumType));

            cardNotificationBadge.gameObject.SetActive(false);
        }
    }
}
