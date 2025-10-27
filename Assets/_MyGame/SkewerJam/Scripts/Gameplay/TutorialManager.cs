using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.BoosterManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialConfigSO tutorialConfigSO;
        [SerializeField] private BoostersConfig boostersConfig;


        public void Init()
        {
            GameController.OnPlayTutorial += OnPlayTutorial;

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.BoosterManager.OnUseBooster += OnUseBooster;
        }

        public void Clear()
        {
            GameController.OnPlayTutorial -= OnPlayTutorial;

            var gameLogicHandler = GameController.Instance.GameLogicHandler;
            gameLogicHandler.BoosterManager.OnUseBooster -= OnUseBooster;
        }

        private void OnUseBooster(GameResource boosterType)
        {
            var popupForceBooster = PanelManager.Instance.GetPanel<PopupTutorialForceBooster>();
            if (popupForceBooster != null)
            {
                PanelManager.Instance.ClosePanel<PopupTutorialForceBooster>();
            }
        }

        private void OnPlayTutorial()
        {
            // tut level 1
            if (GameController.Instance.Level == 1)
            {
                PanelManager.Instance.OpenPanel<PopupTutorialGameplay>();
                return;
            }

            var listTutTypes = GetAllTutorialType();
            if (listTutTypes.Count > 0)
            {
                foreach (var tutorialType in listTutTypes)
                {
                    var isShow = TryShowTutorial(tutorialType);
                    if (isShow)
                    {
                        break;
                    }
                }
            }
        }

        private List<TutorialType> GetAllTutorialType()
        {
            var listTutorialTypes = new List<TutorialType>();

            // tut booster
            var tutorialType = CheckTutorialBooster();
            if (tutorialType != TutorialType.None)
            {
                listTutorialTypes.Add(tutorialType);
                return listTutorialTypes;
            }

            // tut obstacle
            var levelData = GameController.Instance.LevelGenerator.LevelData;
            foreach (var grillData in levelData.grillData)
            {
                if (grillData.isLock) listTutorialTypes.Add(TutorialType.PrimaryGrill_Lock);

                var grillType = grillData.grillType.ValidateGrillType();
                var slotCount = grillData.SlotCount;
                switch (grillType)
                {
                    case GrillType.Lock:
                        listTutorialTypes.Add(TutorialType.PrimaryGrill_Lock);
                        break;
                    case GrillType.Lid:
                        listTutorialTypes.Add(TutorialType.PrimaryGrill_Lid);
                        break;
                    case GrillType.LockAndKey:
                    case GrillType.LockAndKey2:
                        listTutorialTypes.Add(TutorialType.PrimaryGrill_LockAndKey);
                        break;
                    case GrillType.Ice:
                        listTutorialTypes.Add(TutorialType.PrimaryGrill_Ice);
                        break;
                    case GrillType.Vending:
                        listTutorialTypes.Add(TutorialType.PrimaryGrill_Vending);
                        break;
                    case GrillType.Normal:
                        if (slotCount == 1)
                            listTutorialTypes.Add(TutorialType.PrimaryGrill_Single);
                        break;
                }

                if (grillData.layer == null) continue;
                foreach (var itemData in grillData.layer)
                {
                    if (itemData.itemData == null) continue;
                    foreach (var item in itemData.itemData)
                    {
                        if (item == null) continue;
                        if (item.itemType == ItemType.Bomb)
                        {
                            listTutorialTypes.Add(TutorialType.Item_Bomb);
                        }
                        if (item.itemType == ItemType.Hidden)
                        {
                            listTutorialTypes.Add(TutorialType.Item_Hidden);
                        }
                        if (item.itemType == ItemType.Ice)
                        {
                            listTutorialTypes.Add(TutorialType.Item_Ice);
                        }
                    }
                }


            }


            // check obstacle
            if (levelData.obstacleData != null)
            {
                foreach (var obstacleData in levelData.obstacleData)
                {
                    if (obstacleData.obstacleType == ObstacleType.OctoChef)
                    {
                        listTutorialTypes.Add(TutorialType.Obstacle_Octochef_1);
                    }
                }
            }


            if (levelData.conveyorData != null && levelData.conveyorData.Count > 0)
            {
                listTutorialTypes.Add(TutorialType.Obstacle_Conveyor);
            }

            return listTutorialTypes.Distinct().ToList();
        }

        private TutorialType CheckTutorialBooster()
        {
            var level = GameController.Instance.Level;
            var boosterConfig = boostersConfig.configs.Find(x => x.levelUnlock == level);

            if (boosterConfig != null)
            {
                return Enum.Parse<TutorialType>(boosterConfig.booster.ToString());
            }
            return TutorialType.None;
        }

        private bool TryShowTutorial(TutorialType tutorialType)
        {
            if (PlayerPrefs.HasKey($"PopupTutorial_Showed_{tutorialType}"))
            {
                return false;
            }

            PlayerPrefs.SetInt($"PopupTutorial_Showed_{tutorialType}", 1);

            ShowPopupTutorial(tutorialType);

            return true;
        }

        private void ShowPopupTutorial(TutorialType tutorialType)
        {
            switch (tutorialType)
            {
                case TutorialType.BoosterSpatula:
                case TutorialType.BoosterAddPlate:
                case TutorialType.BoosterShuffle:
                case TutorialType.BoosterFoodBox:
                    ShowPopupTutorialBooster(tutorialType);
                    return;


                case TutorialType.PrimaryGrill_Single:
                case TutorialType.PrimaryGrill_Lock:
                case TutorialType.PrimaryGrill_Lid:
                // case TutorialType.PrimaryGrill_LockAndKey:
                // case TutorialType.PrimaryGrill_Vending:
                // case TutorialType.PrimaryGrill_Ice:
                case TutorialType.Item_Bomb:
                case TutorialType.Item_Hidden:
                case TutorialType.Item_Ice:

                case TutorialType.Obstacle_Octochef_1:
                    // case TutorialType.Obstacle_Conveyor:
                    ShowPopupTutorialObstacle(tutorialType);
                    return;

                // -----------special case-----------
                case TutorialType.PrimaryGrill_LockAndKey:
                case TutorialType.PrimaryGrill_Vending:
                case TutorialType.PrimaryGrill_Ice:
                    ShowPopupTutorialGroup(tutorialType, $"PopupTutorialGroup_{tutorialType}");
                    return;

                case TutorialType.Obstacle_Conveyor:
                    ShowPopupTutorialObstacle(tutorialType, $"PopupTutorialObstacle_{tutorialType}");
                    return;

                default:
                    // ShowPopupTutorial(tutorialType);
                    return;
            }
        }

        private async UniTask ShowPopupTutorialBooster(TutorialType tutorialType)
        {
            GameController.Instance.SetBlockUI(true);
            var tutorialData = tutorialConfigSO.tutorialDatas.Find(x => x.tutorialType == tutorialType);
            var boosterType = Enum.Parse<GameResource>(tutorialType.ToString());

            var uiData = new UIData();
            uiData.Add(PopupTutorial.NAME_KEY, tutorialData.name);
            uiData.Add(PopupTutorial.DESCRIPTION_KEY, tutorialData.listDescription);
            uiData.Add(PopupTutorialBooster.GAME_RESOURCE_KEY, boosterType);

            var popup = PanelManager.Instance.OpenPanel<PopupTutorialBooster>(uiData);
            await UniTask.WaitUntil(() => popup == null || popup.gameObject.activeSelf == false);


            var boosterManager = GameController.Instance.GameLogicHandler.BoosterManager;
            await boosterManager.PrepareTutorialBooster(boosterType);


            var uiDataForceBooster = new UIData();
            uiDataForceBooster.Add(PopupTutorialForceBooster.BOOSTER_TYPE_KEY, boosterType);
            var popupForceBooster = PanelManager.Instance.OpenPanel<PopupTutorialForceBooster>(uiDataForceBooster);
            boosterManager.SetForceUseBooster(boosterType);
            GameController.Instance.SetBlockUI(false);
        }

        private void ShowPopupTutorialObstacle(TutorialType tutorialType, string popuTutorialName = "PopupTutorialObstacle")
        {
            var tutorialData = tutorialConfigSO.tutorialDatas.Find(x => x.tutorialType == tutorialType);

            var uiData = new UIData();
            uiData.Add(PopupTutorial.NAME_KEY, tutorialData.name);
            uiData.Add(PopupTutorial.DESCRIPTION_KEY, tutorialData.listDescription);
            uiData.Add(PopupTutorialObstacle.TUTORIAL_TYPE_KEY, tutorialType);

            PanelManager.Instance.OpenPanelByName<PopupTutorialObstacle>(popuTutorialName, uiData);
        }

        private void ShowPopupTutorialGroup(TutorialType tutorialType, string popuTutorialName)
        {
            var tutorialData = tutorialConfigSO.tutorialDatas.Find(x => x.tutorialType == tutorialType);

            var uiData = new UIData();
            uiData.Add(PopupTutorial.NAME_KEY, tutorialData.name);
            uiData.Add(PopupTutorial.DESCRIPTION_KEY, tutorialData.listDescription);
            uiData.Add(PopupTutorialObstacle.TUTORIAL_TYPE_KEY, tutorialType);

            PanelManager.Instance.OpenPanelByName<PopupTutorialGroup>(popuTutorialName, uiData);
        }

