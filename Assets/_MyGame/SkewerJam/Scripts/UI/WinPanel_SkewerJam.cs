using Sonat.Data;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.UserData;
using TMPro;
using UnityEngine;

public class WinPanel_SkewerJam : WinPanelBase
{
    [Header("WinPanel_SkewerJam")]
    [SerializeField] private Transform claimBtn2;
    [SerializeField] private TMP_Text txtReward2;

    [SerializeField] private float delaySoundFireworks = 1f;
    [SerializeField] private float delaySoundReceived = 0.5f;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        Canvas.ForceUpdateCanvases();

        if (Service<UserDataService>.Get().GetLevel() <= SonatSDKAdapter.GetRemoteInt("level_start_x2_coin", 3))
        {
            claimBtn.gameObject.SetActive(false);
            claimBtn2.gameObject.SetActive(true);
            txtReward2.text = data.reward.quantity.ToString();
        }
        else
        {
            claimBtn2.gameObject.SetActive(false);
        }

        MySonatFramework.GetService<AudioService>().StopMusic();
        if (delaySoundFireworks > 0)
        {
            SonatUtils.DelayCall(delaySoundFireworks, () =>
            {
                MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Win_Music_sfx_Grill3);
            }, this);
        }

        var levelStartRate = SonatSDKAdapter.GetRemoteInt("POPUP_RATE_level_start", 3);
        var level = MySonatFramework.userDataService.GetLevel();
        if (level - 1 == levelStartRate)
        {
            SonatUtils.DelayCall(1f, () =>
            {
                PanelManager.Instance.OpenPanel<PopupRate>();
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
