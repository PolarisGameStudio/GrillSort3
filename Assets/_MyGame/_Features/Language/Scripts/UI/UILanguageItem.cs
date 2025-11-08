using MyGame.Modules.UI.LoopScroll;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.Language.UI
{
    public class UILanguageItem : ItemViewBase<LanguageData>
    {
        [SerializeField] public ELanguage language;
        [SerializeField] Image imgSelected;
        [SerializeField] TMP_Text text;
        void Awake()
        {
            if (imgSelected != null)
            {
                imgSelected.gameObject.SetActive(false);
            }
        }

        public override void Bind(LanguageData data)
        {


        }

        public void Unselected()
        {
            if (imgSelected != null)
            {
                imgSelected.gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            Selected();
            MySonatFramework.GetService<LanguageService>().ChangeLanguage(language);
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
        public string languageName;
        public string languageCode;
    }
}