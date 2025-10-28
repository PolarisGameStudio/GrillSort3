using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay;
using MyGame.SkewerJam.Objects.Entities;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using MyGame.SkewerJam.Level;
using MyGame.SkewerJamSO.Boosters;
using MyGame.SkewerJam.Gameplay.Helpers;

namespace MyGame.SkewerJam.Objects
{
    public class WaitingGrillManager : MonoBehaviour
    {
        private const int WAITING_GRILL_ID_OFFSET = 1000;

        [SerializeField] private Transform centerRefPoint;
        [SerializeField] private float distance = 1.2f;
        [SerializeField] private BoosterAddPlateBehaviorSO boosterAddPlateBehaviorSO;

        private List<WaitingGrill> listWaitingGrills = new List<WaitingGrill>();

        public List<WaitingGrill> ListWaitingGrills => listWaitingGrills;
        public event Action<WaitingGrill> OnClearItem;
        void Start()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        #region Init
        public void Init()
        {
            transform.position = centerRefPoint.position;

            // game events
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnItemEndSwitch += GameLogicHandler_OnItemEndSwitch;
            gameLogicHandler.OnItemMoveToSlot += GameLogicHandler_OnItemMoveToSlot;
        }

        public void Clear()
        {
            foreach (var waitingGrill in listWaitingGrills)
            {
                GameFactory.Instance.ReturnEntity(waitingGrill);
            }
            listWaitingGrills.Clear();

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnItemEndSwitch -= GameLogicHandler_OnItemEndSwitch;
            gameLogicHandler.OnItemMoveToSlot -= GameLogicHandler_OnItemMoveToSlot;
        }

        private void GameLogicHandler_OnItemEndSwitch(Item item, SlotBase slot)
        {
            if (slot.GetGrill() is WaitingGrill waitingGrill)
            {
                OnItemEndSwitch(item).Forget();
            }
        }

        private void GameLogicHandler_OnItemMoveToSlot(Item item, SlotBase slot)
        {
            if (slot.GetGrill() is WaitingGrill waitingGrill)
            {
                waitingGrill.Visual.PlayShake();
            }
        }

        private async UniTask OnItemEndSwitch(Item item)
        {
            await GameController.Instance.GameLogicHandler.TryCheckMatchItem(item);
            GameController.Instance.GameLogicHandler.TryCheckLoseGame();
        }
        #endregion

        public async UniTask SetData(List<WaitingGrillData> listWaitingGrillData)
        {
            for (int i = 0; i < listWaitingGrillData.Count; i++)
            {
                if (listWaitingGrillData[i].active == 1)
                {
                    var waitingGrill = await AddWaitingGrill(true);
                    waitingGrill.SetId(WAITING_GRILL_ID_OFFSET + i);
                }
                else
                {
                    var waitingGrill = await AddWaitingGrill(false);
                    waitingGrill.SetId(WAITING_GRILL_ID_OFFSET + i);
                }
            }
            await AlignObjects(() => { });
        }

        public async UniTask<WaitingGrill> AddWaitingGrill(bool active = true)
        {
            var waitingGrill = await GameFactory.Instance.CreateEntityAsync<WaitingGrill>("WaitingGrill", transform);
            waitingGrill.SetActive(active);
            waitingGrill.transform.localScale = Vector3.one;
            listWaitingGrills.Add(waitingGrill);
            return waitingGrill;
        }

        public (WaitingGrill waitingGrill, SlotBase slot) GetDestinationSlot()
        {
            foreach (var waitingGrill in listWaitingGrills)
            {
                if (waitingGrill.IsActive == false) continue;
                var slot = waitingGrill.GetSlot(0);
                if (slot.GetItem() == null)
                {
                    return (waitingGrill, slot);
                }
            }

            return (null, null);
        }

        public async UniTask AlignObjects(Action callback)
        {
            // ignore: thì tất cả đi về 1 hướng
            // no ignore: chỉ có active đi về 1 hướng
            var start = -(transform.childCount - 1) * distance / 2;
            var listLocalTargetPositions = new List<Vector3>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var waitingGrill = transform.GetChild(i).GetComponent<WaitingGrill>();
                listLocalTargetPositions.Add(new Vector3(start + distance * i, 0, 0));
                waitingGrill.transform.DOLocalMove(listLocalTargetPositions[i], 0.1f).SetEase(Ease.OutSine);
                await UniTask.Delay(75);
            }

