using Manager;
using Sonat.Data;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.UserData;
using TMPro;
using UnityEngine;

public class WinPanel_SkewerJam : WinPanelBase
{
    [Header("WinPanel_SkewerJam")]
    [SerializeField] private Transform claimBtn2;
    [SerializeField] private TMP_Text txtReward2;

    [Space(10)]
    [SerializeField] private UIWheel uiWheel;
    [SerializeField] private TMP_Text txtx2Reward;

    [SerializeField] private float delaySoundFireworks = 1f;
    [SerializeField] private float delaySoundReceived = 0.5f;

    private int _multiplier = 1;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        Canvas.ForceUpdateCanvases();

        if (Service<UserDataService>.Get().GetLevel() <= SonatSDKAdapter.GetRemoteInt("level_start_x2_coin", 3))
        {
            claimBtn.gameObject.SetActive(false);
            claimBtn2.gameObject.SetActive(true);
            txtReward2.text = data.reward.quantity.ToString();
            uiWheel.gameObject.SetActive(false);
            _multiplier = 1;
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

        var levelStartRate = GameRemoteConfigValue.levelStartRate;
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
        uiWheel?.StopWheel();
    }

    public override void OnClaimX2Click()
    {
        base.OnClaimX2Click();
        uiWheel?.StopWheel();
    }

    protected override void OnWatchedVideo()
    {
        collected = true;
        var log = new EarnResourceLogData()
        {
            spendType = "win_x2_coin",
            spendId = "win_x2_coin" + _multiplier
        };
        Service<InventoryService>.Get().AddResource(data.reward.resource, data.reward.quantity * _multiplier, log, false);
        OnClaimReward(x2CoinBtn, data.reward.quantity * _multiplier);
        //collectEffectCurveStream.CreateEffect(icon.transform.position, data.reward.resource, 15, NextLevel);
    }

    protected override void OnClaimReward(Transform btn, int quantity)
    {
        base.OnClaimReward(btn, quantity);

        MySonatFramework.audioService.StopMusic();
        SonatUtils.DelayCall(delaySoundReceived, () =>
        {
            MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Win_HLW_Pumkin_Received_Grill_sort);
        }, this);
    }

    private void Update()
    {
        if (uiWheel != null && uiWheel.gameObject.activeInHierarchy)
        {
            _multiplier = uiWheel.GetWheelValue();
            // Debug.Log($"Wheel Multiplier: {_multiplier}");
            txtx2Reward.text = $"{_multiplier * data.reward.quantity}";
        }
    }
}
