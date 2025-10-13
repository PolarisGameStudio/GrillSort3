using System;
using System.Collections;
using System.Collections.Generic;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Objects.Entities;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField] ComboConfigSO comboConfigSO;
        [SerializeField] private UICombo uiCombo;
        private int combo;
        public int Combo => combo;
        private Coroutine cooldownCoroutine;
        public event Action OnComboChange;

        public void Initialize()
        {
            ResetCombo();

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnEndCollectItem += OnEndCollectItem;

            uiCombo.Init();
        }

        public void Clear()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnEndCollectItem -= OnEndCollectItem;

            uiCombo.Clear();
        }

        private void OnEndCollectItem(OrderEntity orderEntity)
        {
            AddCombo();
        }

        public void ResetCombo()
        {
            combo = 0;
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
                cooldownCoroutine = null;
            }
            OnComboChange?.Invoke();
        }

        public int GetComboTime()
        {
            if (combo >= comboConfigSO.comboTime.Count) return comboConfigSO.comboTime[^1];
            return comboConfigSO.comboTime[combo];
        }

        public void AddCombo()
        {
            combo++;
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
                cooldownCoroutine = null;
            }
            cooldownCoroutine = StartCoroutine(ComboCooldown());
            OnComboChange?.Invoke();
        }

        IEnumerator ComboCooldown()
        {
            float time = GetComboTime();
            while (time > 0)
            {
                if (GameController.Instance.GameState == GameState.Playing)
                    time -= Time.deltaTime;
                yield return null;
            }
            ResetCombo();
        }
    }
}
