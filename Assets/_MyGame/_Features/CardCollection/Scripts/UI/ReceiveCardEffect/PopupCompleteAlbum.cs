using DG.Tweening;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection
{
    public class PopupCompleteAlbum : Panel
    {
        public const string ALBUM_TYPE_KEY = "AlbumType";
        [SerializeField] private UIAlbum album;

        [SerializeField] private Slider slider;
        [SerializeField] private UIRewardItem rewardItem;


        [Header("Animation Completed Obj")]
        [SerializeField] private float durationSlider = 0.3f;
        [SerializeField] private float delaySlider = 0.5f;
        [SerializeField] private AnimationCurve curveSlide = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private float durationSliderScaleDown = 0.3f;
        [SerializeField] private float delaySliderScaleDown = 0.5f;
        [SerializeField] private AnimationCurve curveSliderScaleDown = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private readonly Service<CardCollectionService> cardCollectionService = new();
        private AlbumType albumType;
        private RewardData reward;
        public override void Open(UIData uiData)
        {
            base.Open(uiData);

            if (uiData.TryGet<AlbumType>(ALBUM_TYPE_KEY, out albumType))
            {
                album.Setup(albumType);

                var albumConfig = cardCollectionService.Instance.GetConfig().GetAlbumConfig(albumType);
                reward = albumConfig.reward;
                rewardItem.Init(reward.resourceDatas[0].resource, reward.resourceDatas[0].quantity);
            }
            PlayAnimationSlider();
        }

        private void PlayAnimationSlider()
        {
            slider.value = 0;
            slider.DOValue(1, durationSlider).SetEase(curveSlide).SetDelay(delaySlider);
            slider.transform.DOScale(0, durationSliderScaleDown).SetEase(curveSliderScaleDown).SetDelay(delaySliderScaleDown);

        }

        public void OnClickClaim()
        {
            Close();

            UIData uiData = new UIData();
            uiData.Add(PopupReward.REWARD_KEY, reward);
            PanelManager.Instance.OpenPanel<PopupReward>(uiData);
        }
    }
}