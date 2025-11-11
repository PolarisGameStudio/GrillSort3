using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

namespace MyGame.Modules.QuestEvent
{
    [CreateAssetMenu(fileName = "QuestEventConfigSO", menuName = "MyGame/SkewerJam/Features/QuestEvent/QuestEventConfigSO")]
    public class QuestEventConfigSO : LockServiceConfigSO
    {
        [Space(10)]
        [Header("Config milestones")]
        public List<MilestoneConfig> listMilestones;

        public int GetMaxItem(int index)
        {
            return listMilestones[index].numItem;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < listMilestones.Count; i++)
            {
                listMilestones[i].index = i;
            }
        }
#endif
    }

    [Serializable]
    public class MilestoneConfig
    {
        [GUIColor(0, 1, 0, 1)]
        [ReadOnly]
        public int index;
        public int numItem;
        public RewardData rewardData;
    }
}