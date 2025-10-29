using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.SceneManagement;

public class PopupPackBase : Panel
{
    public override void Open(UIData uiData)
    {
        base.Open(uiData);
    }

    public override void Close()
    {
        base.Close();

        if (MySonatFramework.GetService<SceneService>().GetCurrentGamePlacement() == GamePlacement.Gameplay_SkewerJam)
        {
            GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
        }
    }
}