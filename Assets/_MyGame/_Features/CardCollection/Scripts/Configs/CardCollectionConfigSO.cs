using System;
using System.Collections.Generic;
using SonatFramework.Systems.ConfigManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardCollectionConfigSO", menuName = "Sonat Configs/CardCollection/CardCollectionConfigSO")]
    public class CardCollectionConfigSO : ScriptableObject
    {
        public int unlockLevel;
        // public LiveOpsPackData liveOpsPackData;

        [Header("Config expire")]
        public int durationMonth = 3;

        [Space(10)]
        [Header("Config reward")]
        public RewardData rewardInSeason;

        [Space(10)]
        public List<AlbumConfigSO> albums;
        public List<CardConfigSO> cards;
        [Space(10)]
        public RewardData RewardUnlock;

        public int GetNumCard()
        {
            return cards.Count;
        }

        public AlbumType GetAlbumType(CardType cardType)
        {
            return albums.Find(e => e.cards.Contains(cardType))?.type ?? AlbumType.None;
        }
    }
}