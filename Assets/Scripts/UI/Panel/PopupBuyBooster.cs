using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Scripts.SO.UIConfig;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.BoosterManagement;
using SonatFramework.Templates.UI.ScriptBase;
using TMPro;
using UnityEngine;

public class PopupBuyBooster : PopupBuyBoosterBase
{
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDescription;

    private Service<SonatBoosterService> boosterService = new();

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        var popupBuyBoosterConfigSO = boosterService.Instance.BoostersConfig.popupBuyBoosterConfigSO;
        var (title, description) = popupBuyBoosterConfigSO.GetBoosterConfig(boosterConfig.booster);

        txtName.text = title;
        txtDescription.text = description;

        // switch (boosterConfig.booster)
        // {
        //     case GameResource.BoosterUndo:
        //         txtName.text = "Add Plate";
        //         txtDescription.text = "Add one plate";
        //         break;
        //     case GameResource.BoosterSpatula:
        //         txtName.text = "Spatula";
        //         txtDescription.text = "Finish an order faster";
        //         break;
        //     case GameResource.BoosterShuffle:
        //         txtName.text = "Shuffle";
        //         txtDescription.text = "Shuffle all items";
        //         break;
        //     case GameResource.BoosterFoodBox:
        //         txtName.text = "Food Box";
        //         txtDescription.text = "Clear all plates";
        //         break;
        //     case GameResource.BoosterUndo_Test:
        //         txtName.text = "Undo";
        //         txtDescription.text = "Made a mistake? Undo your last move.";
        //         break;
        // }
    }

    public override void Close()
    {
        base.Close();

        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }
}