using System;
using Gameplay.Entities.Items;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Scripts.SO.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using SonatFramework.Systems.SceneManagement;
using TMPro;
using UnityEngine;

public class PopupSkipBomb : Panel
{
    private ItemBombMove itemBomb;
    private Action OnSkipBomb;
    private Action OnGiveUp;
    [SerializeField] private TMP_Text txtBomb;
    [SerializeField] private TMP_Text txtPrice;
    [SerializeField] private GameObject buyWithAdsBtn;
    [SerializeField] private GameplayConfig_SkewerJam gameplayConfig;
    // private CheckRemoteByDayCounter checkRwdDay;
    // private CheckRemoteByLevelCounter checkRwdLevel;

    private ResourceData skipBombPrice;

    public override void OnSetup()
    {
        base.OnSetup();

        skipBombPrice = gameplayConfig.skipBombPrice;
        txtPrice.text = skipBombPrice.quantity.ToString();
        // checkRwdDay = new CheckRemoteByDayCounter("by_day_show_rwd_skip_bomb", 999);
        // checkRwdLevel = new CheckRemoteByLevelCounter("by_level_show_rwd_skip_bomb", 999);
    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);
        uiData.TryGet("Bomb", out itemBomb);
        txtBomb.text = itemBomb.MoveRemaining.ToString();
        if (!uiData.TryGet("OnSkipBomb", out OnSkipBomb))
        {
            OnSkipBomb = null;
        }

        if (!uiData.TryGet("OnGiveUp", out OnGiveUp))
        {
            OnGiveUp = null;
        }

        buyWithAdsBtn.SetActive(false);
    }

    public void BuyWithCoinClick()
    {
        if (MySonatFramework.inventoryService.CanReduce(skipBombPrice.resource, skipBombPrice.quantity))
        {
            SpendResourceLogData logData = new SpendResourceLogData()
            {
                earnType = "booster",
                earnId = "skip_bomb",
                source = "non_iap"
            };
            MySonatFramework.inventoryService.ReduceResource(skipBombPrice.resource, skipBombPrice.quantity, logData);
            OnSuccess();
        }
        else
        {
            PanelManager.Instance.OpenPanelByName<ShopPanelBase>("ShopPanel");
            PopupToast.Cretate("Not enough coin!");
        }
    }

    public void BuyWithAdsClick()
    {
        SonatSDKAdapter.ShowRewardAds(() =>
        {
            // checkRwdDay.AddValue();
            // checkRwdLevel.AddValue();
            OnSuccess();
        }, "booster", "skip_bomb");
    }


    public void OnGiveUpClick()
    {
        Close();
        OnGiveUp?.Invoke();
    }

    private void OnSuccess()
    {
        itemBomb.SkipBomb();
        Close();
        OnSkipBomb?.Invoke();
    }

    public override void Close()
    {
        base.Close();
        if (MySonatFramework.GetService<SceneService>().GetCurrentGamePlacement() == GamePlacement.Gameplay_SkewerJam)
        {
            if (GameController.Instance.GameState != GameState.GameOver)
            {
                GameController.Instance.ChangeGameState(GameState.Playing);
            }
        }
    }
}