using SonatFramework.Scripts.UIModule.UIElements;
using TMPro;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class GameplayScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtLevel;
        [SerializeField] private UICurrency[] currencies;

        public void InitLevel(int level)
        {
            txtLevel.text = $"Level {level}";
            foreach (var currency in currencies)
            {
                currency.gameObject.SetActive(true);
                currency.UpdateValueView(false);
            }
        }

        public void HideCurrencies()
        {
            foreach (var currency in currencies)
            {
                currency.gameObject.SetActive(false);
            }
        }
    }
}
