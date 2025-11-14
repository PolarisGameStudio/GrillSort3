using Gameplay.LevelData;
using UnityEngine;

namespace Gameplay.Entities.ItemScripts
{
    public class ItemKeyAreaVisual : ItemVisual
    {
        [Header("Item key area visual")]
        [SerializeField] private GameObject key;


        public override void SetVisual(ItemData itemData)
        {
            base.SetVisual(itemData);
            key.SetActive(true);
        }

        public override void OnComplete()
        {
            base.OnComplete();
            key.SetActive(false);
        }

    }
}