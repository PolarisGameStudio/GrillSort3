using DG.Tweening;
using Gameplay.LevelData;
using MyGame.SkewerJam.Utils;
using SonatFramework.Scripts.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Entities.GrillScripts
{
    public class GrillVisual : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer stove;
        [SerializeField] protected SpriteRenderer lid;
        [SerializeField] protected SpriteRenderer lidSoldOut;
        [SerializeField] protected SortingGroup sortingGroup;
        [SerializeField] protected string lidType;
        [SerializeField] protected string stoveType;

        protected float defaultLidPos = 0.225f;

        protected virtual void Awake()
        {
            defaultLidPos = lid.transform.localPosition.y;
            if (sortingGroup != null) sortingGroup.enabled = false;
        }

        public virtual void SetDefaultGrill(GrillData grillData)
        {
            SetVisual();
            lid.gameObject.SetActive(true);
            lid.transform.localPosition = new Vector3(0, defaultLidPos, 0);
            // lid.transform.localScale = Vector3.one;
            lid.SetAlpha(1);

            UnHighlightGrill();
            lidSoldOut.gameObject.SetActive(false);
        }


        protected virtual void SetVisual()
        {
            var grillBase = GetComponentInParent<GrillBase>();
            var grillVisualSO = grillBase.GrillBaseBehaviorSO.grillVisualSO;
            grillVisualSO.SetGrillBaseVisual(this, stove, lid, stoveType, lidType);
        }

        public virtual void OpenGrill(bool doEffect = true, bool isSoldOut = false)
        {
            var selectedLid = isSoldOut ? lidSoldOut : lid;
            var otherLid = isSoldOut ? lid : lidSoldOut;

            otherLid.transform.DOKill();
            otherLid.gameObject.SetActive(false);

            selectedLid.transform.DOKill();
            selectedLid.gameObject.SetActive(true);
            if (doEffect)
            {
                // lid.transform.localScale = Vector3.one;
                selectedLid.gameObject.SetActive(true);
                selectedLid.transform.DOLocalMoveY(2.5f, GameDefine.grillLidAnim).From(defaultLidPos).SetEase(Ease.OutQuad);
                // lid.transform.DOScale(0.95f, GameDefine.grillLidAnim);
                selectedLid.DOFade(0, GameDefine.grillLidAnim).SetEase(Ease.InQuad).OnComplete(() => { selectedLid.gameObject.SetActive(false); });
            }
            else
            {
                selectedLid.gameObject.SetActive(false);
            }
        }

        public virtual void CloseGrill(bool doEffect = true, bool isSoldOut = false)
        {
            var selectedLid = isSoldOut ? lidSoldOut : lid;
            var otherLid = isSoldOut ? lid : lidSoldOut;
            otherLid.transform.DOKill();
            otherLid.gameObject.SetActive(false);

            selectedLid.transform.DOKill();
            selectedLid.gameObject.SetActive(true);
            if (doEffect)
            {
                // selectedLid.transform.localScale = Vector3.one * 0.95f;
                selectedLid.transform.localPosition = new Vector3(0, 2.5f, 0);
                selectedLid.DOFade(0, 0);
                // selectedLid.transform.DOScale(1, GameDefine.grillLidAnim);
                selectedLid.transform.DOLocalMoveY(defaultLidPos, GameDefine.grillLidAnim).SetEase(Ease.InQuad);
                selectedLid.DOFade(1, GameDefine.grillLidAnim).SetEase(Ease.OutQuad);
            }
            else
            {
                // selectedLid.transform.localScale = Vector3.one;
                selectedLid.transform.SetLocalPositionY(defaultLidPos);
                selectedLid.DOFade(1, 0);
            }
        }

        private bool isConveyorState = false;

        public virtual void SetMaskVisible(bool state)
        {
            switch (isConveyorState)
            {
                case false when state:
                    isConveyorState = true;
                    stove.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    lid.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    break;
                case true:
                    isConveyorState = false;
                    stove.maskInteraction = SpriteMaskInteraction.None;
                    lid.maskInteraction = SpriteMaskInteraction.None;
                    break;
            }
        }

        public virtual void UpdateSubGrill()
        {
        }

        public virtual void OnReturnGrill()
        {
        }

        public void HighlightGrill()
        {
            // stove.material = GameResourceReference.Instance.grillHighlightMaterial;
            if (sortingGroup != null)
            {
                sortingGroup.enabled = true;
                sortingGroup.sortingLayerName = LayerManager.TopUI;
            }
        }

        public void UnHighlightGrill()
        {
            // stove.material = GameResourceReference.Instance.itemMaterials[0];
            if (sortingGroup != null)
            {
                sortingGroup.enabled = false;
                sortingGroup.sortingLayerName = LayerManager.Object;
            }
        }

        public Bounds GetGrillBounds()
        {
            return stove != null ? stove.bounds : new Bounds(transform.position, Vector3.one);
        }
    }
}