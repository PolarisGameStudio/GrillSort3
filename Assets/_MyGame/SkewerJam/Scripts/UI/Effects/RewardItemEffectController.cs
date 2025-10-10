using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class RewardItemEffectController : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem psHide;

    public void SetAlpha(float alpha)
    {
        GetComponent<CanvasGroup>().alpha = alpha;
    }

    public void PlayFade(float duration)
    {
        GetComponent<CanvasGroup>().DOFade(1, duration);
    }

    public void PlayPS()
    {
        ps.gameObject.SetActive(true);
        ps.Play();
    }

    public void PlayPsHide(){
        psHide.gameObject.SetActive(true);
        psHide.Play();
    }
}
