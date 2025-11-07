using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using Gameplay.Entities.Grills;
using Gameplay.Entities.Obstacle;
using Gameplay.LevelData;
using Manager;
using MyGame.Modules.QuestEvent;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Gameplay.Objects;
using MyGame.SkewerJam.Level;
using MyGame.SkewerJam.Objects;
using Sonat.Enums;
using SonatFramework.Systems.LevelManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private LevelServiceAsync levelService;
        private LevelData_SkewerJam _levelData = null;

        public LevelData_SkewerJam LevelData => _levelData;
        public event Action<LevelData_SkewerJam> OnLoadLevelData;

        private int _level;

        public void Init()
        {

        }

        public void Clear()
        {
            _levelData = null;
        }

        public async UniTask GenerateLevel(int level, bool backup = false)
        {
            _level = level;

            _levelData = await levelService.GetLevelData<LevelData_SkewerJam>(level, GameMode.Classic);
            _levelData = ValidateLevelData(_levelData);
            OnLoadLevelData?.Invoke(_levelData);

            await LoadGameObjects(_levelData);
        }

        public async UniTask LoadGameObjects(LevelData_SkewerJam levelData)
        {
            var gameLogicHandler = GameController.Instance.GameLogicHandler;

            // create grill
            var grillManager = gameLogicHandler.GrillManager;
            await CreateGrill(levelData.grillData, grillManager);
            await UniTask.DelayFrame(1);

            // tính toán viewport mới create conveyor
            await GameController.Instance.GameViewport.CalculateViewport();
            await UniTask.DelayFrame(1);

            // create waiting grill
            var waitingGrillManager = gameLogicHandler.WaitingGrillManager;
            await CreateWaitingGrill(levelData.ListWaitingGrillData, waitingGrillManager);
            await UniTask.DelayFrame(1);

            // create order
            var orderManager = gameLogicHandler.OrderManager;
            await CreateOrder(levelData.ListOrderData, orderManager);
            await UniTask.DelayFrame(1);

            // create conveyor
            var conveyorManager = gameLogicHandler.ConveyorManager;
            await CreateConveyors(levelData.conveyorData, conveyorManager);


            // create obstacles
            var obstacleManager = gameLogicHandler.ObstacleManager;
            await GenerateObstacles(levelData.obstacleData, obstacleManager);
            await UniTask.DelayFrame(1);

            // create special item = quest item
            GenerateQuestItem();
        }

        private LevelData_SkewerJam ValidateLevelData(LevelData_SkewerJam levelData)
        {
            var levelDataSkewerJam = levelData.CloneSkewerJam();
            levelDataSkewerJam.numberOfWaitingGrill = 5;

            levelDataSkewerJam.rescueCondition = new RescueCondition(_level, levelDataSkewerJam.difficulty);

            // bộ level cũ
            // if ((int)levelDataSkewerJam.difficulty == 0)
            // {
            //     levelDataSkewerJam.difficulty = LevelDifficulty.Easy1;
            // }
            // if ((int)levelDataSkewerJam.difficulty == 1)
            // {
            //     levelDataSkewerJam.difficulty = LevelDifficulty.Medium1;
            // }
            // if ((int)levelDataSkewerJam.difficulty == 2)
            // {
            //     levelDataSkewerJam.difficulty = LevelDifficulty.Hard1;
            // }


            // shuffle item
            var sonatLevelServiceAsync = levelService as SonatLevelServiceAsync;
            if (_level > (sonatLevelServiceAsync.GameModeLevels.FirstOrDefault(e => e.mode == GameMode.Classic)?.level ?? 0))
            {
                var itemIds = new HashSet<int>();
                foreach (var grillData in levelDataSkewerJam.grillData)
                {
                    if (grillData.layer == null) continue;
                    foreach (var layerData in grillData.layer)
                    {
                        if (layerData.itemData == null) continue;
                        foreach (var itemData in layerData.itemData)
                        {
                            if (itemData != null && itemData.id > 0)
                            {
                                itemIds.Add(itemData.id);
                            }
                        }
                    }
                }

                // shuffle item
                var dictConvertItemId = new Dictionary<int, int>();
                var shuffleItemIds = new List<int>(itemIds.ToList());
                shuffleItemIds.Shuffle();
                for (int i = 0; i < itemIds.Count; i++)
                {
                    dictConvertItemId.Add(itemIds.ToList()[i], shuffleItemIds[i]);
                }

                // set laij item id
                foreach (var grillData in levelDataSkewerJam.grillData)
                {
                    if (grillData.layer == null) continue;
                    foreach (var layerData in grillData.layer)
                    {
                        if (layerData.itemData == null) continue;
                        foreach (var itemData in layerData.itemData)
                        {
                            if (itemData != null && itemData.id > 0)
                            {
                                itemData.id = dictConvertItemId[itemData.id];
                            }
                        }
                    }
                }

            }


            levelDataSkewerJam.ListWaitingGrillData = ValidateListWaitingGrill(5, 1);


            if (_level == 1)
            {
                levelDataSkewerJam.ListOrderData = ValidateListOrder(2, 2);
            }
            else
            {
                levelDataSkewerJam.ListOrderData = ValidateListOrder(4, 2);
            }
            return levelDataSkewerJam;
        }

        #region Create core
        private async UniTask CreateGrill(List<GrillData> listGrillData, GrillManager grillManager)
        {
            foreach (var grillData in listGrillData)
            {
                if (grillData.isLock) grillData.grillType = GrillType.Lock;

                var grillType = grillData.grillType.ValidateGrillType();
                var slotCount = grillData.SlotCount;

                var (validateGrillType, validateSlotCount) = ValidateGrillType(grillType, slotCount);
                if (validateGrillType == null) continue;

                PrimaryGrill grill = await GameFactory.Instance.CreateEntityAsync<PrimaryGrill>(
                        $"PrimaryGrill{validateGrillType}_{validateSlotCount}",
                        grillManager.transform
                    );

                //grill.transform.SetParent(gameplaySpace);
                await grill.SetData(grillData);
                grillManager.AddGrill(grill);
            }

            CheckObstacles();
        }

        private (GrillType? validateGrillType, int validateSlotCount) ValidateGrillType(GrillType grillType, int slotCount)
        {
            switch (grillType)
            {
                case GrillType.LockAds:
                    return (null, 0);
                    // case GrillType.Lid:
                    //     return (GrillType.Normal, 0);

            }

            return (grillType, slotCount);
        }

        private void CheckObstacles()
        {
            LockObstacle.CheckAndStartProgress();
            PrimaryGrillIce.CheckAndStartProgress();
        }

        private async UniTask CreateWaitingGrill(List<WaitingGrillData> listWaitingGrillData, WaitingGrillManager waitingSlotManager)
        {
            await waitingSlotManager.SetData(listWaitingGrillData);
        }

        private List<WaitingGrillData> ValidateListWaitingGrill(int numberOfWaitingGrill, int numLockedWaitingGrill)
        {
            var listWaitingGrillData = new List<WaitingGrillData>();
            for (int i = 0; i < numberOfWaitingGrill; i++)
            {
                listWaitingGrillData.Add(new WaitingGrillData() { id = i, active = 1 });
            }
            for (int i = 0; i < numLockedWaitingGrill; i++)
            {
                listWaitingGrillData.Add(new WaitingGrillData() { id = i, active = 0 });
            }
            return listWaitingGrillData;
        }

        private async UniTask CreateOrder(List<OrderData_SkewerJam> listOrderData, OrderManager orderManager)
        {
            // không cần đợi tạo order
            orderManager.SetData(listOrderData);
        }

        private List<OrderData_SkewerJam> ValidateListOrder(int maxOrder, int defaultNumberOfReadyOrder)
        {
            var listOrderData = new List<OrderData_SkewerJam>();
            for (int i = 0; i < maxOrder; i++)
            {
                listOrderData.Add(new OrderData_SkewerJam() { id = i, active = i < defaultNumberOfReadyOrder ? 1 : 0 });
            }
            return listOrderData;
        }
        #endregion

        private async UniTask CreateConveyors(List<ConveyorData> listConveyorData, ConveyorManager conveyorManager)
        {
            if (listConveyorData == null) return;

            foreach (var conveyorData in listConveyorData)
            {
                ConveyorType conveyorType = conveyorData.conveyorType == ConveyorType.None
                    ? (conveyorData.moveType == MoveType.Horizontal ? ConveyorType.Horizontal : ConveyorType.Vertical)
                    : conveyorData.conveyorType;
                var conveyor = await GameFactory.Instance.CreateEntityAsync<ConveyorController>($"Conveyor{conveyorType}", conveyorManager.transform);
                conveyor.SetData(conveyorData);
                conveyorManager.AddConveyor(conveyor);
            }
        }

        public bool IsStaticGrill(int id)
        {
            if (_levelData.conveyorData == null) return true;
            foreach (var conveyorData in _levelData.conveyorData)
            {
                if (conveyorData.grillIds.Contains(id)) return false;
            }

            return true;
        }

        private async UniTask GenerateObstacles(List<ObstacleData> listObstacleData, ObstacleManager obstacleManager)
        {
            if (listObstacleData == null) return;
            foreach (var obstacleData in listObstacleData)
            {
                //var grill = primaryGrills.FirstOrDefault(e => e.id == grillObstacleData.grillId);
                var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
                var listGrills = grillManager.ListGrills;
                var grills = listGrills.FindAll(e => obstacleData.grillIds.Contains(e.id));
                if (grills.Count == 0)
                {
                    Debug.LogWarning($"Grill not found");
                    continue;
                }

                var obstacle = await GameFactory.Instance.CreateEntityAsync<ObstacleBase>($"Obstacle{obstacleData.obstacleType}", obstacleManager.transform);
                List<GrillBase> grillsSelected = new List<GrillBase>(grills);
                obstacle.SetData(obstacleData);
                obstacle.SetGrill(grillsSelected);
                obstacleManager.AddObstacle(obstacle);
            }
        }



        private void GenerateQuestItem()
        {
            QuestEventService questEventService = MySonatFramework.GetService<QuestEventService>();
            if (!questEventService.CanSpawItemQuestEvent()) return;

            int specialItemId = 1000;


            var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
            var listPrimaryGrills = grillManager.ListGrills;
            List<PrimaryGrill> grillsSelected = RandomExtensions.GetRandomElemntsInList(listPrimaryGrills, listPrimaryGrills.Count);
            int count = 0;
            foreach (var primaryGrill in grillsSelected)
            {
                if (primaryGrill.CreateSpecialItem(specialItemId, ItemType.Special_QuestEvent))
                {
                    count++;
                    if (count >= GameRemoteConfigValue.numberSpecialItemPerLevel) return;
                }
            }
        }
    }
}