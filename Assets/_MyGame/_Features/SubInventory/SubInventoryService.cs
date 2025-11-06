using System;
using System.Collections.Generic;
using System.Linq;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.GameDataManagement;
using UnityEngine;

namespace MyGame.Modules.SubInventory
{
    [CreateAssetMenu(fileName = "SubInventoryService", menuName = "MyGame/SkewerJam/Features/SubInventory/SubInventoryService")]
    public class SubInventoryService : SonatServiceSo, IServiceInitialize
    {
        protected const string SubInventoryPrefixKey = "SUB_INVENTORY_";
        [SerializeField] protected Service<DataService> dataService = new();


        private readonly Dictionary<SubGameResource, int> lastResources = new();
        private readonly Dictionary<SubGameResource, int> currentResources = new();


        public event Action<SubGameResource> OnResourceUpdate;


        public void Initialize()
        {
            new EventBinding<AddSubItemEvent>(OnAddSubItem);
            new EventBinding<ReduceSubItemEvent>(OnReduceSubItem);
        }

        public int GetResource(SubGameResource resource)
        {
            if (currentResources.TryGetValue(resource, out var value)) return value;
            value = dataService.Instance.GetInt($"{SubInventoryPrefixKey}{resource}", 0);
            currentResources.Add(resource, value);
            return value;
        }

        public int SetResource(SubGameResource resource, int value)
        {
            dataService.Instance.SetInt($"{SubInventoryPrefixKey}{resource}", value);
            if (!currentResources.TryAdd(resource, value))
            {
                currentResources[resource] = value;
            }

            return value;
        }


        public int GetResourceView(SubGameResource resource)
        {
            if (lastResources.TryGetValue(resource, out var value))
            {
                return value;
            }

            value = GetResource(resource);
            lastResources.Add(resource, value);
            return value;
        }

        public int AddResource(SubGameResource resource, int value, bool noti = false)
        {
            int lastResource = GetResource(resource);
            int curr = lastResource + value;
            SetResource(resource, curr);
            if (noti)
            {
                NotiUpdateResource(resource);
            }
            else
            {
                SetLastResource(resource, lastResource);
            }

            return curr;
        }

        public int ReduceResource(SubGameResource resource, int value, bool noti = false)
        {
            int lastResource = GetResource(resource);
            int curr = lastResource - value;
            SetResource(resource, curr);
            if (noti)
            {
                NotiUpdateResource(resource);
            }
            else
            {
                SetLastResource(resource, lastResource);
            }

            return curr;
        }

        public bool CanReduce(SubGameResource resource, int quantity)
        {
            return GetResource(resource) >= quantity;
        }


        private void SetLastResource(SubGameResource resource, int value)
        {
            if (!lastResources.TryAdd(resource, value))
            {
                lastResources[resource] = value;
            }
        }

        private void OnAddSubItem(AddSubItemEvent addEvent)
        {
            NotiUpdateResource(addEvent.resource);
        }

        private void OnReduceSubItem(ReduceSubItemEvent reduceEvent)
        {
            NotiUpdateResource(reduceEvent.resource);
        }

        public void NotiUpdateResource(SubGameResource itemName)
        {
            if (itemName == SubGameResource.MAX)
            {
                if (lastResources != null)
                {
                    var keys = lastResources.Keys.ToList();
                    foreach (var key in keys)
                    {
                        SetLastResource(key, GetResource(key));
                        OnResourceUpdate?.Invoke(key);
                    }
                }
            }
            else
            {
                SetLastResource(itemName, GetResource(itemName));
                OnResourceUpdate?.Invoke(itemName);
            }

        }

    }
}