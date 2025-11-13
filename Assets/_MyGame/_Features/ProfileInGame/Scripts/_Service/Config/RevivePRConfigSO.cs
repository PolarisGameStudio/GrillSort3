using System;
using UnityEngine;

namespace MyGame.Modules.ProfileInGame.Config
{
    [CreateAssetMenu(fileName = "RevivePRConfigSO", menuName = "MyGame/Features/ProfileInGame/RevivePRConfigSO")]
    public class RevivePRConfigSO : ScriptableObject
    {
        public int PR;

        #region OnValidate
        public void OnValidate()
        {
            if (PR > 0)
            {
                Debug.LogError($"RevivePRConfigSO: PR is greater than 0");
            }
        }
        #endregion

        public int GetPR()
        {
            return PR;
        }
    }
}