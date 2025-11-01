using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyGame.Modules.CardCollection;
using SkewerJam.Utils.PageSliderPack;
using SonatFramework.Systems;
using TMPro;
using TS.PageSlider;
using UnityEngine;

public class UIAlbumNavigator : MonoBehaviour
{
    [SerializeField] private CustomPageSlider pageSlider;
    [SerializeField] private TMP_Text txtAlbumIndex;
    [SerializeField] private UIDetailAlbum[] detailAlbums;
    [SerializeField] private float delaySetupDetailAlbums = 0.1f;
    private readonly Service<CardCollectionService> _cardCollectionService = new();

    private int maxAlbum => _cardCollectionService.Instance.config.albums.Count;
    private int currentIndex;

    void OnEnable()
    {
        pageSlider.OnPageChanged.AddListener(OnPageChanged);
    }

    void OnDisable()
    {
        pageSlider.OnPageChanged.RemoveListener(OnPageChanged);
    }

    public async UniTask Setup(AlbumType albumType)
    {
        //Debug.Log($"anhnt: albumType {albumType}");
        currentIndex = (int)albumType;
        txtAlbumIndex.text = $"{currentIndex + 1}/{maxAlbum}";

// tránh lỗi page slider lúc đầu
        detailAlbums[0].Setup((AlbumType)currentIndex);
        await UniTask.Delay((int)(delaySetupDetailAlbums * 1000));
        SetupDetailAlbums();
        pageSlider.SetImmediatePage(1);
    }

    private void SetupDetailAlbums()
    {
        var prev = (AlbumType)((currentIndex - 1 + maxAlbum) % maxAlbum);
        var next = (AlbumType)((currentIndex + 1) % maxAlbum);
        //Debug.Log($"anhnt: SetupDetailAlbums prev={((int)prev)}, cur={currentIndex} ,next={((int)next)}");

        detailAlbums[0].Setup(prev);
        detailAlbums[1].Setup((AlbumType)currentIndex);
        detailAlbums[2].Setup(next);

        foreach (var detailAlbum in detailAlbums)
        {
            detailAlbum.UpdateData();
        }
    }

    public void OnPageChanged(PageContainer page)
    {
        Debug.Log($"UIAlbumNavigator: OnPageChanged {page.name}");
        if (page.TryGetComponent<UIDetailAlbum>(out var detailAlbum))
        {
            currentIndex = (int)detailAlbum.AlbumType;
            txtAlbumIndex.text = $"{currentIndex + 1}/{maxAlbum}";

            SetupDetailAlbums();
            pageSlider.SetImmediatePage(1);
        }
    }

    public void OnPrevButtonClicked()
    {
        pageSlider.ScrollToPreviousPage();
    }

    public void OnNextButtonClicked()
    {
        pageSlider.ScrollToNextPage();
    }

    public void SeeNewCard()
    {
        detailAlbums[1].SeeNewCard();
    }
}