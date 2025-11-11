using MyGame.Modules.UI.LoopScroll;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SonatFramework.Systems;
using SonatFramework.Scripts.Helper;

namespace MyGame.Modules.Language.UI
{
    public class UILanguageItem : ItemViewBase<LanguageData>
    {
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
            UpdateUI();
        }
        #endregion

        public override void Bind(LanguageData data)
        {
            _language = data.language;

            UpdateUI();
        }

        private void UpdateUI()
        {
            text.text = _languageService.Instance.GetConfig().GetLanguageData(_language).languageString;
            text.font = _languageService.Instance.GetConfig().GetLanguageData(_language).font;
            // text.fontSharedMaterial = _languageService.Instance.GetConfig().GetLanguageData(_language).material;
            if (_language == _languageService.Instance.CurrentLanguage)
            {
                Selected();
            }
            else
            {
                Unselected();
            }
        }

        private void Selected()
        {
            if (imgSelected != null)
            {
                imgSelected.gameObject.SetActive(true);
            }
        }

        private void Unselected()
        {
            if (imgSelected != null)
            {
                imgSelected.gameObject.SetActive(false);
            }
        }

        public void OnClickChangeLanguage()
        {
            _languageService.Instance.ChangeLanguage(_language);
        }
    }

    public class LanguageData : ItemData
    {
        public ELanguage language;
    }
}