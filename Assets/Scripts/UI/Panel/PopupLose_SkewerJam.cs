using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Systems;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Scripts.Feature.Lives;
using Manager;
using SonatFramework.Scripts.SonatSDKAdapterModule;

public class PopupLose_SkewerJam : Panel
{
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        MySonatFramework.GetService<AudioService>().StopMusic();
        MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Lose_Giveup_Grill3);
    }

    public void OnClickRetry()
    {
        base.Close();
        var level = MySonatFramework.userDataService.GetLevel();
        if (MySonatFramework.GetService<LivesService>().CanPlay())
        {
            if (level >= GameRemoteConfigValue.levelShowInterLose)
            {
                SonatSDKAdapter.ShowInterAds("Lose_Retry", () =>
                {
                    GameController.Instance.Replay();
                });
            }
            else
            {
                GameController.Instance.Replay();
            }
        }
        else
        {
            if (level >= GameRemoteConfigValue.levelShowInterLose)
            {
                SonatSDKAdapter.ShowInterAds("Lose_GoHome", () =>
                {
                    GameplayHelper.GoHome();
                });
            }
            else
            {
                GameplayHelper.GoHome();
            }
        }
    }

    public void OnClickHome()
    {
        base.Close();
        var level = MySonatFramework.userDataService.GetLevel();
        if (level >= GameRemoteConfigValue.levelShowInterLose)
        {
            SonatSDKAdapter.ShowInterAds("Lose_GoHome", () =>
            {
                GameplayHelper.GoHome();
            });
        }
        else
        {
            GameplayHelper.GoHome();
        }
    }
}