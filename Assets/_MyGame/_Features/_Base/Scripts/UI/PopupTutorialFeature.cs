using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using UnityEngine;

public class PopupTutorialFeature : Panel
{
    [SerializeField] private float delayClose = 10;

    private bool canClose = false;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);
        canClose = false;
        SonatUtils.DelayCall(delayClose, () =>
        {
            canClose = true;
        }, this);
    }

    public override void Close()
    {
        if (canClose)
        {
            base.Close();
        }

    }
}
