using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Modules.CardCollection
{
    public class AlbumBackground : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image borderImage;
        // [SerializeField] private Image exitImage;

        public async UniTask Setup(AlbumConfigSO albumConfig)
        {
            backgroundImage.SetSpriteAsync(albumConfig.GetAlbumBackgroundSpritePath()).Forget();
            borderImage.SetSpriteAsync(albumConfig.GetAlbumBorderSpritePath()).Forget();
            // exitImage.sprite = albumConfig.albumExit;
        }
    }
}