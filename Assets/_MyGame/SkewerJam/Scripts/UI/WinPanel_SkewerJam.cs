using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

public class WinPanel_SkewerJam : WinPanelBase
{
    [SerializeField] private float delaySoundFireworks;
    [SerializeField] private float delaySoundParticle;
    [SerializeField] private float delaySoundReceived;

    [Header("Update Score")]
    [SerializeField] private float delayUpdateScore;
    // [SerializeField] private LeaderboardScrollView leaderboardScrollView;
    // [SerializeField] private LeaderboardItemView leaderboardItemView;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        // SonatUtils.DelayCall(delaySoundFireworks, () =>
        // {
        //     MySonatFramework.audioService.PlaySound(AudioId.Win_HLW_Music_fireworks_Grill_sort);
        // }, this);
        // SonatUtils.DelayCall(delaySoundParticle, () =>
        // {
        //     MySonatFramework.audioService.PlayMusic(AudioId.Win_HLW_Particle_loop_Grill_sort, true, 0.1f);
        // }, this);
    }

    public override void OnClaimClick()
    {
        base.OnClaimClick();

        SonatUtils.DelayCall(delayUpdateScore, () =>
        {
            // var pumpkin = MySonatFramework.GetService<InventoryService>().GetResource(Sonat.Enums.GameResource.Pumpkin);
            // var currentRank = MySonatFramework.GetService<LeaderboardHLWService>().CurrentRank;
            // var obj = leaderboardScrollView.GetObjectByIndex(currentRank - 1);
            // if (obj != null && obj.TryGetComponent<LeaderboardItemView>(out var view))
            // {
            //     view.UpdateScore(pumpkin);
            // }
            // leaderboardItemView.UpdateScore(pumpkin);
        }, this);
        MySonatFramework.audioService.StopMusic();
        // SonatUtils.DelayCall(delaySoundReceived, () =>
        // {
        //     MySonatFramework.audioService.PlaySound(AudioId.Win_HLW_Pumkin_Received_Grill_sort);
        // }, this);
    }
}
