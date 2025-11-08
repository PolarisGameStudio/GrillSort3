using MyGame.Modules.UI.LoopScroll;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SonatFramework.Systems;
using SonatFramework.Scripts.Helper;
using MyGame.Modules.CardCollection;

namespace MyGame.Modules.Language.UI
{
    public class UILanguageItem : ItemViewBase<LanguageData>
    {
        [SerializeField] public ELanguage language;
        [SerializeField] Image imgSelected;
        [SerializeField] TMP_Text text;

        private ELanguage _language;

        private readonly Service<LanguageService> _languageService = new();

        #region Listeners
        private void OnEnable()
        {
            _languageService.Instance.OnLanguageChanged += OnLanguageChanged;
        }
        private void OnDisable()
        {
            _languageService.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
        private void OnLanguageChanged()
        {
            if (_language == _languageService.Instance.CurrentLanguage)
            {
                Selected();
            }
            else
            {
                Unselected();
            }
        }
        #endregion

        public override void Bind(LanguageData data)
        {
            _language = data.language;

            text.SetLocalize(_language.ToString());
            if (_language == _languageService.Instance.CurrentLanguage)
            {
                Selected();
            }
            else
            {
                Unselected();
            }
        }

        public void Unselected()
        {
            if (imgSelected != null)
            {
                imgSelected.gameObject.SetActive(false);
            }
        }

        public void OnClickChangeLanguage()
        {
            Selected();
            _languageService.Instance.ChangeLanguage(language);
        }

        public void Selected()
        {
            if (imgSelected != null)
            {
                imgSelected.gameObject.SetActive(true);
            }
        }

        // #if UNITY_EDITOR
        //         private void OnValidate()
        //         {
        //             text.text = PopupLanguage.txtLanguages[(int)language];
        //         }
        // #endif
    }

    public class LanguageData : ItemData
    {
        public ELanguage language;
    }
}