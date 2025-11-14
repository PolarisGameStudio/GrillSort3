using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyGame.Modules.SubInventory.UI.Elements
{
    public class UICurrencySubItem : MonoBehaviour
    {
        [SerializeField] private SubGameResource resource;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text txtValue;
        [SerializeField] private ParticleSystem blastEffect;

        [SerializeField] private bool useForceEffect = false;
        [SerializeField, ShowIf("useForceEffect")] private UnityEvent OnForceEffectFinished;

        [Header("Anim")]
        public float counterDuration = 0.5f;
        public float scaleSpeed = 3f;
        public float scaleMax = 1.15f;

        [Header("UI Miss value")]
        [SerializeField] private bool forceValue = false;
        [SerializeField, ShowIf("forceValue")] private bool isMiss = false;

        protected int value = -1;
        private bool scaleUp;
        private Coroutine collectAnim = null;

        private readonly Service<SubInventoryService> subInventoryService = new();

        private EventBinding<AddSubItemEvent> addSubItemEvent;
        private EventBinding<ReduceSubItemEvent> reduceSubItemEvent;
        private EventBinding<ForceEffectSubItemEvent> forceEffectSubItemEvent;
        private bool blockCollectEffect = false;

        protected virtual void Start()
        {
            if (blastEffect)
                blastEffect.gameObject.SetActive(false);
        }

        public virtual void OnEnable()
        {
            UpdateValueView(false);
            addSubItemEvent = new EventBinding<AddSubItemEvent>(OnAddSubItem);
            reduceSubItemEvent = new EventBinding<ReduceSubItemEvent>(OnReduceSubItem);

            if (useForceEffect)
            {
                forceEffectSubItemEvent = new EventBinding<ForceEffectSubItemEvent>(OnForceEffectSubItem);
            }

            if (forceValue)
            {
                value = subInventoryService.Instance.GetResourceView(this.resource) * (isMiss ? -1 : 1);
                txtValue.text = value.ToString();
            }
        }

        protected virtual void OnDisable()
        {
            //inventoryService.Instance.OnAddCurrency -= OnAddCurrency;
            EventBus<AddSubItemEvent>.Deregister(addSubItemEvent);
            EventBus<ReduceSubItemEvent>.Deregister(reduceSubItemEvent);
            addSubItemEvent = null;
            reduceSubItemEvent = null;
            if (useForceEffect)
            {
                EventBus<ForceEffectSubItemEvent>.Deregister(forceEffectSubItemEvent);
                forceEffectSubItemEvent = null;
            }
            txtValue.DOKill();
        }


        protected virtual void OnAddSubItem(AddSubItemEvent eventData)
        {
            if (eventData.resource != this.resource && eventData.resource != SubGameResource.MAX) return;
            //UpdateValueView(true);
            if (blockCollectEffect == false && eventData.collectEffect != null && icon != null)
            {
                eventData.collectEffect.Collect(
                    this.resource,
                    eventData.quantity,
                    eventData.position,
                    icon.transform.position,
                    OnCollectEffectFinished,
                    PlayCollectEffect
                );
            }
            else
            {
                UpdateValueView();
            }
        }

        private void OnCollectEffectFinished()
        {
            try
            {
                UpdateValueView();
                // PlayCollectEffect();
                // if (blastEffect)
                // {
                //     blastEffect.gameObject.SetActive(true);
                //     blastEffect.Play();
                // }
            }
            catch (Exception e)
            {

            }
        }

        protected virtual void OnReduceSubItem(ReduceSubItemEvent eventData)
        {
            if (eventData.resource != this.resource && eventData.resource != SubGameResource.MAX) return;
            SonatUtils.DelayCall(0.1f, () => UpdateValueView(false), this);
        }

        public virtual void UpdateValueView(bool doCounter = true)
        {
            int oldvalue = this.value;
            value = subInventoryService.Instance.GetResourceView(this.resource);

            if (value == oldvalue) return;
            if (gameObject.activeInHierarchy && doCounter)
                // txtValue.DOCounter(oldvalue, value, counterDuration, addThousandsSeparator: false).OnUpdate(() =>
                // {
                //     txtValue.text = SonatUtils.FormatNumber(value);
                // });
                DOTween.To(() => oldvalue, x => value = x, value, counterDuration).OnUpdate(() =>
                {
                    txtValue.text = SonatUtils.FormatNumber(value);
                });
            else
                txtValue.text = SonatUtils.FormatNumber(value);
        }

        public void PlayCollectEffect()
        {
            if (!gameObject.activeInHierarchy) return;
            if (collectAnim != null)
            {
                //StopCoroutine(collectAnim);
                scaleUp = true;
                //blastEffect?.Play();
            }
            else
            {
                collectAnim = StartCoroutine(CollectEffect());
                //blastEffect?.gameObject.SetActive(true);
            }

            if (blastEffect)
            {
                // var eff = Instantiate(blastEffect.gameObject, icon.transform);
                // eff.gameObject.SetActive(true);
                // Destroy(eff, 1.2f);
                blastEffect.gameObject.SetActive(true);
                blastEffect.Play();
            }
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

        private void OnForceEffectSubItem(ForceEffectSubItemEvent eventData)
        {
            if (eventData.resource != this.resource && eventData.resource != SubGameResource.MAX) return;
            //UpdateValueView(true);
            if (blockCollectEffect == false && eventData.collectEffect != null && icon != null)
            {
                eventData.collectEffect.Collect(
                    this.resource,
                    eventData.quantity,
                    eventData.position,
                    icon.transform.position,
                    () =>
                    {
                        OnForceEffectFinished?.Invoke();
                    },
                    PlayCollectEffect
                );
            }
            else
            {
                UpdateValueView();
            }
        }
    }
}