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
using static MyGame.SkewerJam.Objects.Entities.OrderEntity;
using Manager;

namespace MyGame.SkewerJam.Objects
{
    public class WaitingGrillManager : MonoBehaviour
    {
        private const int WAITING_GRILL_ID_OFFSET = 1000;

        [SerializeField] private int maxWaitingGrills = 10;
        [SerializeField] private Transform centerRefPoint;
        [SerializeField] private float distance = 1.2f;

        private List<WaitingGrill> listWaitingGrills = new List<WaitingGrill>();

        public List<WaitingGrill> ListWaitingGrills => listWaitingGrills;

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
        }

        private void GameLogicHandler_OnItemEndSwitch(Item item, SlotBase slot)
        {
            if (slot.GetGrill() is WaitingGrill waitingGrill)
            {
                GameController.Instance.GameLogicHandler.TryCheckMatchItem(item);
                GameController.Instance.GameLogicHandler.TryCheckLoseGame();
            }
        }
        #endregion

        public async UniTask SetData(List<WaitingGrillData> listWaitingGrillData)
        {
            for (int i = 0; i < listWaitingGrillData.Count; i++)
            {
                var waitingGrill = await AddWaitingGrill(true);
                waitingGrill.SetId(WAITING_GRILL_ID_OFFSET + i);
            }

            // await AddLockedWaitingGrill();
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
            var start = -(transform.childCount - 1) * distance / 2;
            var listLocalTargetPositions = new List<Vector3>();
            for (int i = 0; i < transform.childCount; i++)
            {
                listLocalTargetPositions.Add(new Vector3(start + distance * i, 0, 0));

                var waitingGrill = transform.GetChild(i).GetComponent<WaitingGrill>();
                waitingGrill.transform.DOLocalMove(listLocalTargetPositions[i], 0.1f).SetEase(Ease.OutSine);
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
            await AlignObjects(() =>
            {
                waitingGrill.transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutSine);
            });

            await UniTask.Delay(500);
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
    }
}