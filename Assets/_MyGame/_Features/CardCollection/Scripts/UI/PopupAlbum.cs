using MyGame.Modules.CardCollection;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

public class PopupAlbum : Panel
{
    [SerializeField] private UIAlbumNavigator albumNavigator;


    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        if (uiData.TryGet<AlbumType>("albumType", out var albumType))
        {
            albumNavigator.Setup(albumType);
        }
    }

    public override void Close()
    {
        base.Close();
        albumNavigator.SeeNewCard();
    }
}
