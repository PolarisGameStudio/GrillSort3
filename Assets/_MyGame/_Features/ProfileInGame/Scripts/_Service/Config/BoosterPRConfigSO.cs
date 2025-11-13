using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Config
{
    [CreateAssetMenu(fileName = "BoosterPRConfigSO", menuName = "MyGame/Features/ProfileInGame/BoosterPRConfigSO")]
    public class BoosterPRConfigSO : ScriptableObject
    {
        public List<BoosterPR> listBoosterPRs;

        #region OnValidate
        public void OnValidate()
        {
            foreach (var boosterPR in listBoosterPRs)
            {
                if (GameResourceHelper.ResourceType(boosterPR.booster) != GameResourceType.Booster)
                {
                    Debug.LogError($"BoosterPRConfig: Booster {boosterPR.booster} is not a booster");
                }
                if (boosterPR.PR > 0)
                {
                    Debug.LogError($"BoosterPRConfig: PR is greater than 0");
                }
            }
        }
        #endregion
        public int GetPRByBooster(GameResource key)
        {
            foreach (var boosterPR in listBoosterPRs)
            {
                if (boosterPR.booster == key)
                {
                    return boosterPR.PR;
                }
            }
            return 0;
        }
    }

    [Serializable]
    public class BoosterPR
    {
        public GameResource booster;
        [GUIColor(0f, 1f, 0f)]
        public int PR;
    }
}