using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

public class WinPanel_SkewerJam : WinPanelBase
{
    [Header("WinPanel_SkewerJam")]
    [SerializeField] private float delaySoundFireworks = 1f;
    [SerializeField] private float delaySoundReceived = 0.5f;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        Canvas.ForceUpdateCanvases();

        MySonatFramework.GetService<AudioService>().StopMusic();
        if (delaySoundFireworks > 0)
        {
            SonatUtils.DelayCall(delaySoundFireworks, () =>
            {
                MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Win_Music_sfx_Grill3);
            }, this);
        }
    }

    public override void OnClaimClick()
    {
        base.OnClaimClick();

        MySonatFramework.audioService.StopMusic();
        SonatUtils.DelayCall(delaySoundReceived, () =>
        {
            MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Win_HLW_Pumkin_Received_Grill_sort);
        }, this);
    }
}
