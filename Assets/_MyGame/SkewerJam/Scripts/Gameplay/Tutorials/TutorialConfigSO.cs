using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    [CreateAssetMenu(fileName = "TutorialConfigSO", menuName = "MyGame/SkewerJam/Gameplay/Tutorials/TutorialConfigSO")]
    public class TutorialConfigSO : ScriptableObject
    {
        public List<TutorialData> tutorialDatas;
    }

    [Serializable]
    public class TutorialData
    {
        public TutorialType tutorialType;
        public string name;
        public string[] listDescription;
    }
}