using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule.SpriteService;
using SonatFramework.Scripts.UIModule.UIElements;
using UnityEditor;
using UnityEngine;

public class BoosterAnim : EffectPoolBase
{
    [SerializeField] protected FixedImageRatio icon;
    [SerializeField] protected AnimationCurve moveXCurve;
    [SerializeField] protected AnimationCurve moveYCurve;
    [SerializeField] protected float duration = 0.5f;

    [Header("Scale")]
    [SerializeField] protected float scale = 0.3f;
    [SerializeField] protected float scaleDuration = 0.5f;
    [SerializeField] protected AnimationCurve scaleCurve;

    [Header("Shake")]
    [SerializeField, ShowIf("isShake")] protected bool isShake = true;
    [SerializeField, ShowIf("isShake")] protected float shakeDelay = 0.75f;
    [SerializeField, ShowIf("isShake")] protected float shakeDuration = 0.5f;
    [SerializeField, ShowIf("isShake")] protected float shakeStrength = 0.5f;
    [SerializeField, ShowIf("isShake")] protected int shakeVibrato = 5;
    [SerializeField, ShowIf("isShake")] protected float shakeRandomness = 90f;

    public void SetBooster(GameResource boosterType)
    {
        icon.sprite = MySonatFramework.GetService<SpriteAtlasService>().GetSprite($"ico_{boosterType}");
    }
    public virtual void SetData(Vector3 position)
    {
        transform.position = position;
        transform.DOLocalMoveX(0, duration).SetEase(moveXCurve);
        transform.DOLocalMoveY(0, duration).SetEase(moveYCurve);

        transform.localScale = Vector3.one * scale;
        transform.DOScale(Vector3.one, scaleDuration).SetEase(scaleCurve);

        if (isShake)
        {
            transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true, ShakeRandomnessMode.Full)
            .SetEase(Ease.OutBounce)
            .SetDelay(shakeDelay);
        }
    }
}