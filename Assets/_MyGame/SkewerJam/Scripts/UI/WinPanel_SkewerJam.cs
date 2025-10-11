using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

public class WinPanel_SkewerJam : WinPanelBase
{

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        Canvas.ForceUpdateCanvases();
    }

    public override void OnClaimClick()
    {
        base.OnClaimClick();
    }
}
