using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.SpriteService;

public class PopupTutorialBooster : PopupTutorial
{
    public const string GAME_RESOURCE_KEY = "GAME_RESOURCE_KEY";

    protected override void SetIcon()
    {
        if (uiData.TryGet(GAME_RESOURCE_KEY, out GameResource gameResource))
        {
            icon.SetSprite(MySonatFramework.GetService<SpriteAtlasService>().GetSprite($"ico_{gameResource}"));
        }
    }
}