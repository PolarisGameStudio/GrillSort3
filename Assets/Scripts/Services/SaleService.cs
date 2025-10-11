using Sonat.Enums;
using SonatFramework.Systems;
using UnityEngine;

namespace SonatFramework.Scripts.Feature.Shop
{
    [CreateAssetMenu(fileName = "SaleService", menuName = "MyGame/Services/Sale Service")]
    public class SaleService : SonatServiceSo, IServiceInitialize
    {
        public void Initialize()
        {
            return;
        }

        public bool VerifyPack(ShopItemKey shopItemKey)
        {
            return true;
        }
    }

}
