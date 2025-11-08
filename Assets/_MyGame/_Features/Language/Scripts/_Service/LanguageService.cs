using UnityEngine;
using SonatFramework.Systems;
using I2.Loc;

namespace MyGame.Modules.Language
{
    [CreateAssetMenu(fileName = "LanguageService", menuName = "MyGame/SkewerJam/Features/Language/LanguageService")]
    public class LanguageService : SonatServiceSo, IServiceInitialize
    {
        [SerializeField] private LanguageConfigSO languageConfig;

        public void Initialize()
        {

        }

        public void ChangeLanguage(ELanguage language)
        {
            if (LocalizationManager.HasLanguage(language.ToString()))
            {
                LocalizationManager.CurrentLanguage = language.ToString();
            }
        }
    }
}

