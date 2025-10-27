using System;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Templates.UI.ScriptBase;
using UnityEngine;

public class PopupRefillLives : PopupRefillLivesBase
{
    [SerializeField] private UITimeCounter timeCounterRefill;
    [SerializeField] private UITimeCounter timeCounterUnlimited;
    private Action onConfirm;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (uiData != null)
        {
            uiData.TryGet(UIDataKey.CallBackOnClose, out onConfirm);
        }

        timeCounterRefill.SetData(liveService.Instance.GetTimeRefillRemainView());
        timeCounterUnlimited.SetData(liveService.Instance.GetUnlimitedLiveRemainView());
    }
    public override void Close()
    {
        base.Close();
        onConfirm?.Invoke();
        onConfirm = null;
    }
}