#if UNITY_EDITOR
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Alpha1))
        //     {
        //         ShowPopupTutorial(TutorialType.BoosterAddPlate);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha2))
        //     {
        //         ShowPopupTutorial(TutorialType.PrimaryGrill_Single);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha3))
        //     {
        //         ShowPopupTutorial(TutorialType.Item_Bomb);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha4))
        //     {
        //         ShowPopupTutorial(TutorialType.Obstacle_Octochef_1);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha5))
        //     {
        //         ShowPopupTutorial(TutorialType.PrimaryGrill_Ice);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha6))
        //     {
        //         ShowPopupTutorial(TutorialType.PrimaryGrill_Vending);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha7))
        //     {
        //         ShowPopupTutorial(TutorialType.PrimaryGrill_LockAndKey);
        //     }
        //     if (Input.GetKeyDown(KeyCode.Alpha8))
        //     {
        //         ShowPopupTutorial(TutorialType.Obstacle_Conveyor);
        //     }
        // }
#endif
    }

    public enum TutorialType
    {
        None = 0,

        BoosterSpatula = 1,
        BoosterAddPlate = 2,
        BoosterShuffle = 3,
        BoosterFoodBox = 4,

        PrimaryGrill_Single = 10,
        PrimaryGrill_Lock = 11,
        PrimaryGrill_Lid = 12,
        PrimaryGrill_Vending = 13,
        PrimaryGrill_LockAndKey = 14,
        PrimaryGrill_Ice = 15,

        Item_Bomb = 40,
        Item_Hidden = 41,
        Item_Ice = 42,

        Obstacle_Octochef_1 = 70,
        Obstacle_Conveyor = 71,
    }
}