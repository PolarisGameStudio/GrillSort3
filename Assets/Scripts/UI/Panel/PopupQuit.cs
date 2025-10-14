using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.SceneManagement;

public class PopupQuit : Panel
{
    public override void Open(UIData uiData)
    {
        base.Open(uiData);
    }

    public void OnContinueClick()
    {
        Close();
        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }

    public void OnQuitClick()
    {
        Close();
        // GameplayHelper.GoHome();

        // GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
        // PopupToast.Cretate("Go Home");

        PanelManager.Instance.OpenPanel<PopupLoading>(new UIData().Add("Time", 2f));
        SonatUtils.DelayCall(2f, () =>
        {
            MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Home);
        });
    }
}