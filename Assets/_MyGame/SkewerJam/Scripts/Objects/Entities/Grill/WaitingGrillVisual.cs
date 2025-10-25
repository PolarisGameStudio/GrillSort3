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

        [SerializeField] private WaitingGrillVisualConfigSO waitingGrillVisualConfig;


        public void SetActive(bool isActive)
        {
            activeObject.SetActive(isActive);
            inactiveObject.SetActive(!isActive);
        }

        public void PlayWarning()
        {
            spriteRenderer.DOKill();
            spriteRenderer.color = Color.white;
            spriteRenderer.DOColor(waitingGrillVisualConfig.warningColor, waitingGrillVisualConfig.warningDuration / 2).SetLoops(4, LoopType.Yoyo);
            MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Slot_Warning_Grill3);
        }

        public void PlayShake()
        {
            transform.parent.DOLocalMoveY(-waitingGrillVisualConfig.yDelta, waitingGrillVisualConfig.shakeDuration).SetEase(waitingGrillVisualConfig.curve).SetLoops(2, LoopType.Yoyo);
        }
    }
}