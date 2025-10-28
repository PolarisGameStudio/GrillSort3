using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using UnityEngine;

public class HomeManager : SingletonSimple<HomeManager>
{
    [SerializeField] private UINavigateBarSlide uINavigateBar;
    [SerializeField] private GameObject blockUI;
    [SerializeField] private float delayBlockUI = 2f;
    [SerializeField] private float delaySoundHome = 0.5f;

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
        SonatUtils.DelayCall(delaySoundHome, () =>
        {
            MySonatFramework.GetService<AudioService>().PlayMusic(AudioId.BGM_Home_Default_Grill3);
        }, this);

        ClaimRewardFreeLives().Forget();
        blockUI.SetActive(true);
        SonatUtils.DelayCall(delayBlockUI, () => { blockUI.SetActive(false); }, this);

        SonatSDKAdapter.SetBanner(false);
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
            rewardData.AddReward(new ResourceData(GameResource.Lives, 1800));
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
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PanelManager.Instance.OpenPanel<CheatPanel>();
        }
    }
#endif
}
