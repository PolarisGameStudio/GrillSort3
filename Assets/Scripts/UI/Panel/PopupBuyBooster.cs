using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Templates.UI.ScriptBase;

public class PopupBuyBooster : PopupBuyBoosterBase
{
    public override void Close()
    {
        base.Close();

        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }
}