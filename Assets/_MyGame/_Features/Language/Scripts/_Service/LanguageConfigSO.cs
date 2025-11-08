using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Modules.Language
{
    [CreateAssetMenu(fileName = "LanguageConfigSO", menuName = "MyGame/SkewerJam/Features/Language/LanguageConfigSO")]
    public class LanguageConfigSO : ScriptableObject
    {
        public List<ELanguage> listLanguages;
    }
}