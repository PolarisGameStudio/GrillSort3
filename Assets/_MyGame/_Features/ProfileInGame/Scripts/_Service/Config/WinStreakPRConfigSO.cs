using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Config
{
    [CreateAssetMenu(fileName = "WinStreakPRConfigSO", menuName = "MyGame/Features/ProfileInGame/WinStreakPRConfigSO")]
    public class WinStreakPRConfigSO : ScriptableObject
    {
        public List<WinStreakPR> listWinStreakPRs;

        #region OnValidate
        public void OnValidate()
        {
            for (int i = 0; i < listWinStreakPRs.Count - 1; i++)
            {
                if (listWinStreakPRs[i].winStreak >= listWinStreakPRs[i + 1].winStreak)
                {
                    Debug.LogError($"WinStreakPRConfig: winStreak {listWinStreakPRs[i].winStreak} is greater than winStreak {listWinStreakPRs[i + 1].winStreak}");
                }
            }
        }
        #endregion

        public int GetPRByWinStreak(int winStreak)
        {
            winStreak = winStreak < 0 ? 0 : winStreak;

            foreach (var winStreakPR in listWinStreakPRs)
            {
                if (winStreak >= winStreakPR.winStreak)
                {
                    return winStreakPR.PR;
                }
            }
            return 0;
        }
    }

    [Serializable]
    public class WinStreakPR
    {
        public int winStreak;
        [GUIColor(0f, 1f, 0f)]
        public int PR;
    }
}