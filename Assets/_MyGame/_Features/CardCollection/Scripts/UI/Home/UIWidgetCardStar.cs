using System;
using DG.Tweening;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;

namespace MyGame.Modules.CardCollection.Home
{
    public class UIWidgetCardStar : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtStar;
        [SerializeField] private Transform root;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private float durationAppear = 0.5f;
        [SerializeField] private AnimationCurve curveAppear = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float durationDisappear = 0.5f;
        [SerializeField] private AnimationCurve curveDisappear = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private ParticleSystem psParticle;

        private readonly Service<CardCollectionService> _cardCollectionService = new();

        private void OnEnable()
        {
            var starModule = _cardCollectionService.Instance.StarSubmodule;
            txtStar.text = starModule.NumberStar.ToString();

            transform.DOKill();
            transform.localPosition = root.localPosition;

            if (playOnEnable)
            {
                PlayAppearAnimation();
            }


        }

        private void OnDisable()
        {

        }

        public void PlayAppearAnimation()
        {
            transform.localPosition = root.localPosition;
            transform.DOLocalMove(Vector3.zero, durationAppear).SetEase(curveAppear);
        }

        public void PlayDisappearAnimation(Action onComplete)
        {
            transform.DOKill();
            transform.DOLocalMove(root.localPosition, durationDisappear).SetEase(curveDisappear).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void PlayParticle()
        {
            psParticle.Play();
        }
    }
}