using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDifficultyController : MonoBehaviour
{
    [Serializable]
    public class DifficultyAndObjects
    {
        public LevelDifficulty difficulty;
        public List<GameObject> objects;
    }

    [SerializeField] private string pathPrefix = "Assets/_MyGame/SkewerJam/Arts/Gameplay/Difficulty/";
    [SerializeField] private List<DifficultyAndObjects> listDifficultyAndObjects;
    [SerializeField] private Image imgProgressBar;
    [SerializeField] private TMP_Text txt;
    [SerializeField] private bool playOnAwake = true;

    public static Action OnUpdateDifficulty;

    private void OnEnable()
    {
        if (playOnAwake)
        {
            UpdateDifficultyAsync();
        }
        // levelStartedEvent = new EventBinding<LevelStartedEvent>(OnLevelStarted);

        OnUpdateDifficulty += UpdateDifficulty;

    }

    private void OnDisable()
    {
        OnUpdateDifficulty -= UpdateDifficulty;
    }

    private void UpdateDifficulty()
    {
        UpdateDifficultyAsync();
    }

    public async UniTask UpdateDifficultyAsync(bool eventUpdate = false)
    {
        if (eventUpdate == true)
        {
            OnUpdateDifficulty?.Invoke();
        }

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

        UpdateObjects(difficulty);
    }

    private void UpdateObjects(LevelDifficulty difficulty)
    {
        foreach (var difficultyAndObjects in listDifficultyAndObjects)
        {
            foreach (var obj in difficultyAndObjects.objects)
            {
                obj.SetActive(difficultyAndObjects.difficulty == difficulty);
            }
        }
    }
}
