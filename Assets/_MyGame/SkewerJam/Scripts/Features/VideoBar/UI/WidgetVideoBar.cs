using System.Collections;
using System.Collections.Generic;
using MyGame.SkewerJam.Features.VideoBar;
using SonatFramework.Systems;
using UnityEngine;

public class WidgetVideoBar : UIHomeWidget
{
    [SerializeField] private GameObject iconWarning;

    private readonly Service<VideoBarService> videoBarService = new();
    public override void Setup()
    {
        base.Setup();

        iconWarning.SetActive(CheckActive());
    }

    public override void OnFocus()
    {
        base.OnFocus();
        iconWarning.SetActive(CheckActive());
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
    }

    private bool CheckActive()
    {
        return videoBarService.Instance.CheckFull() == false;
    }

}
