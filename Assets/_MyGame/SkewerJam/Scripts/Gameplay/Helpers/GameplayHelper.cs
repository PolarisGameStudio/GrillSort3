using System.Collections.Generic;
using MyGame.SkewerJam.UI.Loading;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.SceneManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public static class GameplayHelper
    {
        public static bool CheckStart()
        {
            // var energy = MySonatFramework.GetService<InventoryService>().GetResource(GameResource.Energy);
            // return energy > 0;
            return true;
        }

        public static void GoHome()
        {
            LoadingInstance.Instance.ShowLoading();
            // PanelManager.Instance.OpenPanelByName<PopupLoading>("PopupLoading_SkewerJam");
            SonatUtils.DelayCall(1.5f, () => { MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Home); });
        }

        public static void OnClose_ChangeGameState(GameState gameState)
        {
            SonatUtils.ExecuteNextFrame(() =>
            {
                EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent() { gameState = gameState });
            });
        }

        public static Vector3 GetNewWaitingGrillPosition()
        {
            var waitingGrillManager = GameController.Instance.GameLogicHandler.WaitingGrillManager;
            var waitingGrill = waitingGrillManager.ListWaitingGrills[0];
            var waitingGrill1 = waitingGrillManager.ListWaitingGrills[1];

            var distance = waitingGrill1.transform.position - waitingGrill.transform.position;
            var count = waitingGrillManager.ListWaitingGrills.Count + 1;
            var start = waitingGrillManager.transform.position - distance * (count - 1) / 2;
            return start + distance * (count - 1);
        }

        public static AudioId GetBGMIngame()
        {
            var listBGMIngame = new List<AudioId>() {
                AudioId.BGM_Ingame_Casual,
                AudioId.BGM_Ingame_Relaxing ,
                AudioId.BGM_Ingame_Summer_Grill3,
            };
            return listBGMIngame[Random.Range(0, listBGMIngame.Count)];
        }
    }
}