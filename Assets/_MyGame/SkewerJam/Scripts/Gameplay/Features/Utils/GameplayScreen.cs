using System;
using System.Linq;
using DG.Tweening;
using MyGame.SkewerJam.Features.BannerController;
using MyGame.SkewerJam.Gameplay.Booster;
using Sonat.Enums;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule.UIElements;
using TMPro;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class GameplayScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtLevel;
        [SerializeField] private TMP_Text txtTotalItems;
        [SerializeField] private UICurrency[] currencies;

        [SerializeField] private UIBooster[] uiBoosters;
        [SerializeField] private BannerController bannerController;

        public UIBooster[] UiBoosters => uiBoosters;

        private Tween _totalItemsTween;
        private int _fromValue;

        public void InitLevel(int level)
        {
            txtLevel.text = $"Level {level}";
            foreach (var currency in currencies)
            {
                currency.gameObject.SetActive(true);
                currency.UpdateValueView(false);
            }

            var itemManager = GameController.Instance.GameLogicHandler.ItemManager;
            _fromValue = 0;
            itemManager.OnUpdateItems += UpdateTotalItems;

            bannerController.gameObject.SetActive(MySonatFramework.IsShowBanner());

        }

        public void ClearLevel()
        {
            var itemManager = GameController.Instance.GameLogicHandler.ItemManager;
            itemManager.OnUpdateItems -= UpdateTotalItems;
        }

        public void HideCurrencies()
        {
            foreach (var currency in currencies)
            {
                currency.gameObject.SetActive(false);
            }
        }

        public void UpdateTotalItems(int currentItems)
        {
            var totalItems = GameController.Instance.GameLogicHandler.ItemManager.TotalItems;

            _totalItemsTween?.Kill();
            _totalItemsTween = DOTween.To(() => _fromValue, x => _fromValue = x, totalItems - currentItems, 0.1f).SetEase(Ease.InOutSine).OnUpdate(() =>
            {
                txtTotalItems.text = $"{_fromValue}/{totalItems}";
            });
        }

        public UIBooster GetUIBooster(GameResource boosterType)
        {
            return uiBoosters.FirstOrDefault(e => e.boosterType == boosterType);
        }
    }
}
