
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.ObjectPooling;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BoosterAnim_BoosterFoodBox : EffectPoolBase
{
    [Space]
    [Header("Booster Food Box")]
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private Transform targetObject;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform endPos;

    [Header("Anim")]
    [SerializeField] private float durationMove = 0.5f;
    [SerializeField] private AnimationCurve moveInCurve;
    [SerializeField] private AnimationCurve moveOutCurve;
    [SerializeField] private float duration = 1f;

    [Header("Items")]
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private float delayCollectItems = 0.5f;
    [SerializeField] private float durationItem = 0.5f;
    [SerializeField] private AnimationCurve itemCurveX;
    [SerializeField] private AnimationCurve itemCurveY;
    [SerializeField] private float delayItem = 0.1f;
    [SerializeField] private float distanceItem = 100f;

    [Header("SEA")]
    [SerializeField] private float delaySoundAppear = 0.5f;
    [SerializeField] private float delaySoundOut = 0.25f;

    private Service<PoolingContainerService> poolingContainerService = new();
    private List<Transform> listItemPrefab = new();


    public async UniTask SetData(Vector3 position, List<Item> listItem)
    {
        MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Booster_FoodTray_Appear_Grill3);

        listItemPrefab.Clear();
        skeletonGraphic.gameObject.SetActive(false);
        poolingContainerService.Instance.CleanContainer(itemsContainer);
        targetObject.position = startPos.position;

        skeletonGraphic.gameObject.SetActive(true);
        skeletonGraphic.AnimationState.SetAnimation(0, "In", false);
        await targetObject.DOMove(centerPos.position, durationMove).SetEase(moveInCurve);


        PlayCollectItems(listItem);
        await UniTask.Delay((int)(duration * 1000));
        skeletonGraphic.AnimationState.SetAnimation(0, "Out", false);

        SonatUtils.DelayCall(delaySoundOut, () =>
        {
            MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Booster_Out_Grill3);
        }, this);
        await targetObject.DOMove(endPos.position, durationMove).SetEase(moveOutCurve);
    }

    private async UniTask PlayCollectItems(List<Item> listItem)
    {
        await UniTask.Delay((int)(delayCollectItems * 1000));
        var count = listItem.Count;
        var targetPos = Vector3.left * distanceItem * (count - 1) / 2;
        foreach (var item in listItem)
        {
            var itemPrefab = poolingContainerService.Instance.CreateObject<Transform>(itemsContainer);
            itemPrefab.position = item.transform.position;
            itemPrefab.GetComponent<Image>().sprite = item.Visual.GetSprite();
            itemPrefab.GetComponent<Image>().SetNativeSize();
            listItemPrefab.Add(itemPrefab);

            GameFactory.Instance.ReturnEntity(item);
            itemPrefab.DOLocalMoveX(targetPos.x, durationItem).SetEase(itemCurveX);
            itemPrefab.DOLocalMoveY(targetPos.y, durationItem).SetEase(itemCurveY).OnComplete(() =>
            {
                MySonatFramework.GetService<AudioService>().PlaySound(AudioId.Booster_FoodTray_Fill_tray_Grill3);
            });
            await UniTask.Delay((int)(delayItem * 1000));
            targetPos -= Vector3.left * distanceItem;
        }
    }
}