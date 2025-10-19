using SonatFramework.Scripts.Utils;
using UnityEngine;

namespace MyGame.SkewerJam.Pack
{
    public class PackSwitcher : MonoBehaviour
    {
        [SerializeField] private PackSchedular[] packs;
        [SerializeField] private GameObject hideableObject;

        private void OnEnable()
        {
            SonatUtils.ExecuteNextFrame(() =>
            {
                CheckActivePack();
            });
        }

        public void CheckActivePack()
        {
            HideAllPacks();
            // Hiển thị gói đầu tiên khả dụng
            foreach (var pack in packs)
            {
                if (pack.IsPurchasedToday == false || pack.UnlimitedPurchases == true)
                {
                    pack.gameObject.SetActive(true);
                    return;
                }
            }

            // bị ẩn
            hideableObject.SetActive(false);
        }

        private void HideAllPacks()
        {
            foreach (var pack in packs)
            {
                pack.gameObject.SetActive(false);
            }
        }
    }
}
