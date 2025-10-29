using System;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Templates.UI.ScriptBase;
using UnityEngine;

public class PopupRefillLives : PopupRefillLivesBase
{
    [SerializeField] private UITimeCounter timeCounterRefill;
    [SerializeField] private UITimeCounter timeCounterUnlimited;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        timeCounterRefill.SetData(liveService.Instance.GetTimeRefillRemainView());
        timeCounterUnlimited.SetData(liveService.Instance.GetUnlimitedLiveRemainView());
    }
}