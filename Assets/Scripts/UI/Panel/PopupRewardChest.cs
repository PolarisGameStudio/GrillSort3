using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Modules.Scripts.SO;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement.GameResources;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PopupRewardChest : Panel
{
    public const string REWARD_KEY = "REWARD_KEY";
    public const string SKIN_KEY = "SKIN_KEY";

    [SerializeField] private Button tapToOpen;
    [SerializeField] private Button tapToClaim;

    [Space(10)]
    [SerializeField] private Transform animRoot;
    [SerializeField] private SkeletonGraphic chestAnimation;
    [SerializeField] private string[] skins;
    [SerializeField] private UIRewardGroup uiRewardGroup;
    [SerializeField] private ParticleSystem[] psPumpkins;

    [Header("Move")]
    public ParticleSystem psMove;
    [SerializeField] private ParticleSystem psMoveItemAfter;

    [Header("Pumpkin out")]
    [SerializeField] private Transform pumpkinOutPos;

    [Space(10)]
    [Header("Config")]
    [SerializeField] private PopupChestRewardConfigSO configSO;

    private bool isClickClaim = false;
    private bool isClickOpen = false;
    private bool canReceiveReward = false;
    private List<RewardItemEffectController> uiRewardItems = new();
    private List<Vector3> uiRewardItemsPos = new();

    private int skinIndex = 0;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        isClickClaim = false;
        canReceiveReward = false;
        isClickOpen = false;
        tapToClaim.gameObject.SetActive(false);
        tapToOpen.gameObject.SetActive(true);

        foreach (var psPumpkin in psPumpkins)
        {
            psPumpkin.gameObject.SetActive(false);
        }
        psMove.gameObject.SetActive(false);
        psMoveItemAfter.gameObject.SetActive(false);
        var rootPos = animRoot.position;

        if (uiData.TryGet(REWARD_KEY, out RewardData rewardData))
        {
            uiRewardGroup.SetData(rewardData);
            uiRewardItems.Clear();
            uiRewardItemsPos.Clear();
            SonatUtils.ExecuteNextFrame(() =>
            {
                uiRewardItems = uiRewardGroup.GetComponentsInChildren<RewardItemEffectController>().ToList();
                foreach (var uiRewardItem in uiRewardItems)
                {
                    uiRewardItem.SetAlpha(0f);
                    uiRewardItemsPos.Add(uiRewardItem.transform.position); // lưu vị trí ban đầu
                    uiRewardItem.transform.position = rootPos; // đặt vị trí ban đầu
                }
            });

        }

        if (uiData.TryGet(SKIN_KEY, out skinIndex))
        {
            chestAnimation.AnimationState.ClearTracks();
            chestAnimation.Skeleton.SetSkin(skins[skinIndex]);
        }

        SonatUtils.DelayCall(configSO.delayMovePs, () =>
        {
            canReceiveReward = true;
            psMove.gameObject.SetActive(true);
            psMove.Play();
        });
        // }

        MySonatFramework.audioService.PlaySound(AudioId.Chest_Level_Appear);
        chestAnimation.AnimationState.SetAnimation(0, "Appear", false).Complete += (TrackEntry trackEntry) =>
        {
            chestAnimation.AnimationState.SetAnimation(0, "Idle_Before", true);
            MySonatFramework.audioService.PlaySound(AudioId.Chest_Level_Idle);
        };
    }

    public void OnClaimClick()
    {
        if (!canReceiveReward || isClickOpen == true) return;
        isClickOpen = true;
        PlayOpenAnimation().Forget();
    }

    private async UniTask PlayOpenAnimation()
    {
        MySonatFramework.audioService.PlaySound(AudioId.Chest_Level_Open);
        chestAnimation.AnimationState.SetAnimation(0, "Open", false).Complete += (TrackEntry trackEntry) =>
        {
            chestAnimation.AnimationState.SetAnimation(0, "Idle_After", true);
        };

        SonatUtils.DelayCall(configSO.delayMovePsAfter, () =>
        {
            // psMoveItemAfter.transform.position = psPostions[_rank - 1].position;
            psMoveItemAfter.gameObject.SetActive(true);
            psMoveItemAfter.Play();
        });

        await UniTask.Delay((int)(configSO.delaySpawnItem * 1000));
        psPumpkins[skinIndex].gameObject.SetActive(true);
        psPumpkins[skinIndex].Play();
        for (int i = 0; i < uiRewardItems.Count; i++)
        {
            var idx = i;
            uiRewardItems[i].PlayFade(configSO.durationFadeItem);
            uiRewardItems[i].transform.DOScale(1, configSO.durationMoveItem).SetEase(configSO.curveMoveItemScale).From(0);
            uiRewardItems[i].transform.DOMoveX(uiRewardItemsPos[i].x, configSO.durationMoveItem).SetEase(configSO.curveMoveItemX);
            uiRewardItems[i].transform.DOMoveY(uiRewardItemsPos[i].y, configSO.durationMoveItem).SetEase(configSO.curveMoveItemY).OnComplete(() =>
            {
                uiRewardItems[idx].PlayPS();
            });

            await UniTask.Delay((int)(configSO.delayMoveItem * 1000));
        }

        tapToOpen.gameObject.SetActive(false);
        tapToClaim.gameObject.SetActive(true);
    }

    public void CustomClose()
    {
        if (isClickClaim == true) return;
        isClickClaim = true;
        PlayClose().Forget();
    }
    private async UniTask PlayClose()
    {
        await UniTask.Delay((int)(configSO.delayScaleOutItem * 1000));

        SonatUtils.DelayCall(configSO.delayPumpkinOut, () =>
        {
            animRoot.DOMove(pumpkinOutPos.position, configSO.durationPumpkinOut).SetEase(configSO.curvePumpkinOut);
            animRoot.DOScale(0, configSO.durationPumpkinOut).SetEase(configSO.curvePumpkinOut);
        });

        // Tất cả scale về 0
        for (int i = 0; i < uiRewardItems.Count; i++)
        {
            uiRewardItems[i].transform.DOScale(0, configSO.durationScaleOutItem).SetEase(configSO.curveScaleOutItem);
            var idx = i;
            SonatUtils.DelayCall(configSO.delayPsItemOut, () =>
            {
                uiRewardItems[idx].PlayPsHide();
            });
            await UniTask.Delay((int)(configSO.delayItemOut * 1000));
        }

        RewardData rewardData = uiRewardGroup.GetData();

        foreach (var resourceData in rewardData.resourceDatas)
        {
            EventBus<AddItemEvent>.Raise(new() { resource = resourceData.resource, quantity = resourceData.quantity });
        }

        await UniTask.Delay((int)(configSO.delayClose * 1000));
        Close();

    }
}
