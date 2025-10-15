using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Systems;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Scripts.Feature.Lives;

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
        if(MySonatFramework.GetService<LivesService>().CanPlay())
        {
            GameController.Instance.Replay();
        }
        else
        {
            GameplayHelper.GoHome();
        }
    }

    public void OnClickHome()
    {
        base.Close();
        GameplayHelper.GoHome();
    }
}
