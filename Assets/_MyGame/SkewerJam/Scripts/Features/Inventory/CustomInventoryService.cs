using MyGame.Modules.CardCollection;
using Sonat.Enums;
using SonatFramework.Scripts.Feature.Lives;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
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

        public override void AddReward(RewardData rewardData, EarnResourceLogData logData = null, bool noti = false)
        {
            foreach (var resourceData in rewardData.resourceDatas)
            {
                switch (resourceData.resource)
                {
                    case GameResource.Lives:
                        SonatSystem.GetService<LivesService>().AddUnlimitedLives(resourceData.quantity, logData, noti);
                        break;
                    case GameResource.LivesService_SingleLive:
                        var maxLives = SonatSystem.GetService<LivesService>().config.maxLives;
                        var currentLives = GetResource(GameResource.Lives);
                        var addLives = Mathf.Min(maxLives, currentLives + resourceData.quantity) - currentLives;
                        AddResource(GameResource.Lives, addLives, logData, noti);
                        break;
                    default:
                        if (MySonatFramework.CanReceive(resourceData.resource))
                        {
                            if (GameResourceHelper.ResourceType(resourceData.resource) == GameResourceType.Card)
                            {
                                var cardCollectionService = SonatSystem.GetService<CardCollectionService>();
                                cardCollectionService.UnboxPackCard(resourceData, noti);
                            }
                            else
                            {
                                AddResource(resourceData.resource, resourceData.quantity, logData, noti);
                            }
                        }
                        break;
                }
            }

            if (noti)
            {
                OnClaimReward?.Invoke(rewardData);
            }
        }
    }
}