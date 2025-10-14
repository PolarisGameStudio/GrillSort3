using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyGame.SO.Boosters;
using Sonat.Enums;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class BoosterManager : MonoBehaviour
    {
        private const string LOG_TAG = "<color=yellow>BoosterLogicHandler: </color>";
        [SerializeField] private BaseBoosterBehaviorSO[] boosterBehaviors;

        public BaseBoosterBehaviorSO GetBoosterBehavior(GameResource boosterType)
        {
            return boosterBehaviors.FirstOrDefault(e => e.boosterType == boosterType);
        }
        public async UniTask UseBooster(GameResource boosterType, Vector3 position)
        {
            var boosterBehavior = GetBoosterBehavior(boosterType);
            if (boosterBehavior == null)
            {
                Debug.Log($"{LOG_TAG} SBooster behavior not found: {boosterType}");
            }

            await boosterBehavior.UseBooster(position);
        }

        public bool CanUseBooster(GameResource boosterType)
        {
            var boosterBehavior = GetBoosterBehavior(boosterType);
            return boosterBehavior.CanUseBooster();
        }
    }
}