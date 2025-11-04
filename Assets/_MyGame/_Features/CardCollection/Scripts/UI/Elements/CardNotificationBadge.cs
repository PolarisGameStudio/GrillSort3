using MyGame.Modules.CardCollection;
using Sirenix.OdinInspector;
using SonatFramework.Systems;
using TMPro;
using UnityEngine;

public class CardNotificationBadge : MonoBehaviour
{
    private readonly Service<CardCollectionService> _cardCollectionService = new();
    [SerializeField] private bool isAllAlbum = true;
    [SerializeField, ReadOnly, ShowIf("@!isAllAlbum")] private AlbumType albumType = AlbumType.None;
    [SerializeField] private GameObject notificationObj;
    [SerializeField] private TMP_Text txtCount;

    private void OnEnable()
    {
        _cardCollectionService.Instance.CardInventoryModule.OnChangeNewCard += OnChangeNewCard;

        if (isAllAlbum || albumType != AlbumType.None)
        {
            UpdateData();
        }
    }

    private void OnDisable()
    {
        _cardCollectionService.Instance.CardInventoryModule.OnChangeNewCard -= OnChangeNewCard;
    }

    private void OnChangeNewCard(CardType cardType, AlbumType albumType)
    {
        if (isAllAlbum || albumType == this.albumType)
        {
            UpdateData();
        }
    }

    public void SetData(AlbumType albumType)
    {
        this.albumType = albumType;
        UpdateData();
    }

    public void UpdateData()
    {
        var cardInventoryModule = _cardCollectionService.Instance.CardInventoryModule;
        if (isAllAlbum)
        {
            var numCard = cardInventoryModule.GetTotalNewCards();
            txtCount.text = numCard.ToString();
            notificationObj.SetActive(numCard > 0);
        }
        else
        {
            var numCard = cardInventoryModule.GetNumberNewCardInAlbum(albumType);
            txtCount.text = numCard.ToString();
            notificationObj.SetActive(numCard > 0);
        }
    }
}
