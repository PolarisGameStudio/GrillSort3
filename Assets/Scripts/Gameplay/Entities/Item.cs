using System;
using System.Collections;
using DG.Tweening;
using Gameplay.Entities.ItemScripts;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Scripts.SO;
using PrototypeTest;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Entities
{
    public class Item : EntityBase, IPoolingObject
    {
        public override EntityType entityType => EntityType.Item;
        public ItemType itemType;
        [HideInInspector] public bool isPrimary;
        [SerializeField] protected float offsetY = 1.65f;

        [SerializeField] protected ItemVisual visual;
        [SerializeField] protected BoxCollider2D boxCollider;

        [Header("Behavior SO")]
        [SerializeField] protected ItemBehaviorSO itemBehaviorSO;

        public ItemBehaviorSO ItemBehaviorSO => itemBehaviorSO;

        protected ItemData data;
        public ItemData Data => data;
        public SlotBase Slot => slot;
        public ItemVisual Visual => visual;
        protected SlotBase slot;
        protected ItemPlaceHolder placeHolder;
        protected bool locked;
        protected ItemMatType matType;
        public bool IsLocked => locked;
        public Vector3 MouseDownPos { get => _mouseDownPos; set => _mouseDownPos = value; }
        public Vector3 Offset { get => offset; set => offset = value; }
        public bool IsSelected { get => _isSelected; set => _isSelected = value; }
        public ItemPlaceHolder PlaceHolder => placeHolder;
        public ItemMatType MatType => matType;

        private TimeBonusVisual timeBonusVisual;
        protected bool isProcessing;


        public bool Moving { get; set; }





        public virtual void SetItemData(ItemData data, SlotBase slot)
        {
            this.data = data;
            this.slot = slot;
            if (data == null)
            {
                visual.gameObject.SetActive(false);
                id = 0;
            }
            else
            {
                id = data.id;
                visual.gameObject.SetActive(true);
                visual.SetVisual(data);
                matType = CheckItemType(id);
            }
        }

        public virtual void SetSlot(SlotBase slot)
        {
            this.slot = slot;
        }

        public virtual void SetPlaceHolder(ItemPlaceHolder placeHolder)
        {
            this.placeHolder = placeHolder;
        }

        public virtual void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
        }

        public virtual void SetPrimary(bool isPrimary)
        {
            this.isPrimary = isPrimary;
            boxCollider.enabled = isPrimary;
            visual.SetPrimary(isPrimary);

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(IESpawnSmokeEffect());
            }
        }

        #region Interactive

        private bool _isSelected;
        private bool _isDragging;
        private Vector3 _mouseDownPos;
        private Vector3 offset;

        public virtual bool CanNotTouch()
        {
            return !isPrimary || _isSelected || locked || GameplayController.instance.GameState != GameState.Playing || slot.GetGrill().IsLock || isProcessing;
        }

        public virtual void OnMouseDown()
        {
            itemBehaviorSO.OnMouseDown(this);
        }

        private Vector3 vel;

        public virtual void OnMoveItem()
        {
            if (!_isSelected) return;
            if (!_isDragging)
            {
                float distance = Vector3.Distance(Input.mousePosition, _mouseDownPos);
                // Vector3 touchPos = GameplayController.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                // touchPos.z = transform.position.z;
                // float dist = Vector3.Distance(touchPos, transform.position);
                //Debug.Log(distance);
                if (distance is > 0.005f and < 200f)
                {
                    OnItemBeginDrag();
                    _isDragging = true;
                }
                else
                {
                    return;
                }
            }

            if (GameplayController.instance.GameState != GameState.Playing)
            {
                ForceFinishDrag();
                return;
            }

            // Vector3 target = GameplayController.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // target.z = 0;
            // target += offset;
            // //target += Vector3.up * offsetY;
            // //transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, 0.008f);
            // transform.position = target;
        }

        public virtual void OnUnSelectItem()
        {
            if (!_isSelected) return;
            if (_isDragging)
                OnItemFinishDrag();
        }

        // protected virtual void OnMouseDrag()
        // {
        //     
        // }

        protected virtual void OnMouseUp()
        {
            itemBehaviorSO.OnMouseUp(this);
        }

        protected virtual void OnMouseExit()
        {
            itemBehaviorSO.OnMouseExit(this);
        }

        #endregion


        public virtual void OnSelected()
        {
            itemBehaviorSO.OnSelected(this);
        }

        protected virtual void OnItemBeginDrag()
        {
            slot.OnItemDrag();
            visual.OnBeginDrag();

            placeHolder = GameFactory.CreateEntity<ItemPlaceHolder>("ItemPlaceHolder");
            placeHolder.transform.SetParent(slot.transform);
            placeHolder.transform.localPosition = Vector3.zero;
            placeHolder.SetSprite(visual.GetSprite());
        }

        protected virtual void OnItemFinishDrag()
        {
            _isDragging = false;

            if (placeHolder != null)
            {
                GameFactory.ReturnEntity(placeHolder);
                placeHolder = null;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Grill"));
            PrimaryGrill nearestGrill = null;
            float minDist = float.MaxValue;
            foreach (Collider2D collider in colliders)
            {
                PrimaryGrill grill = collider.GetComponent<PrimaryGrill>();
                if (grill != null)
                {
                    float dist = Vector3.Distance(transform.position, grill.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestGrill = grill;
                    }
                }
            }

            // if (nearestGrill != null)
            // {
            //     SlotBase availableSlot = nearestGrill.GetNearestSlot(transform.position);
            //     if (availableSlot != null && availableSlot != slot)
            //     {
            //         GameplayController.instance.SwitchSlot(availableSlot);
            //         return;
            //     }
            // }

            // GameplayController.instance.SelectItem(null, false);
            BackToSlot();
        }

        protected virtual void ForceFinishDrag()
        {
            _isSelected = false;
            _isDragging = false;
            if (placeHolder != null)
            {
                GameFactory.ReturnEntity(placeHolder);
                placeHolder = null;
            }

            // GameplayController.instance.SelectItem(null, false);
            BackToSlot();
        }

        public void OnDeSelected()
        {
            _isSelected = false;
            visual.OnDeselected();
            visual.OnIntoSlot();
        }


        protected virtual void BackToSlot()
        {
            slot.AddItem(this);
            visual.OnDeselected();
            transform.DOLocalMove(Vector3.zero, GameDefine.itemMoveBackDuration).SetSpeedBased(false).OnComplete(() =>
            {
                _isSelected = false;
                OnIntoSlot();
                PlayDropSound();
            });
        }

        protected virtual void PlayDropSound()
        {
            itemBehaviorSO.PlayDropSound(this);
        }

        public void SwitchSlot(SlotBase slot)
        {
            itemBehaviorSO.SwitchSlot(this, slot);
        }

        private bool isMoveToPrimary = false;
        public bool IsMoveToPrimary => isMoveToPrimary;
        public void MoveToPrimary(SlotBase slot, int index)
        {
            isMoveToPrimary = true;
            SetPrimary(true);
            visual.UpdateVisual();
            if (this.slot != null)
            {
                this.slot.ItemOut();
            }

            this.slot = slot;
            slot.AddItem(this);
            SetIsOnConveyor(this.slot.isOnConveyor);

            float delay = 0.1f * index;
            SonatUtils.DelayCall(delay, () =>
            {
                transform.DOKill();
                transform.DOScale(1, GameDefine.itemMovePrimaryDuration).SetEase(Ease.OutBack);
                transform.DOLocalMove(Vector3.zero, GameDefine.itemMovePrimaryDuration).SetEase(Ease.InSine).OnComplete(OnIntoSlot);
                transform.DOLocalRotate(Vector3.zero, GameDefine.itemMovePrimaryDuration).SetEase(Ease.Linear);
            }, this);
        }

        public virtual void OnDropToSlot(SlotBase slot)
        {
            OnIntoSlot();
            SonatUtils.ExecuteNextFrame(() =>
            {
                if (this.slot.GetItem() != null)
                    PlayDropSound();
            });
            isProcessing = false;
        }

        public virtual void OnIntoSlot()
        {
            isMoveToPrimary = false;
            visual.OnIntoSlot();
            slot.OnItemIntoSlot();
        }

        public virtual void SetLockState(bool lockState)
        {
            if (boxCollider)
                boxCollider.enabled = !lockState;
        }


        private static bool playSoundSizzle = false;
        private IEnumerator IESpawnSmokeEffect()
        {
            while (gameObject.activeInHierarchy)
            {
                float time = Random.Range(6f, 10f);
                yield return new WaitForSeconds(time);
                if (GameController.Instance.GameState == GameState.Playing)
                {
                    int rand = Random.Range(0, 3);
                    if (rand == 0)
                    {
                        SonatSystem.GetService<PoolingServiceAsync>().CreateAsync<EffectPoolBase>("FoodSmoke", transform.position, transform);
                        int sizzleRand = Random.Range(0, 4);

                        if (playSoundSizzle == false)
                        {
                            playSoundSizzle = true;
                            MySonatFramework.GetService<AudioService>().PlaySound((AudioId)((int)AudioId.Sizzle_01_Grill3 + sizzleRand));
                            SonatUtils.DelayCall(10f, () =>
                            {
                                playSoundSizzle = false;
                            });
                        }
                    }
                }

            }
        }

        public virtual void ForceCollect(bool destroy = true)
        {
            _isSelected = false;
            _isDragging = false;
            if (placeHolder != null)
            {
                GameFactory.ReturnEntity(placeHolder);
                placeHolder = null;
            }

            slot.ItemOut();
            slot = null;
            if (destroy)
                GameFactory.ReturnEntity(this);
        }

        public virtual void SetIsOnConveyor(bool state)
        {
            visual.SetVisibleMaskState(state);
        }

        public virtual void OnComplete()
        {
            CheckAddTimeBonus();
        }

        public virtual void SetSuggest(bool suggest)
        {
            if (visual == null) return;
            if (suggest) visual.Suggest();
            else
            {
                visual.EndSuggest();
            }
        }

        public virtual void Setup()
        {
        }

        public virtual void OnCreateObj(params object[] args)
        {
            transform.localScale = Vector3.one;
        }

        public virtual void OnReturnObj()
        {
            transform.DOKill();
            isPrimary = false;
            _isSelected = false;
            _isDragging = false;
            isProcessing = false;
            data = null;
            //visual.OnDeselected();
            transform.localScale = Vector3.one;
            locked = false;
            if (placeHolder != null)
            {
                GameFactory.ReturnEntity(placeHolder);
                placeHolder = null;
            }

            RemoveBonusVisual();
            SetSuggest(false);
        }


        public static ItemMatType CheckItemType(int id)
        {
            // if (id <= 600) return ItemMatType.Food;
            //
            // if ((id >= 401 && id <= 403) || (id >= 407 && id <= 411) || (id >= 465 && id <= 466) || (id >= 479 && id <= 481))
            // {
            //     return ItemMatType.FruitGlass;
            // }
            //
            // return ItemMatType.FruitNormal;
            return ItemMatType.Food;
        }

        public enum ItemMatType : byte
        {
            Food = 0,
            FruitNormal,
            FruitGlass,
            FruitPlastic
        }

        public void Highlight()
        {
            visual.Highlight();
        }

        public void UnHighlight()
        {
            visual.UnHighlight();
        }

        public virtual ItemData GetCurrentItemData()
        {
            return Data.Clone();
        }

        public void ForceOutSlot()
        {
            this.slot = null;
            if (placeHolder != null)
            {
                GameFactory.ReturnEntity(placeHolder);
                placeHolder = null;
            }
        }

        public void Deselect()
        {
            visual.OnDeselected();
        }

        public void AddBonusVisual()
        {
            timeBonusVisual = GameFactory.CreateEntity<TimeBonusVisual>("TimeBonusVisual", visual.transform);
            timeBonusVisual.transform.localPosition = Vector3.zero;
            timeBonusVisual.transform.localScale = Vector3.one;
            timeBonusVisual.SetItem(this);
        }

        public void RemoveBonusVisual()
        {
            if (timeBonusVisual == null) return;
            GameFactory.ReturnEntity(timeBonusVisual);
            timeBonusVisual = null;
        }

        private void CheckAddTimeBonus()
        {
            if (timeBonusVisual == null) return;
            timeBonusVisual.OnBonusTime();
        }

        public virtual bool CanCollect()
        {
            return !IsLocked;
        }

        public void SetIsProcessing(bool isProcessing)
        {
            this.isProcessing = isProcessing;
        }

        public virtual void MoveToOrder()
        {

        }
    }
}