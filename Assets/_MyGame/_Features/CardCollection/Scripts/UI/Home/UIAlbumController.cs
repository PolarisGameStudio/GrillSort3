using System.Collections.Generic;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

namespace MyGame.Modules.CardCollection.Home
{
    public class UIAlbumController : MonoBehaviour
    {
        [SerializeField] private Transform container;
        private readonly Service<PoolingContainerService> _poolingContainerService = new();
        private readonly Service<CardCollectionService> _cardCollectionService = new();
        private List<UIAlbum> albums = new();

        private bool _isInit = false;

        void OnEnable()
        {
            if (_isInit == false)
            {
                _isInit = true;
                _poolingContainerService.Instance.CleanContainer(container);
                foreach (var album in _cardCollectionService.Instance.config.albums)
                {
                    var albumObj = _poolingContainerService.Instance.CreateObject<UIAlbum>(container);
                    albumObj.Setup(album.type);
                    albums.Add(albumObj);
                }
            }

            _cardCollectionService.Instance.CardInventoryModule.OnCollectCard += OnCollectCard;
            SonatUtils.ExecuteNextFrame(() =>
            {
                foreach (var album in albums)
                {
                    album.UpdateData();
                }
            });

        }

        void OnDisable()
        {
            _cardCollectionService.Instance.CardInventoryModule.OnCollectCard -= OnCollectCard;
        }

        private void OnCollectCard(CardType cardType, AlbumType albumType)
        {
            foreach (var album in albums)
            {
                if (album.AlbumType == albumType)
                {
                    album.UpdateData();
                }
            }
        }
    }
}
