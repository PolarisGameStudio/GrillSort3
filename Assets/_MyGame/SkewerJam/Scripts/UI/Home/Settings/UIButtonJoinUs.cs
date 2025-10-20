using System.Collections;
using System.Collections.Generic;
using SonatFramework.Scripts.Helper;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Home.Settings
{
    public class UIButtonJoinUs : MonoBehaviour
    {
        private const string UI_BUTTON_JOIN_US_KEY = "UI_BUTTON_JOIN_US_KEY";
        [SerializeField] private GameObject warningObj;

        private bool canShowWarning
        {
            get
            {
                return PlayerPrefs.GetInt(UI_BUTTON_JOIN_US_KEY + "_canShowWarning", 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(UI_BUTTON_JOIN_US_KEY + "_canShowWarning", value ? 1 : 0);
            }
        }

        private void OnEnable()
        {
            warningObj.SetActive(canShowWarning);
        }

        public void OnClickHiddenWarning()
        {
            warningObj.SetActive(false);
            canShowWarning = false;
        }
    }
}
