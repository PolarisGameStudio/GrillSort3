using MyGame.SkewerJam.Gameplay;
using SonatFramework.Scripts.UIModule;
using UnityEngine;

namespace SkewerJam.UI.Elements
{
    public class UIButtonPlay : MonoBehaviour
    {
        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        public void PlayClick()
        {
            PanelManager.Instance.ClosePanel<PopupLose_SkewerJam>();

            GameController.Instance.Replay();
            // if (GameplayHelper.CheckStart() || forcePlay || PlayerPrefs.GetInt("FirstPlayHLW", 0) == 0)
            // {
            // PlayerPrefs.SetInt("FirstPlayHLW", 1);
            // MySonatFramework.GetService<SceneService>().SwitchScene(GamePlacement.Gameplay_SkewerJam);
            // }
            // else
            // {
            //     // PanelManager.Instance.OpenPanel<PopupWarningEnergy_SkewerJam>(new UIData().Add("GamePlacement", GamePlacement.Home));
            //     PanelManager.Instance.OpenPanel<PopupTrickOrTreat_HLW>();
            // }
        }
    }
}
