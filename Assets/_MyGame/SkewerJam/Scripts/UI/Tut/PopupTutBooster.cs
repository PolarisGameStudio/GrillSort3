using DG.Tweening.Plugins.Options;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.UIModule.SpriteService;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems.BoosterManagement;
using TMPro;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Tut
{
    public class PopupTutBooster : PopupTutorials
    {
        [SerializeField] private FixedImageRatio icon;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtDescription;

        public override void Open(UIData uiData)
        {
            base.Open(uiData);

            if (uiData.TryGet("BoosterType", out GameResource boosterType))
            {
                SetLayout(boosterType);
            }
        }

        private void SetLayout(GameResource boosterType)
        {
            switch (boosterType)
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
                    txtName.text = "Shuffle";
                    txtDescription.text = "Shuffle all items";
                    break;
                case GameResource.BoosterFoodBox:
                    txtName.text = "Food Box";
                    txtDescription.text = "Clear all plates";
                    break;
            }

            icon.SetSprite(MySonatFramework.GetService<SpriteAtlasService>().GetSprite($"ico_{boosterType}"));
        }
    }
}