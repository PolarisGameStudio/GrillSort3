using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Utils;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.SettingsManagement.Vibation;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyGame.SkewerJam.Objects.Entities
{
    public class OrderEntityVisual : MonoBehaviour
    {
        [SerializeField] private GameObject normalOrder;
        [SerializeField] private GameObject bonusOrder;
        [SerializeField] private SpriteRenderer bonusLid;

        [SerializeField] private SpriteRenderer imageLid;
        [SerializeField] private SortingGroup sortingGroup;

        [SerializeField] private OrderEntityConfigSO orderEntityConfigSO;

        [Header("Animation & Effect")]
        [SerializeField] private ParticleSystem completeEffect;
        private Vector3 _originalPosition;

        [SerializeField] private OrderEntityVisualConfigSO orderEntityVisualConfigSO;

        private void Start()
        {
            _originalPosition = bonusLid.transform.localPosition;
        }

        public void ResetLid()
        {
            bonusLid.transform.DOKill();
            bonusLid.transform.localPosition = _originalPosition;
            bonusLid.gameObject.SetActive(true);
            bonusLid.transform.localScale = Vector3.one;
            bonusLid.SetAlpha(1);

            imageLid.transform.DOKill();
            imageLid.transform.localPosition = _originalPosition;
            imageLid.gameObject.SetActive(true);
            imageLid.transform.localScale = Vector3.one;
            imageLid.SetAlpha(1);
        }

        public void SetActive(bool isActive)
        {
            ResetLid();
            imageLid.gameObject.SetActive(isActive);
            bonusLid.gameObject.SetActive(!isActive);

            normalOrder.gameObject.SetActive(isActive);
            bonusOrder.gameObject.SetActive(!isActive);

            //init
            completeEffect.Stop();
            SetSortingGroup(false);
        }


        public virtual void OpenGrill(bool isNormal = true, bool doEffect = true)
        {
            var lid = isNormal ? imageLid : bonusLid;
            var ortherLid = isNormal ? bonusLid : imageLid;
            ortherLid.transform.DOKill();
            ortherLid.gameObject.SetActive(false);

            lid.transform.DOKill();

            if (doEffect)
            {
                lid.transform.localScale = Vector3.one;
                lid.gameObject.SetActive(true);
                lid.transform.DOLocalMoveY(2.5f, GameDefine.grillLidAnim).From(0.035f).SetEase(Ease.OutQuad);
                // lid.transform.DOScale(0.95f, GameDefine.grillLidAnim);
                lid.DOFade(0, GameDefine.grillLidAnim).SetEase(Ease.InQuad).OnComplete(() => { lid.gameObject.SetActive(false); });
            }
            else
            {
                lid.gameObject.SetActive(false);
            }
        }

        public async UniTask PlayComplete(Action onComplete)
        {
            imageLid.gameObject.SetActive(true);
            imageLid.transform.localPosition = Vector3.up * orderEntityConfigSO.up;
            imageLid.transform.localScale = Vector3.one;
            imageLid.SetAlpha(1);

            await imageLid.transform.DOLocalMove(Vector3.zero, orderEntityConfigSO.durationUp).SetEase(orderEntityConfigSO.downCurve);
            completeEffect.Play();

            // PlayEntityShake().Forget();

            MySonatFramework.audioService.PlaySound(AudioId.Box_Close_Grill3);
            onComplete?.Invoke();

            MySonatFramework.GetService<VibrationService>().Vibrate(100);
            await UniTask.Delay((int)(orderEntityConfigSO.delayMoveOut * 1000));

            // sound
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            if (orderManager.ListOrders.Count < orderManager.ListOrderData.Count)
            {
                MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Box_Appear_Grill3);
            }

            var orderEntity = transform.parent.GetComponent<OrderEntity>();
            var targetPos = orderEntity.transform.localPosition + Vector3.up * orderEntityConfigSO.up;
            await orderEntity.transform.DOLocalMove(targetPos, orderEntityConfigSO.durationDown).SetEase(orderEntityConfigSO.upCurve);
            MyGame.SkewerJam.Gameplay.GameFactory.Instance.ReturnEntity(orderEntity);


        }

        public async UniTask PlayEntityShake()
        {
            var orderEntity = transform.parent;
            await orderEntity.DOScale(orderEntityVisualConfigSO.scaleValue, orderEntityVisualConfigSO.scaleDuration);

            var scaleValue2 = new Vector2(orderEntityVisualConfigSO.scaleValue.y, orderEntityVisualConfigSO.scaleValue.x);
            await orderEntity.DOScale(scaleValue2, orderEntityVisualConfigSO.scaleDuration * 2);

            await orderEntity.DOScale(Vector3.one, orderEntityVisualConfigSO.scaleDuration);
        }

        public void SetSortingGroup(bool enabled, string sortingLayerName = LayerManager.Object, int sortingOrder = 1)
        {
            sortingGroup.enabled = enabled;
            sortingGroup.sortingLayerName = sortingLayerName;
            sortingGroup.sortingOrder = sortingOrder;
        }
    }
}
