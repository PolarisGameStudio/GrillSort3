using DG.Tweening;
using Sonat.Enums;
using SonatFramework.Systems.AudioManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Objects.Entities
{
    public class WaitingGrillVisual : MonoBehaviour
    {
        [SerializeField] private GameObject activeObject;
        [SerializeField] private GameObject inactiveObject;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Warning")]
        [SerializeField] private float warningDuration = 0.5f;
        [SerializeField] private Color warningColor = new Color(0.7f, 0.0f, 0.0f, 1.0f);
        public void SetActive(bool isActive)
        {
            activeObject.SetActive(isActive);
            inactiveObject.SetActive(!isActive);
        }

        public void PlayWarning()
        {
            spriteRenderer.DOKill();
            spriteRenderer.color = Color.white;
            spriteRenderer.DOColor(warningColor, warningDuration / 2).SetLoops(4, LoopType.Yoyo);
            MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Slot_Warning_Grill3);
        }
    }
}