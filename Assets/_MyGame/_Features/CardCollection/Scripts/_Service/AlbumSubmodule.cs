using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "AlbumSubmodule", menuName = "MyGame/SkewerJam/Features/CardCollection/AlbumSubmodule")]
    public class AlbumSubmodule : ScriptableObject
    {
        [SerializeField] private CardInventoryModule cardInventoryModule;




    }
}