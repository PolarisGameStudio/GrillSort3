using SonatFramework.Systems.UserData;
using UnityEngine;

public class TabHome : UITabBase
{
    [Space]
    [Header("Tab Home")]
    [SerializeField] private HomeWidgetManager homeWidgetManager;
    private bool firstTime = true;

    protected override void Start()
    {
        base.Start();
        // MySonatFramework.audioService.PlayMusic(GameplayController.GetBackgroundMusic(), true, 0.5f);
        // MySonatFramework.audioService.PlayMusic(Sonat.Enums.AudioId.BGM_Home_summer_Grill_sort, true, 0.5f);
        homeWidgetManager.Setup();
    }

    public override void OnShow()
    {
        base.OnShow();
        homeWidgetManager.OnFocus();
    }

    public override void OnHide()
    {
        base.OnHide();
        homeWidgetManager.OnLoseFocus();
    }

    public override void FadeIn()
    {
        if (firstTime)
        {
            firstTime = false;
            return;
        }

        base.FadeIn();
    }
}
