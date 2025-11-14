using System;
using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using UnityEngine;

public class PopupDifficultyLevelInGame : Panel
{
    public const string TARGET_KEY = "TARGET_KEY";
    public const string ON_COMPLETE_KEY = "ON_COMPLETE_KEY";

    [SerializeField] private float delayCanClose = 1;
    [SerializeField] private float delayForceClose = 2;
    [SerializeField] private GameObject root;

    private Transform target;
    private Action onComplete;
    private bool isClose = false;
    private bool canClose = false;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        canClose = false;
        SonatUtils.DelayCall(delayCanClose, () =>
        {
            canClose = true;
        });

        target = uiData.Get<Transform>(TARGET_KEY);
        onComplete = uiData.Get<Action>(ON_COMPLETE_KEY);

        isClose = false;
        SonatUtils.DelayCall(delayForceClose, Close);
    }

    public override void Close()
    {
        CloseAsync().Forget();
    }

    private async UniTask CloseAsync()
    {
        if (canClose == false || isClose) return;
        isClose = true;
        var effect = await MySonatFramework.poolingServiceAsync.CreateAsync<UIEffectDifficultyLevelInGame>("UIEffectDifficultyLevelInGame", root.transform.position, PanelManager.Instance.transform);
        effect.Setup(target, () =>
        {
            onComplete?.Invoke();
            MySonatFramework.audioService.PlaySound(AudioId.ButtonClick);
        });
        root.SetActive(false);
        await UniTask.Delay(200);
        base.Close();
    }
}
