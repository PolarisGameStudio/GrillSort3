using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems.InventoryManagement.GameResources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.UI.PopupVideoBar
{
    public class UIMilestone : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtNumber;
        [SerializeField] private Slider slider;
        [SerializeField] private UIRewardGroup rewardGroup;
        [SerializeField] private UIBubbleReward bubbleReward;


        public void SetData(int number, float valueSlider, RewardData rewardData)
        {
            txtNumber.text = number.ToString();
            slider.value = valueSlider;

            var useBubbleReward = rewardData.resourceDatas.Count > 1;
            if (useBubbleReward)
            {
                bubbleReward.SetReward(rewardData);
                bubbleReward.gameObject.SetActive(true);
                rewardGroup.gameObject.SetActive(false);
            }
            else
            {
                rewardGroup.SetData(rewardData);
                rewardGroup.gameObject.SetActive(true);
                bubbleReward.gameObject.SetActive(false);
            }
        }
    }
}

