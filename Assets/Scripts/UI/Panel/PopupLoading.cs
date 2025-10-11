using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : Panel
{
    private float time = 1;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text txtLoading;
    [SerializeField] private float delayBetweenTexts = 0.5f;
    [SerializeField] private string[] loadingTexts = { "Loading...", "Loading..", "Loading." };

    private Coroutine loadingCoroutine;
    public override void Open(UIData uiData)
    {
        base.Open(uiData);
        if (uiData != null && uiData.TryGet("Time", out time))
        {
            SonatUtils.DelayCall(time, Close);
        }
        slider.value = 0;

        loadingCoroutine = StartCoroutine(IELoading());
    }

    public override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        slider.DOValue(1, time);
    }

    private IEnumerator IELoading()
    {
        var index = 0;
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenTexts);
            txtLoading.text = loadingTexts[index];
            index++;
            if (index >= loadingTexts.Length)
            {
                index = 0;
            }
        }
    }

    public override void Close()
    {
        base.Close();
        StopCoroutine(loadingCoroutine);
    }
}
