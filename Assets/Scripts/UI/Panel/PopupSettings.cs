using System;
using Manager;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.SceneManagement;
using UnityEngine;

public class PopupSettings : PopupSettingsBase
{
    [SerializeField] private UIButtonSetting uiButtonSetting;

    public void ResetClick()
    {
        clicked = false;
    }

    public override void ReplayClick()
    {
        if (clicked) return;

        clicked = true;
        uiButtonSetting.GoOut(false);
        UIData data = new UIData();
        data.Add("OnConfirm", (Action)ConfirmReplay);
        PanelManager.Instance.OpenPanel<PopupAreYouSure>(data);
    }

    protected override void Replay()
    {
        GameController.Instance.Replay();
    }

    private void ConfirmReplay()
    {
        if (CanReplay())
        {
            var level = MySonatFramework.userDataService.GetLevel();
            if (level >= GameRemoteConfigValue.levelShowInterReplay)
                SonatSDKAdapter.ShowInterAds("replay", CheckCanReplay);
            else
            {
                CheckCanReplay();
            }
        }
        else
        {
            // PanelManager.Instance.OpenPanel<PopupRefillLives>(new UIData().Add(UIDataKey.CallBackOnClose, (Action)(AfterRefillLive)));
            PopupToast.Cretate("Not enough lives to replay!");
        }
    }

    private bool CanReplay()
    {
        return MySonatFramework.livesService.isUnlimitedLive.BoolValue || MySonatFramework.GetService<InventoryService>().GetResource(GameResource.Lives) >= 2;
    }

    private void CheckCanReplay()
    {
        // if (MySonatFramework.livesService.CanPlay())
        // {
        MySonatFramework.livesService.ReduceLive(1, "replay");
        AfterRefillLive();
        // }
        // else
        // {
        //     PanelManager.Instance.OpenPanel<PopupRefillLives>(new UIData().Add(UIDataKey.CallBackOnClose, (Action)(AfterRefillLive)));
        //     PopupToast.Cretate("No more lives left!");
        // }
    }

    private void AfterRefillLive()
    {
        var level = MySonatFramework.userDataService.GetLevel();
        EventBus<LevelQuitEvent>.Raise(new LevelQuitEvent() { cause = "replay" });
        if (MySonatFramework.livesService.CanPlay())
        {
            Replay();
        }
        else
        {
            GoHome();
        }
    }

    public override void HomeClick()
    {
        if (clicked) return;

        uiButtonSetting.GoOut(false);
        UIData data = new UIData();
        data.Add("OnConfirm", (Action)ConfirmGoHome);
        PanelManager.Instance.OpenPanel<PopupQuit>(data);
    }

    private void ConfirmGoHome()
    {
        clicked = true;
        var level = MySonatFramework.userDataService.GetLevel();
        // string cause = GameplayController.instance.levelGenerator.CheckOutOfMove() ? "back_home_ out_of_move" : "back_home";
        EventBus<LevelQuitEvent>.Raise(new LevelQuitEvent() { cause = "back_home" });
        // EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { gameMode = GameMode.Classic, level = level, success = false });
        MySonatFramework.livesService.ReduceLive(1, "back_home");
        if (level >= GameRemoteConfigValue.levelShowInterLose)
        {
            SonatSDKAdapter.ShowInterAds("BackHome", GoHome);
        }
        else
        {
            GoHome();
        }
    }

    protected override void LoadHomeScene()
    {
        // LoadingScreenInstance.Instance.Show(1.5f);
        SonatUtils.DelayCall(0.5f, () => { MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Home); });
    }

    public void LanguageClick()
    {
        // PanelManager.Instance.OpenPanel<PopupLanguage>();
    }

    public void RateClick()
    {
        PanelManager.Instance.OpenPanel<PopupRate>();
    }

    protected override void GoHome()
    {
        GameplayHelper.GoHome();
    }

    public void OpenFanPage()
    {
        Application.OpenURL("https://www.facebook.com/groups/1526661221663574");
    }

    public void OpenGroup()
    {
        Application.OpenURL("https://www.facebook.com/groups/1526661221663574");
    }
}