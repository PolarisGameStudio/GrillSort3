using MyGame.Modules.Language.UI;
using SonatFramework.Scripts.UIModule;
using UnityEngine;

public class PopupLanguage : Panel
{
    [SerializeField] private UILanguageScroll uiLanguageScroll;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);
    }
}
