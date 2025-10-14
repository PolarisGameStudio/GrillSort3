using MyGame.SkewerJam.Gameplay.Helpers;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Templates.UI.ScriptBase;
using TMPro;
using UnityEngine;

public class PopupBuyBooster : PopupBuyBoosterBase
{
    [SerializeField] private TMP_Text txtName;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        switch (boosterConfig.booster)
        {
            case GameResource.BoosterAddPlate:
                txtName.text = "Add Plate";
                break;
            case GameResource.BoosterSpatula:
                txtName.text = "Spatula";
                break;
            case GameResource.BoosterShuffle:
                txtName.text = "Shuffle";
                break;
            case GameResource.BoosterFoodBox:
                txtName.text = "Food Box";
                break;
        }
    }

    public override void Close()
    {
        base.Close();

        GameplayHelper.OnClose_ChangeGameState(GameState.Playing);
    }
}