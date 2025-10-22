using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupStarChest : Panel
{
    private const string POPUP_STAR_CHEST_KEY = "POPUP_STAR_CHEST_KEY";

    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text txtProgress;
    [SerializeField] private UIBubbleReward bubbleReward;
    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnPlay;

    [Header("Anim")]
    [SerializeField] private float delayAnim = 0.5f;
    [SerializeField] private float durationAnim = 1f;

    private readonly Service<StarChestService> starChestService = new();
    private IntDataPref _starView;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        LoadData();
        StartCoroutine(UpdateView());
        bubbleReward.SetReward(starChestService.Instance.CurrentChest.reward);
    }

    private void LoadData()
    {
        _starView = new IntDataPref(POPUP_STAR_CHEST_KEY + "_starView", 0);
    }

    private IEnumerator UpdateView()
    {
        var star = MySonatFramework.GetService<InventoryService>().GetResource(GameResource.Star);
        var maxStar = starChestService.Instance.GetRequiredStar();
        if (_starView.Value >= star) _starView.Value = 0;

        btnClaim.interactable = false;
        btnPlay.interactable = false;
        btnPlay.gameObject.SetActive(star < maxStar);
        btnClaim.gameObject.SetActive(star >= maxStar);

        progressBar.value = _starView.Value * 1.0f / maxStar;
        txtProgress.text = $"{_starView.Value}/ {maxStar}";

        if (star != _starView.Value)
        {
            yield return new WaitForSeconds((int)(delayAnim * 1000));

            var endValue = Mathf.Min(star, maxStar);
            DOTween.To(() => _starView.Value, x => _starView.Value = x, endValue, durationAnim).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                txtProgress.text = $"{_starView.Value}/ {maxStar}";
                progressBar.value = _starView.Value * 1.0f / maxStar;
            }).OnComplete(() =>
            {
                btnClaim.interactable = true;
                btnPlay.interactable = true;

                // if (CheckComplete())
                // {

                // }
            });
            _starView.Value = star;
        }
        else
        {
            btnClaim.interactable = true;
            btnPlay.interactable = true;
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnClickClaim()
    {
        RewardData rewardData = starChestService.Instance.CurrentChest.reward;
        var uiData = new UIData();
        uiData.Add(PopupRewardChest.REWARD_KEY, rewardData);
        PanelManager.Instance.OpenPanel<PopupRewardChest>(uiData);

        // logic
        // starChestService.Instance.NextStarChest();
        StartCoroutine(UpdateView());
    }

    private bool CheckComplete()
    {
        return _starView.Value >= starChestService.Instance.GetRequiredStar();
    }
}
