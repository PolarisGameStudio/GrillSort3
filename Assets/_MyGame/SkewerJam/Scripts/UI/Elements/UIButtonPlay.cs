using System;
using MyGame.SkewerJam.Gameplay;
using Sonat;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.Lives;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.SceneManagement;
using UnityEngine;

namespace SkewerJam.UI.Elements
{
    public class UIButtonPlay : MonoBehaviour
    {
        private readonly Service<LivesService> livesService = new();

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        public void PlayClick()
        {
            if (livesService.Instance.CanPlay())
            {
                MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Gameplay_SkewerJam);
            }
            else
            {
                PanelManager.Instance.OpenPanel<PopupRefillLives>();
                PopupToast.Cretate("No more lives left!");
            }
        }
    }
}
