using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Templates.UI.ScriptBase;
using TMPro;
using UnityEngine;

public class PopupBuyBooster : PopupBuyBoosterBase
{
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDescription;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        switch (boosterConfig.booster)
        {
            case GameResource.BoosterAddPlate:
                txtName.text = "Add Plate";
                txtDescription.text = "Add one plate";
                break;
            case GameResource.BoosterSpatula:
                txtName.text = "Spatula";
                txtDescription.text = "Finish an order faster";
                break;
            case GameResource.BoosterShuffle:
                txtName.text = "Refresh all items";
                break;
            case GameResource.BoosterFoodBox:
                txtName.text = "Food Box";
                txtDescription.text = "Clear all plates";
                break;
        }
    }

    public override void Close()
    {
        base.Close();

        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }
}