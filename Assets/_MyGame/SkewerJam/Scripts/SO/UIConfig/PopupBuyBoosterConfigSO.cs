using System;
using System.Collections.Generic;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Scripts.SO.UIConfig
{
    [CreateAssetMenu(fileName = "PopupBuyBoosterConfigSO", menuName = "MyGame/SkewerJam/UIConfig/PopupBuyBoosterConfigSO")]
    public class PopupBuyBoosterConfigSO : ScriptableObject
    {
        public List<BoosterConfigSO> boostersConfigSO;

        public (string title, string description) GetBoosterConfig(GameResource booster)
        {
            var boosterConfig = boostersConfigSO.Find(x => x.booster == booster);
            return (boosterConfig.name, boosterConfig.description);
        }
    }

    [Serializable]
    public class BoosterConfigSO
    {
        public GameResource booster;
        public string name;
        public string description;
    }
}