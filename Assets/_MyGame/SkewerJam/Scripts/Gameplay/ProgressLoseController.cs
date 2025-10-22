using Manager;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Systems;
using SonatFramework.Systems.TrackingModule;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProgressLoseController : MonoBehaviour
{
    [SerializeField] UnityEvent onRevive;
    [SerializeField] TMP_Text txtCount;

    private const string PROGRESS_LOSE_KEY = "PROGRESS_LOSE_KEY";

    private readonly Service<GameplayAnalyticsService> gameplayAnalytics = new();
    private IntDataPref currentLevelStartCount;
    private IntDataPref count;

    private bool active = true;
    private int maxCount = 3;
    private int levelStartFeature = 5;

    private void OnEnable()
    {
        LoadData();

        if (!active)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            if (CheckReset())
            {
                Reset();
            }

            UpdateUI();
        }
    }

    private void OnDisable()
    {

    }

    private void LoadData()
    {
        currentLevelStartCount = new IntDataPref(PROGRESS_LOSE_KEY + "_currentLevelStartCount");
        count = new IntDataPref(PROGRESS_LOSE_KEY + "_count");

        var level = MySonatFramework.userDataService.GetLevel();
        active = GameRemoteConfigValue.activeProgressLose && level >= SonatSDKAdapter.GetRemoteInt(PROGRESS_LOSE_KEY + "_level_start_feature", levelStartFeature);
        maxCount = SonatSDKAdapter.GetRemoteInt(PROGRESS_LOSE_KEY + "_max_count_revive", maxCount);
    }

    private bool CheckReset()
    {
        // bắt đầu level mới hoặc chơi lại
        return gameplayAnalytics.Instance.levelPlayData.startCount != currentLevelStartCount.Value;
    }

    private void Reset()
    {
        currentLevelStartCount.Value = gameplayAnalytics.Instance.levelPlayData.startCount;
        count.Value = 0;
    }

    private void UpdateUI()
    {
        // update UI
        if (count.Value >= maxCount)
        {
            gameObject.SetActive(false);
        }

        txtCount.text = $"{count.Value}/{maxCount}";

    }

    public void OnClickRevive()
    {
        SonatSDKAdapter.ShowRewardAds(OnReviveWithAds, "progress_lose", "progress_lose");
    }

    private void OnReviveWithAds()
    {
        count.Value++;
        onRevive?.Invoke();
    }
}