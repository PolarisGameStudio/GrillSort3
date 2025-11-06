using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using TMPro;
using UnityEngine;

namespace MyGame.Modules.CardCollection.Home
{
    public class UIWidgetCardStarBase : MonoBehaviour
    {
        [SerializeField] protected TMP_Text txtStar;
        [SerializeField] private Transform root;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private bool registerEvent = true;
        [Space]
        [Header("Animation")]
        [SerializeField] private float delayAppear = 0f;
        [SerializeField] private float durationAppear = 0.5f;
        [SerializeField] private AnimationCurve curveAppear = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float durationDisappear = 0.5f;
        [SerializeField] private AnimationCurve curveDisappear = AnimationCurve.Linear(0, 0, 1, 1);

        private readonly Service<CardCollectionService> _cardCollectionService = new();

        protected virtual void OnEnable()
        {
            var starModule = _cardCollectionService.Instance.StarSubmodule;
            txtStar.text = starModule.NumberStar.ToString();

            transform.DOKill();
            transform.localPosition = root.localPosition;

            if (playOnEnable)
            {
                PlayAppearAnimation(delayAppear);
            }

            if (registerEvent)
            {
                var starSubmodule = _cardCollectionService.Instance.StarSubmodule;
                starSubmodule.OnStarChanged += OnStarChanged;
            }
        }

        private void OnDisable()
        {
            if (registerEvent)
            {
                var starSubmodule = _cardCollectionService.Instance.StarSubmodule;
                starSubmodule.OnStarChanged -= OnStarChanged;
            }
        }

        private void OnStarChanged(int value)
        {
            txtStar.text = value.ToString();
        }

        public void PlayAppearAnimation(float delay = 0)
        {
            transform.localPosition = root.localPosition;
            transform.DOLocalMove(Vector3.zero, durationAppear).SetEase(curveAppear).SetDelay(delay);
        }

        public void PlayDisappearAnimation(Action onComplete)
        {
            transform.DOKill();
            transform.DOLocalMove(root.localPosition, durationDisappear).SetEase(curveDisappear).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}