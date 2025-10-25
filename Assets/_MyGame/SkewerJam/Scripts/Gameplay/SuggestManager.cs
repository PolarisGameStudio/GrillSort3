using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Plugins.Options;
using Gameplay.Entities;
using Manager;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class SuggestManager : MonoBehaviour
    {
        [SerializeField] private float waitSuggestTime = 15f;

        private List<Item> suggestItems = new List<Item>();

        public void Init()
        {
            StartCoroutine(StartWaitSuggests());

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.OnItemEndSwitch += OnItemEndSwitch;
        }

        public void Clear()
        {
            ClearSuggestItems();
            ClearSuggestBoosters();
        }

        #region Suggest items
        private void OnItemEndSwitch(Item item, SlotBase slot)
        {
            Clear();
            CheckSuggestBoosters();
        }

        public IEnumerator StartWaitSuggests()
        {
            yield return new WaitForSeconds(waitSuggestTime);
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
        private void CheckSuggestBoosters()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            var boosterManager = gameLogicHandler.BoosterManager;
            var boosterType = boosterManager.CheckSuggestBoosters();
            ClearSuggestBoosters();
            if (boosterType != GameResource.None)
            {
                var gameplayScreen = GameController.Instance.GameplayScreen;
                var uiBooster = gameplayScreen.GetUIBooster(boosterType);
                uiBooster.SetSuggest(true);
            }
            // var boosters = gameLogicHandler.BoosterManager.GetBoosters();
            // if (boosters.Count > 0)
            // {
            //     foreach (var booster in boosters)
            //     {
            //         booster.SetSuggest(true);
            //     }
            // }
        }

        private void ClearSuggestBoosters()
        {
            var gameplayScreen = GameController.Instance.GameplayScreen;
            foreach (var uiBooster in gameplayScreen.UiBoosters)
            {
                uiBooster.SetSuggest(false);
            }
        }
        #endregion
    }
}