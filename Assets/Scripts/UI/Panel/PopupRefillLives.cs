using System;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Templates.UI.ScriptBase;

public class PopupRefillLives : PopupRefillLivesBase
{
    private Action onConfirm;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (uiData != null)
        {
            uiData.TryGet(UIDataKey.CallBackOnClose, out onConfirm);
        }
    }
    public override void Close()
    {
        base.Close();
        onConfirm?.Invoke();
        onConfirm = null;
    }
}