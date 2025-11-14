using Gameplay.Entities.GrillScripts;
using Gameplay.LevelData;
using UnityEngine;

public class GrillVisualShutter : GrillVisual
{
    [SerializeField] private Animator shutterAnim;

    public override void SetDefaultGrill(GrillData grillData)
    {
        base.SetDefaultGrill(grillData);

        lid.gameObject.SetActive(false);
        lidSoldOut.gameObject.SetActive(false);
    }

    public override void OpenGrill(bool doEffect = true, bool isSoldOut = false)
    {
        // OpenShutter();
    }

    public override void CloseGrill(bool doEffect = true, bool isSoldOut = false)
    {
        CloseShutter();
    }

    public void SetUpShutter(bool isClosed)
    {
        if (isClosed)
        {
            CloseShutter();
        }
        else
        {
            OpenShutter();
        }
    }

    public void OpenShutter()
    {
        shutterAnim.SetTrigger("Open");
    }

    public void CloseShutter()
    {
        shutterAnim.SetTrigger("Close");
    }
}
