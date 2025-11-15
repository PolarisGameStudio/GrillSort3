using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "ForceHardOrderConfigSO_v2", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/ForceHardOrderConfigSO_v2")]
    public class ForceHardOrderConfigSO_v2 : ScriptableObject
    {
        public List<ForceHardOrderData> listForceHardOrderDatas;

#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var forceHardOrderData in listForceHardOrderDatas)
            {
                forceHardOrderData.index = listForceHardOrderDatas.IndexOf(forceHardOrderData);
            }
        }
#endif
        public ForceHardOrderData GetForceHardOrderData(int curveIndex)
        {
            var count = listForceHardOrderDatas.Count;
            return listForceHardOrderDatas[Mathf.Clamp(curveIndex, 0, count - 1)];
        }

        [Serializable]
        public class ForceHardOrderData
        {
            [GUIColor(0f, 1f, 0f)]
            [ReadOnly]
            public int index;
            public float rateP4;
            public float rateP5;

            public float rateDuplicate;

            public int GetRandomRemainingSlot(int numEmptyWaitingSlot)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= rateP4)
                {
                    return 1;
                }
                else if (rand <= rateP4 + rateP5)
                {
                    return 0;
                }
                return 0;
            }

            public bool CanNextUse()
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                return rand <= rateDuplicate;
            }
        }
    }
}