using Manager;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using SonatFramework.Systems.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Gameplay.PackBooster
{
    public class UIPackBooster : MonoBehaviour
    {
        [SerializeField] private UIRewardGroup rewardGroup;
        [SerializeField] private TMP_Text txtPrice;
        [SerializeField] private FixedImageRatio icon;
        [SerializeField] private Button btnBuy;
        [SerializeField] private Button btnBuyWithAds;

        private readonly Service<SonatBoosterService> boosterService = new();
        private readonly Service<InventoryService> inventoryService = new();
        private RewardData rewardData;
        private ResourceData price;

        public void SetData(RewardData rewardData, ResourceData price, bool canUseAds = false)
        {
            this.rewardData = rewardData;
            this.price = price;
            rewardGroup.SetData(rewardData);
            txtPrice.text = price.quantity.ToString();

            if (canUseAds)
            {
                var level = MySonatFramework.GetService<UserDataService>().GetLevel();
                btnBuyWithAds.gameObject.SetActive(level >= GameRemoteConfigValue.showRewardAdsLevelAddPackBooster);
                btnBuy.gameObject.SetActive(level < GameRemoteConfigValue.showRewardAdsLevelAddPackBooster);
            }
            else{
                btnBuyWithAds.gameObject.SetActive(false);
                btnBuy.gameObject.SetActive(true);
            }
        }

        public void OnBuyClick()
        {
            if (inventoryService.Instance.CanReduce(price.resource, price.quantity))
            {
                var logSpend = new SpendResourceLogData()
                {
                    earnType = price.resource.ToString(),
                    earnId = price.resource.ToString(),
                    source = "non_iap"
                };
                inventoryService.Instance.ReduceResource(price.resource, price.quantity, logSpend);

                var logEarn = new EarnResourceLogData()
                {
                    spendType = price.resource.ToString(),
                    spendId = price.resource.ToString(),
                    source = "non_iap"
                };
                inventoryService.Instance.AddReward(rewardData, logEarn, false);

                EventBus<AddItemEvent>.Raise(new AddItemEvent()
                {
                    resource = rewardData.resourceDatas[0].resource,
                    quantity = rewardData.resourceDatas[0].quantity,
                    position = icon.transform.position,
                    collectEffect = new CollectEffectMultiple()
                    {
                        radius = 0,
                        scale = 1.5f
                    }
                });

                PanelManager.Instance.ClosePanel<PopupBuyBooster2>();
            }
            else
            {
                PopupToast.Cretate("Not enough coin!");
                PanelManager.Instance.OpenPanelByName<ShopPanelBase>("ShopPanel");
            }
        }

        public void OnBuyWithAdsClick()
        {
            if (!MySonatFramework.IsRewardAdsReady())
            {
                return;
            }
            SonatSDKAdapter.ShowRewardAds(OnWatchedAds, "booster", rewardData.resourceDatas[0].resource.ToString());
        }

        private void OnWatchedAds()
        {
            var logEarn = new EarnResourceLogData()
            {
                spendType = "rw_ads",
                spendId = "rw_ads",
                source = "non_iap"
            };
            boosterService.Instance.AddBooster(rewardData.resourceDatas[0].resource, 1, logEarn);
            EventBus<AddItemEvent>.Raise(new AddItemEvent()
            {
                resource = rewardData.resourceDatas[0].resource,
                quantity = 1,
                position = icon.transform.position,
                collectEffect = new CollectEffectSingle()
                {
                    scale = 1.5f
                }
            });
            PanelManager.Instance.ClosePanel<PopupBuyBooster2>();
        }
    }
}