using Scripts.UI.Gameplay.PackBooster;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.BoosterManagement;
using UnityEngine;

public class PopupBuyBooster2 : PopupBuyBooster
{
    [Header("PopupBuyBooster2")]
    [SerializeField] private UIPackBooster[] packBoosters;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        for (int i = 0; i < packBoosters.Length; i++)
        {
            var config = (boosterService.Instance as SonatBoosterService).BoostersConfig.boosterPackConfigs.Find(x => x.booster == boosterConfig.booster);
            packBoosters[i].SetData(config.pack[i].rewardData, config.pack[i].price, i == 0);
        }
    }
}