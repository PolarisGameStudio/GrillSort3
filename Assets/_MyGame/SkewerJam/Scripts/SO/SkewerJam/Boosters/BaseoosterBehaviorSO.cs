using System;
using Cysharp.Threading.Tasks;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SO.Boosters
{
    public abstract class BaseBoosterBehaviorSO : ScriptableObject
    {
        public abstract GameResource boosterType { get; }
        public abstract UniTask UseBooster();

        public virtual bool CanUseBooster()
        {
            return true;
        }
    }
}