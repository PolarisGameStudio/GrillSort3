using SonatFramework.Systems.EventBus;
using UnityEngine;

namespace MyGame.Modules.SubInventory
{
    public struct AddSubItemEvent : IEvent
    {
        public SubGameResource resource;
        public int quantity;
        public Vector3 position;
        public SonatCollectEffect collectEffect;
    }

    public struct ReduceSubItemEvent : IEvent
    {
        public SubGameResource resource;
        public int quantity;
        public Vector3 position;
        public SonatCollectEffect collectEffect;
    }

    public struct ForceEffectSubItemEvent : IEvent
    {
        public SubGameResource resource;
        public int quantity;
        public Vector3 position;
        public SonatCollectEffect collectEffect;
    }
}