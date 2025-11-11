using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.SkewerJam.Gameplay
{
    public class UICombo : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private LocalizationParamsManager comboParamsManager;
        [SerializeField] private GameObject mainObject;
        [SerializeField] private ParticleSystem blastEffect;
        [SerializeField] private LayoutElement layoutElement;

        private ComboManager comboManager => GameController.Instance.ComboManager;
        private int combo;
        private Coroutine cooldownCoroutine;

        public void Init()
        {
            comboManager.OnComboChange += OnComboUpdate;
            OnComboUpdate();
        }

        public void Clear()
        {
            comboManager.OnComboChange -= OnComboUpdate;
        }

        private void OnComboUpdate()
        {
            combo = comboManager.Combo;
            if (combo < 1)
            {
                if (mainObject != null)
                {
                    mainObject.SetActive(false);
                }
                if (layoutElement != null)
                {
                    layoutElement.ignoreLayout = true;
                }
                if (cooldownCoroutine != null)
                {
                    StopCoroutine(cooldownCoroutine);
                    cooldownCoroutine = null;
                }
            }
            else
            {
                if (mainObject != null)
                {
                    mainObject.SetActive(true);
                }
                if (layoutElement != null)
                {
                    layoutElement.ignoreLayout = false;
                }
                if (blastEffect != null)
                {
                    blastEffect.gameObject.SetActive(true);
                    blastEffect.Play();
                }
                comboParamsManager.SetParameterValue("VALUE", combo.ToString());
                if (cooldownCoroutine != null)
                {
                    StopCoroutine(cooldownCoroutine);
                }

                cooldownCoroutine = StartCoroutine(ComboCooldown());
            }
        }

        IEnumerator ComboCooldown()
        {
            float maxTime = comboManager.GetComboTime();
            float time = maxTime;
            while (time > 0)
            {
                if (GameController.Instance.GameState == GameState.Playing)
                {
                    time -= Time.deltaTime;
                    slider.value = time / maxTime;
                }

                yield return null;
            }
        }
    }
}