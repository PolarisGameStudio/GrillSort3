using System.Collections;
using TMPro;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Loading
{
    [RequireComponent(typeof(TMP_Text))]
    public class UILoadingText : MonoBehaviour
    {
        [SerializeField] private float delayBetweenTexts = 0.5f;
        private string[] texts = { "Loading", "Loading.", "Loading..", "Loading..." };

        private TMP_Text textLoading;
        private Coroutine loadingCoroutine;

        private void OnEnable()
        {
            textLoading = GetComponent<TMP_Text>();
            loadingCoroutine = StartCoroutine(IELoading());
        }

        private void OnDisable()
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }

        private IEnumerator IELoading()
        {
            int index = 0;
            while (true)
            {
                yield return new WaitForSeconds(delayBetweenTexts);
                textLoading.text = texts[index];
                index++;
                if (index >= texts.Length)
                {
                    index = 0;
                }
            }
        }


    }
}
