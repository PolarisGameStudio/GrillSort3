using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;

public class PopupAreYouSure : Panel
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

    public void OnRetryClick()
    {
        Close();
        GameController.Instance.Replay();
    }
}
