using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDifficultyController : MonoBehaviour
{
    [SerializeField] private string pathPrefix = "Assets/_MyGame/SkewerJam/Arts/Gameplay/Difficulty/";
    [SerializeField] private Image imgProgressBar;
    [SerializeField] private TMP_Text txt;
    [SerializeField] private bool playOnAwake = true;

    private EventBinding<LevelStartedEvent> levelStartedEvent;

    private void OnEnable()
    {
        if (playOnAwake)
        {
            UpdateDifficulty();
        }
        // levelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);

    }

    private void OnDisable()
    {
        // EventBus<LevelStartedEvent>.Deregister(levelStartedEvent);
    }

    // private void OnLevelStarted(LevelStartedEvent eventData)
    // {
    //     UpdateDifficulty();
    // }

    public async UniTask UpdateDifficulty()
    {
        var difficulty = await MySonatFramework.GetLevelDifficulty();

        var subfix = "";
        var term = "1FTVVIPMikado-Black SDF-Title-Blue";
        switch (difficulty)
        {
            case LevelDifficulty.Hard:
                subfix = "hard";
                term = "1FTVVIPMikado-Black SDF-Title-Red";
                break;
            case LevelDifficulty.SuperHard:
                subfix = "superhard";
                term = "1FTVVIPMikado-Black SDF-Title-Purple";
                break;
        }
        var path = $"{pathPrefix}{subfix}.png";

        if (imgProgressBar != null)
        {
            imgProgressBar.SetSpriteAsync(path);
        }

        if (txt != null)
        {
            txt.SetSecondaryTerm(term);
        }
    }
}
