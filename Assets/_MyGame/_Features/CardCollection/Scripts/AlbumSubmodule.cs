using System.Collections.Generic;
using SonatFramework.Scripts.Helper;
using SonatFramework.Systems;
using SonatFramework.Systems.GameDataManagement;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "AlbumSubmodule", menuName = "MyGame/SkewerJam/Features/CardCollection/AlbumSubmodule")]
    public class AlbumSubmodule : ScriptableObject
    {
        public const string DATA_KEY = "CARD_COLLECTION_ALBUM_SUBMODULE";

        private ListDataPref<int> _completedAlbumTypes;
        private IntDataPref _numCompleteAlbum;

        public void LoadData()
        {
            _completedAlbumTypes = new ListDataPref<int>(DATA_KEY + "_completedAlbumTypes");
            _numCompleteAlbum = new IntDataPref(DATA_KEY + "_numCompleteAlbum");
        }
    }
}