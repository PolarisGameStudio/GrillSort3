using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : SingletonSimple<HomeManager>
{
    public UINavigateBarSlide uINavigateBar;

    private bool running = false;

    private void Awake()
    {
        if (uINavigateBar == null)
            uINavigateBar = GetComponentInChildren<UINavigateBarSlide>();
        //OnCompleteAlbum();
    }

    public async UniTask SwitchTab(Sonat.Enums.NavigationType navigation, float delay = 0)
    {
        await UniTask.WaitForSeconds(delay);

        uINavigateBar.SwitchTab(navigation);
    }

    private void OnEnable()
    {
        running = false;
        MySonatFramework.GetService<AudioService>().PlayMusic(AudioId.BGM_Ingame_Summer_Grill3);

        ClaimRewardFreeLives().Forget();
    }

    private void OnDisable()
    {
        running = false;

    }

    private async UniTask ClaimRewardFreeLives()
    {

        var checkRewardFreeLives = new IntDataPref("check_reward_free_lives");
        if (checkRewardFreeLives.Value == 1)
        {
            checkRewardFreeLives.Value = 0;
            var rewardData = new RewardData();
            rewardData.AddReward(new ResourceData(GameResource.Lives, 900));
            MySonatFramework.GetService<InventoryService>().AddReward(rewardData, new EarnResourceLogData
            {
                spendType = "reward_free_lives",
                spendId = "reward_free_lives",
                isFirstBuy = false,
                source = "non_iap"
            }, false);
            await UniTask.Delay(2000);
            PanelManager.Instance.OpenPanel<PopupReward>(new UIData().Add(PopupReward.KEY_REWARD, rewardData));
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var rewardData = new RewardData();
            rewardData.AddReward(new ResourceData(GameResource.Coin, 1000));
            rewardData.AddReward(new ResourceData(GameResource.Lives, 900));
            rewardData.AddReward(new ResourceData(GameResource.BoosterAddPlate, UnityEngine.Random.Range(1, 10)));
            rewardData.AddReward(new ResourceData(GameResource.BoosterSpatula, UnityEngine.Random.Range(1, 10)));
            rewardData.AddReward(new ResourceData(GameResource.BoosterShuffle, UnityEngine.Random.Range(1, 10)));
            rewardData.AddReward(new ResourceData(GameResource.BoosterFoodBox, UnityEngine.Random.Range(1, 10)));

            MySonatFramework.GetService<InventoryService>().AddReward(rewardData, new EarnResourceLogData
            {
                spendType = "test",
                spendId = "test",
                isFirstBuy = false,
                source = "non_iap"
            }, false);
            PanelManager.Instance.OpenPanel<PopupReward>(new UIData().Add(PopupReward.KEY_REWARD, rewardData));
        }
    }
#endif
}
