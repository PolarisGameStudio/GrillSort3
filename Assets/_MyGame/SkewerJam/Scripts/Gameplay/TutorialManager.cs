using Cysharp.Threading.Tasks;
using Gameplay.LevelData;
using MyGame.SkewerJam.UI.Tut;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class TutorialManager : MonoBehaviour
    {
        EventBinding<LevelStartedEvent> eventBinding;

        private readonly Service<BoosterService> boosterService = new();

        private void OnEnable()
        {
            eventBinding = new EventBinding<LevelStartedEvent>(OnStartLevel);
            // boosterService.Instance.onUnlockBooster += OnUnlockBooster;
        }

        private void OnDisable()
        {
            EventBus<LevelStartedEvent>.Deregister(eventBinding);
            // boosterService.Instance.onUnlockBooster -= OnUnlockBooster;
        }

        private void OnStartLevel(LevelStartedEvent eventData)
        {
            var tutorialType = CheckTutorial(eventData.level);
            if (tutorialType != TutorialType.None)
            {
                if (PlayerPrefs.HasKey($"{tutorialType.ToString()}Showed"))
                {
                    return;
                }
                CheckShowTutObstacle(tutorialType.ToString());
            }

            GameResource boosterType = GameResource.None;
            switch (eventData.level)
            {
                case 2:
                    boosterType = GameResource.BoosterSpatula;
                    break;
                case 5:
                    boosterType = GameResource.BoosterAddPlate;
                    break;
                case 7:
                    boosterType = GameResource.BoosterShuffle;
                    break;
                case 9:
                    boosterType = GameResource.BoosterFoodBox;
                    break;
            }

            if (boosterType != GameResource.None && !PlayerPrefs.HasKey($"{boosterType.ToString()}Showed"))
            {
                PlayerPrefs.SetInt($"{boosterType.ToString()}Showed", 1);

                var uiData = new UIData();
                uiData.Add("BoosterType", boosterType);
                SonatUtils.DelayCall(2.75f, () =>
                {
                    PanelManager.Instance.OpenPanel<PopupTutBooster>(uiData);
                }, this);
            }
        }

        // private void OnUnlockBooster(GameResource boosterType)
        // {
        //     // ShowPopupTutorial<PopupTutBooster>("PopupTut" + boosterType.ToString()).Forget();
        // }

        private TutorialType CheckTutorial(int level)
        {
            var levelData = GameController.Instance.LevelGenerator.LevelData;
            if (levelData.obstacleData != null)
            {
                foreach (var obstacleData in levelData.obstacleData)
                {
                    if (obstacleData.obstacleType == ObstacleType.OctoChef)
                    {
                        return TutorialType.PopupTutOctochef_SkewerJam;
                    }
                }
            }
            return TutorialType.None;
        }

        private void CheckShowTutObstacle(string tutorial)
        {
            PlayerPrefs.SetInt($"{tutorial}Showed", 1);
            // UIFlowController.isShowedTut = true;
            // SonatUtils.DelayCall(1f, () => { ShowPopupTutorial<PopupTutNewMode>(tutorial).Forget(); });
        }

        private async UniTask ShowPopupTutorial<T>(string tutorial) where T : Panel
        {
            PanelManager.Instance.OpenPanelByName<T>(tutorial);
        }


        private async UniTask TryOpenPopupStartGameplay()
        {
            if (PlayerPrefs.GetInt("ShowPopupStartGameplay_HLW", 0) == 0)
            {
                await UniTask.Delay(1500);
                PlayerPrefs.SetInt("ShowPopupStartGameplay_HLW", 1);
                // var popupStart = PanelManager.Instance.OpenPanelByName<PopupStartGameplay_HLW>("PopupStartGameplay_HLW");
                // await UniTask.WaitUntil(() => popupStart == null || !popupStart.gameObject.activeInHierarchy);
                // await UniTask.Delay(1500);
                // var popupTut = PanelManager.Instance.OpenPanelByName<PopupTutNewMode>("PopupTutGameplayHLW");
                // await UniTask.WaitUntil(() => popupTut == null || !popupTut.gameObject.activeInHierarchy);
                // GameController.Instance.ChangeGameState(GameState.Playing);
            }
        }
    }

    public enum TutorialType
    {
        None,
        PopupTutBoosterAddPlate,
        PopupTutBoosterSpatula,
        PopupTutBoosterShuffle,
        PopupTutBoosterFoodBox,
        PopupTutOctochef_SkewerJam
    }
}