            callback?.Invoke();
        }

        public async UniTask AlignObjects2(Action callback)
        {
            // ignore: thì tất cả đi về 1 hướng
            // no ignore: chỉ có active đi về 1 hướng
            var start = -(transform.childCount - 1) * distance / 2;
            var count = transform.childCount;

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<WaitingGrill>(out WaitingGrill w) && w.IsActive == false)
                {
                    w.transform.SetSiblingIndex(count - 1);
                    break;
                }
            }

            WaitingGrill lockedWaitingGrill = null;
            for (int i = 0; i < count; i++)
            {
                var waitingGrill = transform.GetChild(i).GetComponent<WaitingGrill>();

                if (waitingGrill.IsActive == false)
                {
                    lockedWaitingGrill = waitingGrill;
                    var lastTargetPosition = new Vector3(start + distance * (count - 1), 0, 0);
                    lockedWaitingGrill.transform.DOLocalMove(lastTargetPosition, 0.1f).SetEase(Ease.OutSine);
                }
                else
                {
                    if (i == count - 1)
                    {
                        await UniTask.Delay(75);
                    }
                    var targetPosition = new Vector3(start + distance * i, 0, 0);
                    waitingGrill.transform.DOLocalMove(targetPosition, 0.1f).SetEase(Ease.OutSine);
                }

                if (i == count - 2) continue;
                await UniTask.Delay(75);
            }

            callback?.Invoke();
        }

        public async UniTask AddPlate()
        {
            // var lockedWaitingGrill = listWaitingGrills.Where(e => e.IsActive == false).FirstOrDefault();
            // if (lockedWaitingGrill != null)
            // {
            //     lockedWaitingGrill.PlayUnlock(listWaitingGrills.Count < maxWaitingGrills);
            // }
            var waitingGrill = await AddWaitingGrill(true);
            waitingGrill.transform.localScale = Vector3.zero;
            var targetScale = listWaitingGrills[0].transform.localScale;
            await AlignObjects2(() =>
            {
                waitingGrill.transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutSine);
            });

            await UniTask.Delay(500);
        }

        private void RemoveWaitingGrill(WaitingGrill waitingGrill)
        {
            listWaitingGrills.Remove(waitingGrill);
            GameFactory.Instance.ReturnEntity(waitingGrill);
        }

        public bool CheckClearAllItems()
        {
            foreach (var waitingGrill in listWaitingGrills)
            {
                if (waitingGrill.IsActive && waitingGrill.GetSlots().Where(e => e.GetItem() != null).Count() != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public List<int> GetWaitingGrillIds()
        {
            var list = new List<int>();
            foreach (var waitingGrill in listWaitingGrills)
            {
                if (waitingGrill.IsActive)
                {
                    var item = waitingGrill.GetSlot(0).GetItem();
                    if (item != null)
                    {
                        list.Add(item.id);
                    }
                    else
                    {
                        list.Add(0);
                    }
                }
            }
            return list;
        }

        public bool ClearOnePlate()
        {
            var count = listWaitingGrills.Count(e => e.IsActive);
            if (count > 0 && listWaitingGrills[count - 1].GetSlot(0).GetItem() != null)
            {
                var waitingGrill = listWaitingGrills[count - 1];
                waitingGrill.ClearItem();
                OnClearItem?.Invoke(waitingGrill);
                return true;
            }
            return false;
        }

        public async UniTask Unlock(WaitingGrill waitingGrill)
        {
            GameController.Instance.SetBlockUI(true);
            if (listWaitingGrills.Count(e => e.IsActive) + 1 < boosterAddPlateBehaviorSO.MaxPlate)
            {
                await boosterAddPlateBehaviorSO.UseBooster(waitingGrill.transform.position);
                // waitingGrill.PlayUnlock(true);
            }
            else
            {
                // waitingGrill.PlayUnlock(false);
                var lockedWaitingGrill = listWaitingGrills.Where(e => e.IsActive == false).FirstOrDefault();
                RemoveWaitingGrill(lockedWaitingGrill);
                await boosterAddPlateBehaviorSO.UseBooster(waitingGrill.transform.position);
            }
            GameController.Instance.SetBlockUI(false);
        }
    }
}