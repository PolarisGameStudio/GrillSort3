using Sonat.Enums;
using SonatFramework.Scripts.UIModule.SpriteService;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Systems.EventBus;

public class PopupTutorialBooster : PopupTutorial
{
    public const string GAME_RESOURCE_KEY = "GAME_RESOURCE_KEY";

    private GameResource gameResource;

    protected override void SetIcon()
    {
        if (uiData.TryGet(GAME_RESOURCE_KEY, out gameResource))
        {
            icon.SetSprite(MySonatFramework.GetService<SpriteAtlasService>().GetSprite($"ico_{gameResource}"));
        }
    }

    public override void Close()
    {
        base.Close();

        var boosterService = MySonatFramework.GetService<BoosterService>();
        var boosterConfig = boosterService.GetBoosterConfig(gameResource);
        var quantity = boosterConfig.defaultValue;
        EventBus<AddItemEvent>.Raise(new AddItemEvent()
        {
            resource = gameResource,
            quantity = quantity,
            position = icon.transform.position,
            collectEffect = new CollectEffectMultiple()
            {
                scale = 3f,
                radius = 0.2f
            }
        });
    }
}