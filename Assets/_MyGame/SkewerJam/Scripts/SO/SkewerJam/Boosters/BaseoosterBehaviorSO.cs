using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

namespace MyGame.SO.Boosters
{
    public abstract class BaseBoosterBehaviorSO : ScriptableObject
    {
        public abstract GameResource boosterType { get; }
        public abstract UniTask<bool> UseBooster(Vector3 position, bool isForce = false);
        public abstract UniTask<bool> ForceUseBooster();

        public virtual (bool canUse, string reason) CanUseBooster()
        {
            return (true, "");
        }


        protected virtual async UniTask PlayBoosterAnim(Vector3 position)
        {
            var boosterAnim = await MySonatFramework.GetService<PoolingServiceAsync>().CreateAsync<BoosterAnim>(
                "BoosterAnim",
                PanelManager.Instance.transform);
            boosterAnim.SetBooster(boosterType);
            boosterAnim.SetData(position);
            await UniTask.Delay(2000);
        }
    }
}