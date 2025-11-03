using System;
using System.Collections.Generic;
using Sonat.Enums;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "AlbumConfigSO", menuName = "MyGame/SkewerJam/Features/CardCollection/AlbumConfigSO")]
    public class AlbumConfigSO : ScriptableObject
    {
        public AlbumType type;
        public string albumName;
        public RewardData reward;

        [Space]
        [Header("Cards")]
        public List<CardType> cards;

        [Space]
        [Header("Text Color")]
        public TextColorType color;

        [Space]
        [Header("Path")]
        public string albumNamePath = "Assets/_MyGame/_Features/CardCollection/Arts/Albums";

        [Space]
        [Header("Background Popup")]
        public string backgroundPopupPath = "Assets/_MyGame/_Features/CardCollection/Arts/BackgroundPopup";

        public string GetAlbumSpritePath()
        {
            return $"{albumNamePath}/{type.ToString().ToLower()}.png";
        }

        #region Background Popup
        public string GetAlbumBackgroundSpritePath()
        {
            return $"{backgroundPopupPath}/{type.ToString().ToLower()}_board.png";
        }

        public string GetAlbumBorderSpritePath()
        {
            return $"{backgroundPopupPath}/{type.ToString().ToLower()}_bg.png";
        }
        #endregion
        //#if UNITY_EDITOR
        //        private void OnValidate()
        //        {
        //            albumName = type.ToString();
        //        }
        //#endif
    }

    [System.Serializable]
    public enum AlbumType
    {
        None = -1,
        Album_0 = 0,
        Album_1 = 1,
        Album_2 = 2,
        Album_3 = 3,
        Album_4 = 4,
        Album_5 = 5,
        Album_6 = 6,
        Album_7 = 7,
        Album_8 = 8,
        Album_9 = 9
    }

}