using Manager;
using MyGame.SkewerJam.Gameplay;

public class PopupTutorialObstacle : PopupTutorial
{
    public const string TUTORIAL_TYPE_KEY = "TUTORIAL_TYPE_KEY";

    protected override void SetIcon()
    {
        if (icon == null) return;
        if (uiData.TryGet(TUTORIAL_TYPE_KEY, out TutorialType tutorialType))
        {
            icon.SetSpriteAsync(PathManager.TutorialSprite(tutorialType));
        }
    }
}