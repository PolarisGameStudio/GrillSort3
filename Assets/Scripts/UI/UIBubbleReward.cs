using MyGame.SkewerJam.Utils;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

public class UIBubbleReward : MonoBehaviour
{
    [SerializeField] private GameObject pReward;
    [SerializeField] private UIRewardGroup rewardGroup;
    [SerializeField] private bool dynamicSize = false;
    [SerializeField, ShowIf("dynamicSize")] private RectTransform bg;
    [SerializeField, ShowIf("dynamicSize")] private float[] widths;
    [SerializeField, ShowIf("dynamicSize")] private int threshold = 3;

    private RewardData reward;

    public void SetReward(RewardData reward)
    {
        this.reward = reward;

        if (dynamicSize)
        {
            int count = reward.resourceDatas.Count;
            if (count > threshold)
            {
                bg.sizeDelta = new Vector2(bg.sizeDelta.x, widths[1]);
            }
            else
            {
                bg.sizeDelta = new Vector2(bg.sizeDelta.x, widths[0]);
            }
        }
        rewardGroup.SetData(reward);
        Unselect();
    }

    public void OnClick()
    {
        if (reward == null || reward.resourceDatas.Count == 0) return;

        if (pReward.activeSelf)
        {
            Unselect();
        }
        else
        {
            Select();
        }
    }

    public void Select()
    {
        pReward.SetActive(true);
        if (gameObject.TryGetComponent<Canvas>(out var canvas))
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = LayerManager.TopUI;
            canvas.sortingOrder = 100;
        }
    }

    public void Unselect()
    {
        pReward.SetActive(false);
        if (gameObject.TryGetComponent<Canvas>(out var canvas))
        {
            canvas.overrideSorting = false;
            canvas.sortingLayerName = LayerManager.UI;
            canvas.sortingOrder = 0;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Unselect();
        }
    }
}
