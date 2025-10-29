using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SO.Boosters;
using Sonat.Enums;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class BoosterManager : MonoBehaviour
    {
        private const string LOG_TAG = "<color=yellow>BoosterLogicHandler: </color>";
        public event Action<GameResource> OnUseBooster;

        private GameResource _forceUseBoosterType = GameResource.None;
        private List<BaseBoosterBehaviorSO> listBoosterBehaviors => MySonatFramework.GetService<SonatBoosterService>().BoostersConfig.boosterBehaviors.ToList();

        private void OnItemStartSwitch(Item item, SlotBase slot)
        {
            var uiBooster = GameController.Instance.GameplayScreen.UiBoosters.FirstOrDefault(e => e.boosterType == GameResource.BoosterFoodBox);
            if (uiBooster != null)
            {
                uiBooster.EnableActiveButton(WaitingGrillHelper.CheckAllEmpty() == false);
            }
        }
        private void OnResetStack()
        {
            var uiBooster = GameController.Instance.GameplayScreen.UiBoosters.FirstOrDefault(e => e.boosterType == GameResource.BoosterUndo);
            if (uiBooster != null)
            {
                uiBooster.EnableActiveButton(false);
            }
        }

        private void OnAddCommand()
        {
            var uiBooster = GameController.Instance.GameplayScreen.UiBoosters.FirstOrDefault(e => e.boosterType == GameResource.BoosterUndo);
            if (uiBooster != null)
            {
                uiBooster.EnableActiveButton(true);
            }
        }

        public void Init()
        {
            _forceUseBoosterType = GameResource.None;

            var uiBooster = GameController.Instance.GameplayScreen.UiBoosters.FirstOrDefault(e => e.boosterType == GameResource.BoosterFoodBox);
            var uiBoosterUndo = GameController.Instance.GameplayScreen.UiBoosters.FirstOrDefault(e => e.boosterType == GameResource.BoosterUndo);
            if (uiBooster != null)
            {
                uiBooster.EnableActiveButton(false);
            }

            if (uiBoosterUndo != null)
            {
                uiBoosterUndo.EnableActiveButton(false);
            }

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.CommandInvoker.OnResetStack += OnResetStack;
            gameLogicHandler.CommandInvoker.OnAddCommand += OnAddCommand;
            gameLogicHandler.OnItemStartSwitch += OnItemStartSwitch;
            gameLogicHandler.OnItemStartSwitchUndo += OnItemStartSwitch;
        }

        public void Clear()
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.CommandInvoker.OnResetStack -= OnResetStack;
            gameLogicHandler.CommandInvoker.OnAddCommand -= OnAddCommand;
            gameLogicHandler.OnItemStartSwitch -= OnItemStartSwitch;
            gameLogicHandler.OnItemStartSwitchUndo -= OnItemStartSwitch;
        }

        #region Booster Behavior
        public BaseBoosterBehaviorSO GetBoosterBehavior(GameResource boosterType)
        {
            return listBoosterBehaviors.FirstOrDefault(e => e.boosterType == boosterType);
        }

        public async UniTask<bool> UseBooster(GameResource boosterType, Vector3 position)
        {
            GameController.Instance.ChangeGameState(GameState.UsingBooster);
            var boosterBehavior = GetBoosterBehavior(boosterType);
            if (boosterBehavior == null)
            {
                Debug.Log($"{LOG_TAG} SBooster behavior not found: {boosterType}");
            }

            WaitingGrillHelper.ResetWarning();
            OnUseBooster?.Invoke(boosterType);
            var success = await boosterBehavior.UseBooster(position);
            if (success)
            {
                await boosterBehavior.PostUseBooster();
            }

            GameController.Instance.ChangeGameState(GameState.Playing);
            return success;
        }

        public async UniTask<bool> ForceUseBooster(GameResource boosterType)
        {
            GameController.Instance.ChangeGameState(GameState.UsingBooster);
            var boosterBehavior = GetBoosterBehavior(boosterType);
            if (boosterBehavior == null)
            {
                Debug.Log($"{LOG_TAG} SBooster behavior not found: {boosterType}");
            }

            WaitingGrillHelper.ResetWarning();
            var success = await boosterBehavior.ForceUseBooster();
            if (success)
            {
                await boosterBehavior.PostUseBooster();
            }

            GameController.Instance.ChangeGameState(GameState.Playing);
            return success;
        }

        public (bool canUse, string reason) CanUseBooster(GameResource boosterType)
        {
            if (GameController.Instance.GameState != GameState.Playing) return (false, "GameState is not playing");

            var boosterBehavior = GetBoosterBehavior(boosterType);
            return boosterBehavior.CanUseBooster();
        }
        #endregion

        #region Suggest Booster
        public List<GameResource> GetSuggestBoosters()
        {
            var suggestBoosterTypes = new List<GameResource>();
            foreach (var boosterBehavior in listBoosterBehaviors)
            {
                var checkUnlock = MySonatFramework.GetService<BoosterService>().IsBoosterUnlock(boosterBehavior.boosterType);
                if (checkUnlock && boosterBehavior.CanUseBooster().canUse && boosterBehavior.CheckSuggest())
                {
                    suggestBoosterTypes.Add(boosterBehavior.boosterType);
                }
            }

            var suggestBoosterTypes2 = new List<GameResource>();
            foreach (var boosterType in suggestBoosterTypes)
            {
                var quantity = MySonatFramework.GetService<InventoryService>().GetResource(boosterType);
                if (quantity > 0)
                {
                    suggestBoosterTypes2.Add(boosterType);
                }
            }

            return suggestBoosterTypes2.Count > 0 ? suggestBoosterTypes2 : suggestBoosterTypes;
        }
        #endregion

        #region Force Use Booster
        public void SetForceUseBooster(GameResource boosterType)
        {
            _forceUseBoosterType = boosterType;
        }

        public bool IsForceUseBooster(GameResource boosterType = GameResource.None)
        {
            if (boosterType == GameResource.None)
            {
                return _forceUseBoosterType != GameResource.None;
            }
            return _forceUseBoosterType == boosterType;
        }

        public async UniTask PrepareTutorialBooster(GameResource boosterType)
        {
            var boosterBehavior = GetBoosterBehavior(boosterType);
            if (boosterBehavior == null)
            {
                Debug.Log($"{LOG_TAG} SBooster behavior not found: {boosterType}");
            }

            await boosterBehavior.PrepareTutorialBooster();
        }
    }
    #endregion
}