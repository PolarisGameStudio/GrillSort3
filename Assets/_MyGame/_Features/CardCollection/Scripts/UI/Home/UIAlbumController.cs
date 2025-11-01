using System.Collections.Generic;
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
            if (_isInit) return;
            _isInit = true;
            _poolingContainerService.Instance.CleanContainer(container);
            foreach (var album in _cardCollectionService.Instance.config.albums)
            {
                var albumObj = _poolingContainerService.Instance.CreateObject<UIAlbum>(container);
                albumObj.Setup(album.type);
                albums.Add(albumObj);
            }
        }

        public void UpdateData()
        {
            foreach (var album in albums)
            {
                album.UpdateData();
            }
        }
    }
}
