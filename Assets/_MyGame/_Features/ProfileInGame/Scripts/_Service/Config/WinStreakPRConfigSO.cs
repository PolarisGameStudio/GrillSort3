using System;
using System.Collections.Generic;
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
            listWinStreakPRs.Sort((a, b) => a.winStreak.CompareTo(b.winStreak));
        }
        #endregion
    }

    [Serializable]
    public class WinStreakPR
    {
        public int winStreak;
        public int PR;
    }
}