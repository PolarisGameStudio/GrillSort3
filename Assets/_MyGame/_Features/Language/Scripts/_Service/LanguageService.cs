using UnityEngine;
using I2.Loc;
using SonatFramework.Scripts.Utils;
using System;

namespace MyGame.Modules.Language
{
    [CreateAssetMenu(fileName = "LanguageService", menuName = "MyGame/SkewerJam/Features/Language/LanguageService")]
    public class LanguageService : MyService<LanguageConfigSO>
    {
        public override string DATA_KEY => "LANGUAGE_SERVICE";
        public ELanguage CurrentLanguage => LocalizationManager.CurrentLanguage.ToEnum<ELanguage>();

        public event Action OnLanguageChanged;

        #region Override
        protected override void LoadConfig()
        {
            // do nothing
        }

        protected override void ResetData()
        {
            // do nothing
        }

        protected override void LoadData()
        {
            LocalizationManager.CurrentLanguage = CurrentLanguage.ToString();
        }
        #endregion

        public void ChangeLanguage(ELanguage language)
        {
            if (LocalizationManager.HasLanguage(language.ToString()))
            {
                LocalizationManager.CurrentLanguage = language.ToString();
                OnLanguageChanged?.Invoke();
            }
        }
    }
}

