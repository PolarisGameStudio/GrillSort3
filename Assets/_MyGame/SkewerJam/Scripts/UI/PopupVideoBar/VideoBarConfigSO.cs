using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.SkewerJam.Features.VideoBar
{
    [CreateAssetMenu(fileName = "VideoBarConfigSO", menuName = "MyGame/SkewerJam/VideoBarConfigSO")]
    public class VideoBarConfigSO : ScriptableObject
    {
        public List<MilestoneData> milestones;
        // public int duration = 43200; // 12 hours

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < milestones.Count; i++)
            {
                milestones[i].index = i;
            }
        }
#endif
    }

    [Serializable]
    public class MilestoneData
    {
        [ReadOnly] public int index;
        // public int numberAds;
        public RewardData rewardData;
        public int duration;
    }
}