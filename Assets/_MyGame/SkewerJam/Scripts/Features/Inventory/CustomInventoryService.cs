using Sonat.Enums;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

namespace MyGame.SkewerJam.Scripts.Features.Inventory
{
    [CreateAssetMenu(fileName = "CustomInventoryService", menuName = "MyGame/SkewerJam/Features/CustomInventoryService")]
    public class CustomInventoryService : SonatInventoryService
    {
        public override int GetResource(GameResource resource)
        {
            Backup();
            return base.GetResource(resource);
        }

        private void Backup()
        {
            // if (currentResources.TryGetValue(resource, out var value)) return value;
            // value = dataService.Instance.GetInt($"{GameResourcePrefixKey}{resource}", 0);
            // currentResources.Add(resource, value);
            // return value;
            var addPlate = dataService.Instance.GetInt($"{GameResourcePrefixKey}{GameResource.BoosterAddPlate}", 0);
            if (addPlate != 0)
            {
                dataService.Instance.SetInt($"{GameResourcePrefixKey}{GameResource.BoosterUndo}", addPlate);
                dataService.Instance.SetInt($"{GameResourcePrefixKey}{GameResource.BoosterAddPlate}", 0);
            }
        }
    }
}