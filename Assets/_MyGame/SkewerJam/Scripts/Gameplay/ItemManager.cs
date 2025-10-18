using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyGame.SkewerJam.Level;
using MyGame.SkewerJam.Objects.Entities;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class ItemManager : MonoBehaviour
    {
        private int totalItems = 0;
        private int currentItems = 0;

        public int TotalItems => totalItems;
        public int CurrentItems => currentItems;

        public event Action<int> OnUpdateItems;

        public void OnStartCollectItem(OrderEntity orderEntity)
        {
            currentItems -= orderEntity.MaxItems;
            Debug.Log($"OnStartCollectItem: {currentItems}");
            OnUpdateItems?.Invoke(currentItems);
        }

        public void Init()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData += OnLoadLevelData;
            GameController.Instance.GameLogicHandler.OnStartCollectItem += OnStartCollectItem;
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
                                if (itemData != null && itemData.id > 0)
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


        public void Clear()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;
            GameController.Instance.GameLogicHandler.OnStartCollectItem -= OnStartCollectItem;
            totalItems = 0;
            currentItems = 0;
        }
    }
}