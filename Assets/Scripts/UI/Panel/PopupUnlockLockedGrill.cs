using System;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using TMPro;
using UnityEngine;


public class PopupUnlockLockedGrill : Panel
{
    [SerializeField] private TMP_Text txtPrice;
    [SerializeField] private GameObject rwdButton;


    private Action onSuccess;
    private ResourceData price;

    private readonly Service<InventoryService> inventoryService = new();
    // private CheckRemoteByLevelCounterHLW checkRemoteByLevelCounter;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        GetData(uiData);
        // checkRemoteByLevelCounter = new CheckRemoteByLevelCounterHLW("by_level_show_rwd_booster_pkr", 9999);
        UpdateLayout();
    }
    private void GetData(UIData uiData)
    {
        if (uiData.TryGet("Price", out ResourceData price))
        {
            this.price = price;
        }
        if (uiData.TryGet("OnSuccess", out Action onSuccess))
        {
            this.onSuccess = onSuccess;
        }
    }

    public override string GetPlacement()
    {
        // string placement = "";
        // switch (selectedObjectType)
        // {
        //     case SelectedObjectType.Tray:
        //         placement = "LO:::pkr_I_add_order";
        //         break;
        //     case SelectedObjectType.Plate:
        //         placement = "LO:::pkr_I_add_slot";
        //         break;
        //     case SelectedObjectType.OctoChef:
        //         placement = "LO:::pkr_I_remove_octochef";
        //         break;
        //     default:
        //         placement = base.GetPlacement();
        //         break;
        // }
        return "LO:::pkr_I_unlock_tray";
    }


    private void UpdateLayout()
    {
        // HideAllLayouts();
        // switch (selectedObjectType)
        // {
        //     case SelectedObjectType.Tray:
        //         var objTray = layouts.FirstOrDefault(x => x.selectedObjectType == SelectedObjectType.Tray);
        //         if (objTray.gameObject) objTray.gameObject.SetActive(true);
        //         break;
        //     case SelectedObjectType.Plate:
        //         var objPlate = layouts.FirstOrDefault(x => x.selectedObjectType == SelectedObjectType.Plate);
        //         if (objPlate.gameObject) objPlate.gameObject.SetActive(true);
        //         break;
        //     case SelectedObjectType.OctoChef:
        //         var objOctoChef = layouts.FirstOrDefault(x => x.selectedObjectType == SelectedObjectType.OctoChef);
        //         if (objOctoChef.gameObject) objOctoChef.gameObject.SetActive(true);
        //         break;
        // }

        if (txtPrice) txtPrice.text = price.quantity.ToString();

        // var level = MySonatFramework.userDataService.GetLevel();
        // if (rwdButton) rwdButton.SetActive(level >= GameRemoteConfigValue.showRewardAdsLevel);
        rwdButton.SetActive(false);
    }

    private void HideAllLayouts()
    {
        // foreach (var layout in layouts)
        // {
        //     layout.gameObject.SetActive(false);
        // }
    }

    public void OnClickUseCoin()
    {
        if (inventoryService.Instance.CanReduce(price.resource, price.quantity))
        {
            var earnType = "unlock_tray";
            var earnId = "HLW_Unlock_Tray";
            var log = new SpendResourceLogData()
            {
                earnType = earnType,
                earnId = earnId,
            };
            inventoryService.Instance.ReduceResource(price.resource, price.quantity,
                log);
            onSuccess?.Invoke();
            Close();
        }
        else
        {
            PopupToast.Cretate("Not enough coin!");
            PanelManager.Instance.OpenPanelByName<ShopPanelBase>("ShopPanel");
        }
    }

    public void OnClickWatchAds()
    {
        var itemType = "rwd_add_order";
        var itemId = "HLW_Add_Order";
        // SonatSDKAdapter.ShowRewardAds(OnWatchedAds, itemType, itemId);
        OnWatchedAds();
    }

    private void OnWatchedAds()
    {
        // checkRemoteByLevelCounter.AddValue(1);
        onSuccess?.Invoke();
        Close();
    }

    public override void Close()
    {
        base.Close();
        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }


}