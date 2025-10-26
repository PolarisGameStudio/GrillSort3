using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay.Configs;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class SuggestManager : MonoBehaviour
    {
        [SerializeField] private SuggestManagerConfigSO suggestManagerConfigSO;

        private List<Item> suggestItems = new List<Item>();

        public void Init()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnItemEndSwitch += OnItemEndSwitch;
            gameLogicHandler.BoosterManager.OnUseBooster += OnUseBooster;
        }

        public void Clear()
        {
            ClearSuggestItems();
            ClearSuggestBoosters();
        }

        private void OnItemEndSwitch(Item item, SlotBase slot)
        {
            Clear();
            StartCoroutine(StartWaitSuggests());
            StartCoroutine(StartSuggestBoosters());
        }

        private void OnUseBooster(GameResource boosterType)
        {
            Clear();
            StartCoroutine(StartWaitSuggests());
            StartCoroutine(StartSuggestBoosters());
        }

        #region Suggest items
        public IEnumerator StartWaitSuggests()
        {
            yield return new WaitForSeconds(suggestManagerConfigSO.waitSuggestItemsTime);
            yield return new WaitUntil(() => GameController.Instance.GameState == GameState.Playing && suggestItems.Count == 0);
            var items = GetSuggestItems();
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    item.SetSuggest(true);
                    suggestItems.Add(item);
                }
            }
        }

        public List<Item> GetSuggestItems()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;

            var orderItemsDict = gameLogicHandler.OrderManager.GetOrderItemsDict();
            var listItemsInGrillManager = gameLogicHandler.GrillManager.GetItemsWithLayer(1);

            var listItemsBackup = new List<Item>();
            foreach (var (itemId, (maxItems, num)) in orderItemsDict)
            {
                var items = listItemsInGrillManager.FindAll(e => (ItemId)e.id == itemId);
                if (items.Count >= (maxItems - num))
                {
                    return items.Take(maxItems - num).ToList();
                }
                if (listItemsBackup.Count == 0 && items.Count > 0)
                {
                    listItemsBackup = items;
                }
            }
            return listItemsBackup;
        }

        private void ClearSuggestItems()
        {
            StopAllCoroutines();
            if (suggestItems.Count > 0)
            {
                foreach (var item in suggestItems)
                {
                    item.SetSuggest(false);
                }
                suggestItems.Clear();
            }
        }
        #endregion

        #region Suggest boosters
        private IEnumerator StartSuggestBoosters()
        {
            // không còn ăn được item nào
            yield return new WaitForSeconds(suggestManagerConfigSO.waitSuggestBoostersTime);
            if (CheckMatchItems())
            {
                yield break;
            }

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var boosterManager = gameLogicHandler.BoosterManager;
            var boosterTypes = boosterManager.GetSuggestBoosters();

            if (boosterTypes.Count > 0)
            {
                var rand = Random.Range(0, boosterTypes.Count);
                var boosterType = boosterTypes[rand];
                var gameplayScreen = GameController.Instance.GameplayScreen;
                var uiBooster = gameplayScreen.GetUIBooster(boosterType);
                uiBooster.SetSuggest(true);
            }
        }

        private void ClearSuggestBoosters()
        {
            var gameplayScreen = GameController.Instance.GameplayScreen;
            foreach (var uiBooster in gameplayScreen.UiBoosters)
            {
                uiBooster.SetSuggest(false);
            }
        }

        private bool CheckMatchItems()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;

            var orderItemsDict = gameLogicHandler.OrderManager.GetOrderItemsDict();
            var listItemsInGrillManager = gameLogicHandler.GrillManager.GetItemsWithLayer(1);

            foreach (var item in listItemsInGrillManager)
            {
                if (orderItemsDict.ContainsKey((ItemId)item.id)) return true;
            }
            return false;
        }
        #endregion
    }
}