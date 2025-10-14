using System.Collections;
using SonatFramework.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text txtLoading;
    [SerializeField] private float delayBetweenTexts = 0.5f;
    [SerializeField] private string[] loadingTexts = { "Loading...", "Loading..", "Loading." };

    private float time = 1;

    private Coroutine loadingCoroutine;

    private void OnEnable()
    {
        // slider.value = 0;
        // loadingCoroutine = StartCoroutine(IELoading());
    }

    private void OnDisable()
    {
        StopCoroutine(loadingCoroutine);
        loadingCoroutine = null;
    }

    public void SetTime(float time)
    {
        SonatUtils.DelayCall(time, () =>
        {
            gameObject.SetActive(false);
        }, this);
        slider.value = 0;

        loadingCoroutine = StartCoroutine(IELoading());
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
}
