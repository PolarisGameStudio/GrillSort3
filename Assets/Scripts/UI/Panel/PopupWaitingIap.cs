using System;
using System.Collections;
using System.Collections.Generic;
using SonatFramework.Scripts.UIModule;
using UnityEngine;

public class PopupWaitingIap : PopupWaitingBase
{
    private bool closed = false;
    public override void Open(UIData uiData)
    {
        closed = false;
        base.Open(uiData);
    }


    public override void Close()
    {
        closed = true;
        base.Close();
    }
    
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!closed && hasFocus)
            Close();
    }
}