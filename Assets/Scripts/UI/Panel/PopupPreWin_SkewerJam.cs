using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using Spine;
using Spine.Unity;
using UnityEngine;

public class PopupPreWin_SkewerJam : PopupPreWin
{
    [SerializeField] private SkeletonGraphic animationBack;
    [SerializeField] private SkeletonGraphic animationFront;
    [SerializeField] private ParticleSystem psSpawn;

    [SerializeField] private float delayPlay;

    [Header("Animation")]
    [SerializeField] private float durationAppear = 1.167f;
    [SerializeField] private float durationDrop = 2f;
    [SerializeField] private float durationCollectDone = 2f;
    [SerializeField] private float delayOut = 1f;
    [SerializeField] private float delayPsDrop = 0.5f;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        // var num = GameController.Instance.GameLogicHandler.Pumpkin;

        // MySonatFramework.audioService.PlaySound(AudioId.Pre_win_HLW_sound_Grill_sort);
        Play();
    }

    private async UniTask Play()
    {
        await UniTask.Delay((int)(delayPlay * 1000));
        // appear
        // Drop
        // Collect_Done
        // Drop_Pumpkin
        // Out
        SonatUtils.DelayCall(delayPsDrop, () =>
        {
            psSpawn.gameObject.SetActive(true);
            psSpawn.Play();
        });
        
        animationBack.AnimationState.SetAnimation(0, "Appear", false);
        animationFront.AnimationState.SetAnimation(0, "Appear", false);
        await UniTask.Delay((int)(durationAppear * 1000));
        animationBack.AnimationState.SetAnimation(0, "Drop", true);
        animationFront.AnimationState.SetAnimation(0, "Drop", true);

        await UniTask.Delay((int)(durationDrop * 1000));
        animationBack.AnimationState.SetAnimation(0, "Collect_Done2", false);
        animationFront.AnimationState.SetAnimation(0, "Collect_Done2", false);
        await UniTask.Delay((int)(durationCollectDone * 1000));
        // animation.AnimationState.SetAnimation(0, "Drop_Pumkin", true);
        psSpawn.Stop();
        // animation.AnimationState.ClearTracks();
        await UniTask.Delay((int)(delayOut * 1000));
        animationBack.AnimationState.SetAnimation(0, "Out", false);
        animationFront.AnimationState.SetAnimation(0, "Out", false);
        // await UniTask.Delay((int)(durationDropPumpkin * 1000));


    }
}
