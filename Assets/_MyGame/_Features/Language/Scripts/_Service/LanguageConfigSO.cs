using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Modules.Language
{
    [CreateAssetMenu(fileName = "LanguageConfigSO", menuName = "MyGame/SkewerJam/Features/Language/LanguageConfigSO")]
    public class LanguageConfigSO : MyServiceConfigSO
    {
        public List<LanguageData> listLanguageDatas;

        private Dictionary<ELanguage, LanguageData> _dictLanguageDatas = new();

        public LanguageData GetLanguageData(ELanguage language)
        {
            if (_dictLanguageDatas.ContainsKey(language) == false)
            {
                var languageData = listLanguageDatas.Find(e => e.language == language);
                if (languageData != null)
                {
                    _dictLanguageDatas.Add(language, languageData);
                }
            }
            return _dictLanguageDatas[language];
        }
    }

    [Serializable]
    public class LanguageData
    {
        [GUIColor(0, 1, 0)]
        public ELanguage language;
    }
}