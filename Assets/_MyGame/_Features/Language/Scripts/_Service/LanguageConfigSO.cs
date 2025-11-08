using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Modules.Language
{
    [CreateAssetMenu(fileName = "LanguageConfigSO", menuName = "MyGame/SkewerJam/Features/Language/LanguageConfigSO")]
    public class LanguageConfigSO : MyServiceConfigSO
    {
        public List<ELanguage> listLanguages;
    }
}