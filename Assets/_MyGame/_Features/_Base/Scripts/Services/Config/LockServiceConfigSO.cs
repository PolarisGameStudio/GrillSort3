using UnityEngine;

namespace MyGame.Modules
{
    public abstract class LockServiceConfigSO : MyServiceConfigSO
    {
        [Space(10)]
        [Header("Config unlock")]
        public int unlockLevel;
        public int orderTutorial = 0;
    }
}