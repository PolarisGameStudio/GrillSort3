using Cysharp.Threading.Tasks;
using SkewerJam.Utils.Effects;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using UnityEngine;

public class WidgetStarChest : UIHomeWidget
{
    [SerializeField] private Transform spanwPosition;
    [SerializeField] private GameObject iconWarning;

    private readonly Service<StarChestService> starChestService = new();

    public override void Setup()
    {
        base.Setup();

        if (starChestService.Instance.Config.active)
        {
            gameObject.SetActive(true);
            UpdateUI();
            starChestService.Instance.OnNextMilestone += UpdateUI;
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void OnDestroy()
    {
        starChestService.Instance.OnNextMilestone -= UpdateUI;
    }

    public override void OnFocus()
    {
        base.OnFocus();
        UpdateUI();
    }

    public override async UniTask<bool> ProcessTask()
    {
        if (starChestService.Instance.Config.active == false) return false;
        OnCollectStar();
        return false;
    }

    private void OnCollectStar()
    {
        var starView = MySonatFramework.GetService<InventoryService>().GetResourceView(GameResource.Star);
        var star = MySonatFramework.GetService<InventoryService>().GetResource(GameResource.Star);


        var diff = star - starView;
        if (diff > 0)
        {
            SonatUtils.DelayCall(1.5f, () =>
            {
                EventBus<AddItemEvent>.Raise(new AddItemEvent()
                {
                    resource = GameResource.Star,
                    quantity = diff,
                    position = spanwPosition.position,
                    collectEffect = new CollectEffectMultipleAtHome()
                });
            });
        }
    }

    private void UpdateUI()
    {
        var star = MySonatFramework.GetService<InventoryService>().GetResource(GameResource.Star);
        if (star >= starChestService.Instance.GetRequiredStar())
        {
            iconWarning.SetActive(true);
        }
        else
        {
            iconWarning.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventBus<AddItemEvent>.Raise(new AddItemEvent()
            {
                resource = GameResource.Star,
                quantity = 25,
                position = spanwPosition.position,
                collectEffect = new CollectEffectMultipleAtHome()
                {
                    collectEffectName = "UICollectEffectMultipleAtHome"
                }
            });
        }
    }
#endif
}