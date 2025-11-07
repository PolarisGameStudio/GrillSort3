using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using MyGame.SkewerJam.Objects.Entities;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Objects
{
    public class ItemManager : MonoBehaviour
    {
        private int totalItems = 0;
        private int currentItems = 0;

        public int TotalItems => totalItems;
        public int CurrentItems => currentItems;

        public event Action<int> OnUpdateItems;

        public void Init()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData += OnLoadLevelData;
            // GameController.Instance.GameLogicHandler.OnStartCollectItem += OnStartCollectItem;
            GameController.Instance.GameLogicHandler.OnItemEndSwitch += OnItemEndSwitch;
            GameController.Instance.GameLogicHandler.WaitingGrillManager.OnClearItem += OnClearItem;
        }

        public void Clear()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;
            // GameController.Instance.GameLogicHandler.OnStartCollectItem -= OnStartCollectItem;
            GameController.Instance.GameLogicHandler.OnItemEndSwitch -= OnItemEndSwitch;
            GameController.Instance.GameLogicHandler.WaitingGrillManager.OnClearItem -= OnClearItem;
            totalItems = 0;
            currentItems = 0;
        }

        public void OnItemEndSwitch(Item item, SlotBase slot)
        {
            item.OnEndSwitch();
            if (slot.GetGrill() is OrderEntity orderEntity)
            {
                currentItems -= 1;
                Debug.Log($"OnItemEndSwitch: {currentItems}");
                OnUpdateItems?.Invoke(currentItems);
            }
            else
            {
            }

        }
        private void OnClearItem(WaitingGrill waitingGrill)
        {
            currentItems -= 1;
            Debug.Log($"OnClearItem: {currentItems}");
            OnUpdateItems?.Invoke(currentItems);
        }

        private void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            totalItems = 0;
            currentItems = 0;
            foreach (var grillData in levelData.grillData)
            {
                if (grillData.layer != null)
                {
                    foreach (var layerData in grillData.layer)
                    {
                        if (layerData.itemData != null)
                        {
                            foreach (var itemData in layerData.itemData)
                            {
                                if (itemData != null && itemData.id > 0 && ItemHelper.IsItemSpecial((ItemId)itemData.id) == false)
                                {
                                    totalItems++;
                                }
                            }
                        }
                    }
                }
            }

            currentItems = totalItems;
            Debug.Log($"Init: {currentItems}");
            OnUpdateItems?.Invoke(currentItems);
        }
    }
}