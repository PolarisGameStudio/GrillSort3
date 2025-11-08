using UnityEngine;
using SonatFramework.Systems;
using I2.Loc;
using SonatFramework.Scripts.Utils;

namespace MyGame.Modules.Language
{
    [CreateAssetMenu(fileName = "LanguageService", menuName = "MyGame/SkewerJam/Features/Language/LanguageService")]
    public class LanguageService : SonatServiceSo, IServiceInitialize
    {
        public LanguageConfigSO config;

        public ELanguage CurrentLanguage => LocalizationManager.CurrentLanguage.ToEnum<ELanguage>();


        public void Initialize()
        {
            LoadData();
        }

        private void LoadData()
        {
            LocalizationManager.CurrentLanguage = CurrentLanguage.ToString();
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

