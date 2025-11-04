using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;
using UnityEngine.UI;

public class PopupReward : Panel
{
    public const string REWARD_KEY = "Reward";
    public const string ON_CLAIM_COMPLETE_KEY = "OnClaimComplete";

    [SerializeField] private UIRewardGrid rewardGrid;
    [SerializeField] private Image bg;
    [SerializeField] private Transform btnObj, titleObj;

    [Header("Anim")]
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float delayBetweenItemsAppear = 0.1f;
    [SerializeField] private float delayBetweenItems = 0.3f;

    private RewardData rewardData;
    private Action onClaimComplete;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (uiData.TryGet(REWARD_KEY, out rewardData))
        {
            rewardGrid.SetReward(rewardData);

            uiData.TryGet(ON_CLAIM_COMPLETE_KEY, out onClaimComplete);
            clicked = false;
        }

        PlayAppearReward().Forget();
    }

    private async UniTask PlayAppearReward()
    {
        foreach (Transform uiItemGroup in rewardGrid.transform)
        {
            foreach (Transform item in uiItemGroup.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    item.transform.localScale = Vector3.zero;
                }
            }
        }

        foreach (Transform uiItemGroup in rewardGrid.transform)
        {
            foreach (Transform item in uiItemGroup.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    item.DOScale(1, duration).SetEase(Ease.OutBack);
                    await UniTask.Delay((int)(delayBetweenItemsAppear * 1000));
                }
            }
        }
    }

    private bool clicked = false;
    public void ClaimClick()
    {
        if (clicked) return;
        clicked = true;
        ClaimClickAsync().Forget();

        DisableAllUI();
    }

    private void DisableAllUI()
    {
        titleObj.DOKill();
        btnObj.DOKill();
        titleObj.DOScale(0, 0.3f).SetEase(Ease.InBack);
        btnObj.DOScale(0, 0.3f).SetEase(Ease.InBack);
    }

    private async UniTask ClaimClickAsync()
    {
        await ReceiveItems();
        onClaimComplete?.Invoke();
        Close();
    }

    // public void ClaimX2Click()
    // {
    //     if (clicked) return;
    //     if (SonatSDKAdapter.IsRewardAdsReady())
    //         clicked = true;
    //     SonatSDKAdapter.ShowRewardAds(OnClickX2Reward, "x2_reward", "x2_reward");

    //     DisableAllUI();
    // }

    // private void OnClickX2Reward()
    // {
    //     OnClickX2RewardAsync().Forget(); // chạy async mà không cần chờ, không bị warning
    //     onClaimComplete?.Invoke();
    //     Close();
    // }

    // private async UniTask OnClickX2RewardAsync()
    // {
    //     await ReceiveItemsX2(); // chờ chạy xong toàn bộ animation
    // }

    // private async UniTask ReceiveItemsX2()
    // {
    //     bg.DOFade(0, 0.3f).SetEase(Ease.OutCubic);
    //     var logData = new EarnResourceLogData
    //     {
    //         spendType = "x2_reward",
    //         spendId = "x2_reward",
    //         isFirstBuy = false,
    //         source = "non_iap"
    //     };

    //     MySonatFramework.GetService<InventoryService>().AddReward(rewardData, logData);
    //     foreach (var resource in rewardData.resourceDatas)
    //     {
    //         if (resource.resource == GameResource.Coin)
    //         {
    //             SonatCollectEffect collectEffect = new CollectEffectMultiple();
    //             EventBus<AddItemEvent>.Raise(new AddItemEvent()
    //             {
    //                 position = Vector3.zero,
    //                 resource = resource.resource,
    //                 quantity = resource.quantity * 2,
    //                 collectEffect = collectEffect
    //             });
    //         }
    //         else
    //         {
    //             SonatCollectEffect collectEffect = new CollectEffectSingle();
    //             EventBus<AddItemEvent>.Raise(new AddItemEvent()
    //             {
    //                 position = transform.position,
    //                 resource = resource.resource,
    //                 quantity = resource.quantity * 2,
    //                 collectEffect = collectEffect
    //             });
    //         }


    //         await UniTask.Delay(200); // thay cho yield return WaitForSeconds(0.2f)
    //     }

    //     MySonatFramework.GetService<InventoryService>().NotiUpdateResource();
    // }

    private async UniTask ReceiveItems()
    {
        bg.DOFade(0, 0.3f).SetEase(Ease.OutCubic);
        foreach (var resource in rewardData.resourceDatas)
        {
            UIRewardItem rewardItem = rewardGrid.GetRewardItem(resource.resource);
            if (rewardItem != null)
            {
                rewardItem.GetComponent<CanvasGroup>().alpha = 0;
            }

            var quantity = resource.resource == GameResource.Lives ? 1 : resource.quantity;

            SonatCollectEffect collectEffect;
            if (quantity > 10)
            {
                collectEffect = new CollectEffectMultiple()
                {
                    radius = 0.75f,
                };
            }
            else
            {
                collectEffect = new CollectEffectMultiple()
                {
                    radius = 0.2f
                };
            }

            EventBus<AddItemEvent>.Raise(new AddItemEvent()
            {
                position = rewardItem != null ? rewardItem.transform.position : Vector3.zero,
                resource = resource.resource,
                quantity = quantity,
                collectEffect = collectEffect
            });

            await UniTask.Delay((int)(delayBetweenItems * 1000)); // thay cho yield return WaitForSeconds(0.2f)
        }
        MySonatFramework.GetService<InventoryService>().NotiUpdateResource();
    }

    // private void OnApplicationFocus(bool hasFocus)
    // {
    //     if (hasFocus)
    //     {
    //         SonatUtils.DelayCall(1f, () =>
    //         {
    //             if (clicked && !receivedX2) Close();
    //         }, this);
    //     }
    // }

    // private RewardData ValidateReward(RewardData rewardData)
    // {
    //     var newRewardData = new RewardData();
    //     foreach (var resource in rewardData.resourceDatas)
    //     {
    //         // if (GameResourceHelper.ResourceType(resource.resource) == GameResourceType.Card)
    //         // {
    //         //     var cardCollectionService = MySonatFramework.GetService<CardCollectionService>();
    //         //     if (cardCollectionService == null || cardCollectionService.IsUnlocked() == false)
    //         //     {
    //         //         continue;
    //         //     }
    //         // }
    //         newRewardData.AddReward(resource);
    //     }
    //     return newRewardData;
}
