using Cysharp.Threading.Tasks;
using MyGame.Modules.CardCollection;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.ConfigManagement;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using System;
using UnityEngine;

public class UICardCollectionWidget : UIHomeWidget
{
    private readonly Service<CardCollectionService> _cardCollectionService = new();

    public override void Setup()
    {
        if (_cardCollectionService.Instance.IsUnlocked() == false)
        {
            if (_cardCollectionService.Instance.CanUnlock() == true)
            {
                _cardCollectionService.Instance.Unlock();
                return;
            }
        }

    }

    public override void OnFocus()
    {

    }

    public override void OnLoseFocus()
    {

    }

    // Chỉ hiện ở phiên đầu tiên trong ngày
    public override async UniTask<bool> ProcessTask()
    {
        if (_cardCollectionService.Instance.IsUnlocked() == false) return false;

        var open = false;
        // Hiện tut
        if (PlayerPrefs.HasKey($"{CardCollectionService.DATA_KEY}_AppearTut") == false)
        {
            PlayerPrefs.SetInt($"{CardCollectionService.DATA_KEY}_AppearTut", 1);

            UIData uiData = new();
            uiData.Add(UIDataKey.CallBackOnClose, (Action)(() =>
            {
                RewardUnlockFeature();
            }));

            _ = HomeManager.Instance.SwitchTab(NavigationType.CardCollection, 0.25f);

            var popup = PanelManager.Instance.OpenPanelByName<Panel>("PopupTutCardCollection", uiData);
            var popup2 = PanelManager.Instance.GetPanel<PopupReward>();

            await UniTask.WaitUntil(() => popup == null || popup.gameObject.activeInHierarchy == false);
            await UniTask.WaitUntil(() => popup2 == null || popup2.gameObject.activeInHierarchy == false);

            open = true;
        }

        // Hiện popup complete album
        var openCompleteAlbum = await _cardCollectionService.Instance.RunQueueCompleteAlbum();
        open = open || openCompleteAlbum;
        return open;

    }

    private void RewardUnlockFeature()
    {
        RewardData reward = _cardCollectionService.Instance.config.RewardUnlock;
        MySonatFramework.inventoryService.AddReward(reward, new EarnResourceLogData
        {
            spendType = "card_collection",
            spendId = "card_collection",
            isFirstBuy = false,
            source = "non_iap"
        });
        UIData uiData = new UIData();
        uiData.Add("Title", "REWARD!");
        uiData.Add("Reward", reward);
        uiData.Add("x2", false);
        PanelManager.Instance.OpenPanel<PopupReward>(uiData);
    }


    // #if UNITY_EDITOR
    //     void Update()
    //     {
    //         if (Input.GetKeyDown(KeyCode.Space))
    //         {
    //             var albumType = AlbumType.Album_0;
    //             PanelManager.Instance.OpenPanel<PopupCompleteAlbum>(new UIData().Add("AlbumType", albumType));
    //         }
    //     }
    // #endif
}
