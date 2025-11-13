using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.Modules.ProfileInGame;
using MyGame.SkewerJam.Gameplay.Features.LogicOrder;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Level;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public abstract class BaseLogicOrderHandler : MonoBehaviour
    {
        [SerializeField] protected List<BaseOrderSO> listLogicOrders;
        [SerializeField] RescueCondititonSO rescueCondititonSO;

        public bool IsForceRescue { get; private set; }
        public ForceRescueData ForceRescueData => _forceRescueData;
        public LogicOrderData LogicOrderData => _logicOrderData;

        private ForceRescueData _forceRescueData = new ForceRescueData();
        protected LogicOrderData _logicOrderData = new LogicOrderData();

        #region Init
        public virtual void Init()
        {
            foreach (var logicOrder in listLogicOrders)
            {
                logicOrder.Init();
            }

            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData += OnLoadLevelData;

            SetForceRescue(false, -1);
        }

        public virtual void Clear()
        {
            var levelGenerator = GameController.Instance.LevelGenerator;
            levelGenerator.OnLoadLevelData -= OnLoadLevelData;
            SetForceRescue(false, -1);
        }

        protected virtual void OnLoadLevelData(LevelData_SkewerJam levelData)
        {
            var profileInGameService = MySonatFramework.GetService<ProfileInGameService>();
            var (difficulty, difficultyValue) = profileInGameService.GetDifficultyValue(levelData.difficulty, levelData.difficultyValue);

            _logicOrderData = new LogicOrderData();
            _logicOrderData.level = levelData.level;
            _logicOrderData.difficulty = difficulty;
            _logicOrderData.difficultyValue = difficultyValue;

            Debug.Log("<color=purple>LogicOrderHandler:</color> OnLoadLevelData: " + _logicOrderData.level + " - " + _logicOrderData.difficulty + " - " + _logicOrderData.difficultyValue);

            var rescueCondition = rescueCondititonSO.GetRescueCondition(levelData.level, difficulty, difficultyValue);
            var rescueOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Rescue) as RescueOrderSO;
            rescueOrder.SetRescueCondition(rescueCondition);
        }
        #endregion

        public bool CheckCreateNextOrder()
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;

            var dictAllItemIds = ItemHelper.GetItemIdDictInGameplay(-1);
            var orderItemsDict = orderManager.GetOrderItemsDict();

            // số item còn lại <= số item còn lại tạo order
            foreach (var id in dictAllItemIds.Keys)
            {
                if (orderItemsDict.ContainsKey(id) == true)
                {
                    // Nếu item target + item còn lại cùng loại nó > maxItems thì cần tạo thêm order
                    if (dictAllItemIds[id] + orderItemsDict[id].num > orderItemsDict[id].maxItems) return true;
                }
                else
                {
                    // Nếu item còn lại không có trong target thì cần tạo order
                    return true;
                }
            }
            return false;
        }



        public void SetForceRescue(bool isForceRescue, int deltaSlot = -1)
        {
            IsForceRescue = isForceRescue;
            if (isForceRescue == true)
            {
                _forceRescueData.deltaSlot = deltaSlot;
            }
            else
            {
                _forceRescueData.deltaSlot = -1;
            }
        }

        public virtual async UniTask<(ItemId itemId, int num, LogicOrderType logicOrderType)> GetItemOrder(bool isRescue = false)
        {
            // ưu tiện chọn theo logic được chọn
            // otherwise chọn theo basic order
            // otherwise chọn theo force basic order
            // otherwise chọn theo random order
            var gameplayInfoForLogicOrder = GetGameplayInfoForLogicOrder();

            Debug.Log("<color=purple>LogicOrderHandler:</color> -----GetItemOrder----");
            var forceLogicOrder = listLogicOrders.FirstOrDefault(e => e.ForceUse(isRescue));
            if (forceLogicOrder != null)
            {
                Debug.Log("<color=blue>OrderHelper:</color> Use " + forceLogicOrder.name + " to rescue");
                var (rescueItemId, rescueNum) = forceLogicOrder.GetOrder(gameplayInfoForLogicOrder);
                if (rescueItemId != ItemId.None)
                {
                    SetForceRescue(false, -1);
                    return (rescueItemId, rescueNum, LogicOrderType.Rescue);
                }
                else
                {
                    return ForceGetItemOrder(gameplayInfoForLogicOrder);
                }
            }

            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            SetForceRescue(false, -1);

            // lấy order info mỗi layer (2 layer đầu) --> OPTIMIZE: giảm tính toán
            var selectedLogicOrder = ChooseLogicOrder();
            Debug.Log("<color=green>OrderHelper:</color> Use " + selectedLogicOrder.name);

            var (itemId, num) = selectedLogicOrder.GetOrder(gameplayInfoForLogicOrder);
            if (itemId != ItemId.None)
            {
                return (itemId, num, selectedLogicOrder.LogicOrderType);
            }
            else
            {
                return ForceGetItemOrder(gameplayInfoForLogicOrder);
            }
        }

        protected virtual GameplayInfoForLogicOrder GetGameplayInfoForLogicOrder()
        {
            var gameplayInfoForLogicOrder = new GameplayInfoForLogicOrder();
            gameplayInfoForLogicOrder.UpdateState();
            return gameplayInfoForLogicOrder;
        }

        protected abstract BaseOrderSO ChooseLogicOrder();


        private (ItemId itemId, int num, LogicOrderType logicOrderType) ForceGetItemOrder(GameplayInfoForLogicOrder gameplayInfoForLogicOrder)
        {
            Debug.Log("<color=red>OrderHelper:</color> ForceGetItemOrder");
            var basicOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Basic);
            var (i, n, s) = (basicOrder as BasicOrderSO).ForceGetItemOrderBasic(gameplayInfoForLogicOrder);
            if (i != ItemId.None)
            {
                return (i, n, LogicOrderType.Basic);
            }
            else
            {
                var randomOrder = listLogicOrders.FirstOrDefault(e => e.LogicOrderType == LogicOrderType.Random);
                var (randomItemId, randomNum) = randomOrder.GetOrder(gameplayInfoForLogicOrder);
                return (randomItemId, randomNum, LogicOrderType.Random);
            }
        }
    }

    public class LogicOrderData
    {
        public int level;
        public LevelDifficulty difficulty;
        public int difficultyValue;
    }

    public class ForceRescueData
    {
        public int deltaSlot = -1;
    }
}