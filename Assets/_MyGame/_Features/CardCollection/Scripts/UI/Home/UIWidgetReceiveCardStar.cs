using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

namespace MyGame.Modules.CardCollection.Home
{
    public class UIWidgetReceiveCardStar : UIWidgetCardStarBase
    {
        [Space]
        [Header("Update view ")]
        [SerializeField] private bool useParticle = false;
        [SerializeField, ShowIf("useParticle")] private Transform containerPs;

        private readonly Service<CardCollectionService> _cardCollectionService = new();
        private readonly Service<PoolingContainerService> _poolingContainerService = new();


        private bool scaleUp = false;
        private Coroutine collectAnim = null;
        private float scaleSpeed = 3f;
        private float scaleMax = 1.15f;
        private int value = 0;
        private Tween tween = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            var starModule = _cardCollectionService.Instance.StarSubmodule;
            value = starModule.NumberStarView;
            txtStar.text = value.ToString(); // update view
        }

        public void UpdateValueView(int addedStar)
        {
            var oldValue = value;
            value += addedStar;
            PlayParticle().Forget();
            tween?.Kill();
            tween = DOTween.To(() => oldValue, x => value = x, value, 0.1f).OnUpdate(() =>
                {
                    txtStar.text = SonatUtils.FormatNumber(value);
                });
        }

        public async UniTask PlayParticle()
        {
            if (useParticle == false) return;

            var ps = _poolingContainerService.Instance.CreateObject<Transform>(containerPs);
            ps.GetComponent<ParticleSystem>().Play();

            if (collectAnim != null)
            {
                scaleUp = true;
            }
            else
            {
                collectAnim = StartCoroutine(CollectEffect());
            }

            await UniTask.Delay(1000);
            ps.gameObject.SetActive(false);
        }

        IEnumerator CollectEffect()
        {
            scaleUp = true;
            while (transform.localScale.x < scaleMax)
            {
                transform.localScale += Vector3.one * Time.deltaTime * scaleSpeed;
                yield return null;
            }

            //SettingManager.Vibrations(100);
            yield return null;
            scaleUp = false;

            while (transform.localScale.x > 1)
            {
                if (scaleUp)
                {
                    collectAnim = StartCoroutine(CollectEffect());
                    yield break;
                }

                transform.localScale -= Vector3.one * Time.deltaTime * scaleSpeed;
                yield return null;
            }

            transform.localScale = Vector3.one;
            collectAnim = null;
        }
    }
}