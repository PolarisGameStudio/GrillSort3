using SonatFramework.Systems;
using TMPro;
using UnityEngine;

namespace MyGame.Modules.CardCollection.Home
{
    public class UIStarController : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtStar;

        private readonly Service<CardCollectionService> _cardCollectionService = new();

        private void OnEnable()
        {
            var starModule = _cardCollectionService.Instance.StarSubmodule;
            txtStar.text = starModule.NumberStar.ToString();
        }

        private void OnDisable()
        {

        }
    }
}