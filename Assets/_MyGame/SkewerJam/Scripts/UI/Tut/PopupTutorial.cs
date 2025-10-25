using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Utils;
using Sonat.AdsModule;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Scripts.Utils;
using TMPro;
using UnityEngine;

public abstract class PopupTutorial : Panel
{
    public const string NAME_KEY = "NAME_KEY";
    public const string DESCRIPTION_KEY = "DESCRIPTION_KEY";

    [SerializeField] private TextGroup txtName;
    [SerializeField] private TMP_Text[] txtDescription;
    [SerializeField] protected FixedImageRatio icon;

    [Space]
    [SerializeField] private float delayClickClose = 3;
    [SerializeField] private float delayClose = 10;

    protected bool canClickClose = false;
    protected bool clicked = false;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (uiData.TryGet(NAME_KEY, out string name))
        {
            txtName.SetText(name);
        }
        if (uiData.TryGet(DESCRIPTION_KEY, out string[] descriptions))
        {
            if (descriptions.Length > txtDescription.Length)
            {
                Debug.LogError($"PopupTutorial: {name} has more descriptions than text description");
                return;
            }
            for (int i = 0; i < txtDescription.Length; i++)
            {
                txtDescription[i].text = descriptions[i];
            }
        }

        SetIcon();

        SonatUtils.DelayCall(delayClose, Close, this);

        canClickClose = false;
        clicked = false;
        SonatUtils.DelayCall(delayClickClose, () =>
        {
            canClickClose = true;
        }, this);
    }

    public virtual void OnClickClose()
    {
        if (canClickClose && clicked == false)
        {
            clicked = true;
            Close();
        }
    }

    public override void Close()
    {
        base.Close();
        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }

    protected abstract void SetIcon();
}