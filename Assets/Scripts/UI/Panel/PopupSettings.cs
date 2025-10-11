using System;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
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

        // if (MySonatFramework.livesService.CanPlay())
        // {
        //     UIData data = new UIData();
        //     data.Add("OnConfirm", (Action)ConfirmReplay);
        //     PanelManager.Instance.OpenPanel<PopupLostLives>(data);
        // }
        // else
        // {
        //     PanelManager.Instance.OpenPanel<PopupRefillLives>();
        // }

        // Replay();
        uiButtonSetting.GoOut(false);
        PanelManager.Instance.OpenPanel<PopupAreYouSure>();
    }

    protected override void Replay()
    {
        // Close();
        GameController.Instance.Replay();
    }

    private void ConfirmReplay()
    {
        clicked = true;
        var level = MySonatFramework.userDataService.GetLevel();
        if (level >= GameRemoteConfigValue.levelShowInterReplay)
            SonatSDKAdapter.ShowInterAds("replay", CheckCanReplay);
        else
        {
            CheckCanReplay();
        }
    }

    private void CheckCanReplay()
    {
        if (MySonatFramework.livesService.CanPlay())
        {
            Replay();
            MySonatFramework.livesService.ReduceLive(1, "replay");
        }
        else
        {
            PanelManager.Instance.OpenPanel<PopupRefillLives>(new UIData().Add(UIDataKey.CallBackOnClose, (Action)(AfterRefillLive)));
            PopupToast.Cretate("No more lives left!");
        }
    }

    private void AfterRefillLive()
    {
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

        // if (MySonatFramework.livesService.CanPlay())
        // {
        //     clicked = true;
        //     SonatSDKAdapter.ShowInterAds("BackHome", GoHome);
        // }
        // else
        // {
        //     UIData data = new UIData();
        //     data.Add("OnConfirm", (Action)ConfirmGoHome);
        //     PanelManager.Instance.OpenPanel<PopupLostLives>(data);
        // }
        // ConfirmGoHome();

        uiButtonSetting.GoOut(false);
        PanelManager.Instance.OpenPanel<PopupQuit>();
    }

    private void ConfirmGoHome()
    {
        clicked = true;
        var level = MySonatFramework.userDataService.GetLevel();
        // string cause = GameplayController.instance.levelGenerator.CheckOutOfMove() ? "back_home_ out_of_move" : "back_home";
        // EventBus<LevelQuitEvent>.Raise(new LevelQuitEvent() { cause = cause });
        // EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { gameMode = GameMode.Classic, level = level, success = false });
        // MySonatFramework.livesService.ReduceLive(1, "back_home");
        // if (level >= GameRemoteConfigValue.levelShowInterLose)
        // {
        //     SonatSDKAdapter.ShowInterAds("BackHome", GoHome);
        // }
        // else
        // {
        //     GoHome();
        // }

        PopupToast.Cretate("Go Home");
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
        // PanelManager.Instance.OpenPanel<PopupRate>();
    }

    protected override void GoHome()
    {
        // var consecutiveWinService = MySonatFramework.GetService<ConsecutiveWinService>();
        // var level = MySonatFramework.userDataService.GetLevel();
        // // load lại prewin
        // if (consecutiveWinService.CheckStart(level))
        // {
        //     GameplayController.instance.Replay();
        // }
        // else
        // {
        LoadHomeScene();
        // }
    }

    public void OpenFanPage()
    {
        Application.OpenURL("https://www.facebook.com/people/Grill-Sorting-Food-Challenge/61579717830509/#");
    }

    public void OpenGroup()
    {
        Application.OpenURL("https://www.facebook.com/groups/1610718010312520");
    }
}