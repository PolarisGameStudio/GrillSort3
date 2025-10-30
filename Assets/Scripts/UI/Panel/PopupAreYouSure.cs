using System;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;

public class PopupAreYouSure : Panel
{
    private Action<Action> onConfirm;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (uiData != null)
        {
            uiData.TryGet("OnConfirm", out onConfirm);
        }
    }

    public override void Close()
    {
        base.Close();
        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }

    public void OnContinueClick()
    {
        Close();
    }

    public void OnRetryClick()
    {
        onConfirm?.Invoke(() =>
        {
            Close();
        });
    }
}
