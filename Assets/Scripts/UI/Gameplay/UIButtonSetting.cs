using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIButtonSetting : MonoBehaviour
{
    [SerializeField] private PopupSettings popupSettings;
    [SerializeField] private List<RectTransform> buttonTrans;
    [SerializeField] private Image background;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool isActive = false;


    [SerializeField] private float offsetX;
    [SerializeField] private float goInDuration = 0.3f;
    [SerializeField] private float goOutDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float delayCount = 0.1f;
    [SerializeField] private AnimationCurve goInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve goOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private List<float> originalPositions = new List<float>();
    private List<Tween> activeTweens = new List<Tween>();
    private void Awake()
    {
        foreach (var button in buttonTrans)
        {
            float originalX = button.anchoredPosition.x;
            originalPositions.Add(originalX);
            button.anchoredPosition = new Vector2(originalX + offsetX, button.anchoredPosition.y);
        }
        GoOutImmediate();
    }
    private void OnEnable()
    {
        GoOutImmediate();
    }

    public void GoIn()
    {
        if (isActive) return;

        if (GameController.Instance.GameState != GameState.Playing) return;

        KillAllTweens();
        GameplayHelper.OnClose_ChangeGameState(GameState.Paused);

        isActive = true;
        background.gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        var canvasGroupTween = canvasGroup.DOFade(1f, fadeDuration).SetEase(goInCurve);
        activeTweens.Add(canvasGroupTween);

        for (int i = 0; i < buttonTrans.Count; i++)
        {
            var button = buttonTrans[i];
            var index = i;

            button.gameObject.SetActive(true);
            button.anchoredPosition = new Vector2(originalPositions[i] + offsetX, button.anchoredPosition.y);

            var delayTween = DOVirtual.DelayedCall(i * delayCount, () =>
            {
                var tween = button.DOAnchorPosX(originalPositions[index], goInDuration)
                    .SetEase(goInCurve);
                activeTweens.Add(tween);
            });

            activeTweens.Add(delayTween);
        }

    }

    public void OnActive()
    {
        if (isActive)
        {
            GoOut();
        }
        else
        {
            GoIn();
        }
    }
    public void HomeClick()
    {
        GoOut();
    }


    public void GoOut(bool isChangeGameState = true)
    {
        Debug.Log("before go out");
        if (!isActive) return;
        Debug.Log("after go out");

        popupSettings.ResetClick();
        if (isChangeGameState)
        {
            GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
        }
        KillAllTweens();

        int completedCount = 0;

        var canvasGroupTween = canvasGroup.DOFade(0f, fadeDuration)
            .SetEase(goOutCurve);
        activeTweens.Add(canvasGroupTween);

        for (int i = buttonTrans.Count - 1; i >= 0; i--)
        {
            var button = buttonTrans[i];
            var index = i;
            var delayIndex = buttonTrans.Count - 1 - i;

            var delayTween = DOVirtual.DelayedCall(delayIndex * (delayCount - 0.03f), () =>
            {
                var tween = button.DOAnchorPosX(originalPositions[index] + offsetX, goOutDuration)
                    .SetEase(goOutCurve)
                    .OnComplete(() =>
                    {
                        completedCount++;
                        if (completedCount == buttonTrans.Count)
                        {
                            OnCompleted();
                        }
                    });
                activeTweens.Add(tween);
            });

            activeTweens.Add(delayTween);
        }
    }
    public void GoOutImmediate()
    {
        KillAllTweens();
        for (int i = 0; i < buttonTrans.Count; i++)
        {
            var button = buttonTrans[i];
            button.anchoredPosition = new Vector2(originalPositions[i] + offsetX, button.anchoredPosition.y);
        }
        OnCompleted();
    }
    private void OnCompleted()
    {
        isActive = false;
        background.gameObject.SetActive(false);
        foreach (var button in buttonTrans)
        {
            button.gameObject.SetActive(false);
        }
    }
    private void KillAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }
        activeTweens.Clear();

        foreach (var button in buttonTrans)
        {
            button.DOKill();
        }

        // Kill CanvasGroup tween
        canvasGroup.DOKill();
    }
    private void OnDisable()
    {
        KillAllTweens();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }
}
