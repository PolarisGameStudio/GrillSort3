
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Entities;
using MyGame.SkewerJam.Gameplay;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;
using UnityEngine.UI;

public class BoosterAnim_BoosterFoodBox : EffectPoolBase
{
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
    [SerializeField] private AnimationCurve itemCurve;
    [SerializeField] private float delayItem = 0.1f;
    [SerializeField] private float distanceItem = 100f;

    private Service<PoolingContainerService> poolingContainerService = new();
    private List<Transform> listItemPrefab = new();
    public async UniTask SetData(Vector3 position, List<Item> listItem)
    {
        listItemPrefab.Clear();
        poolingContainerService.Instance.CleanContainer(itemsContainer);
        targetObject.position = startPos.position;
        await targetObject.DOMove(centerPos.position, durationMove).SetEase(moveInCurve);


        PlayCollectItems(listItem);
        await UniTask.Delay((int)(duration * 1000));

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
            itemPrefab.DOLocalMove(targetPos, durationItem).SetEase(itemCurve);
            await UniTask.Delay((int)(delayItem * 1000));
            targetPos -= Vector3.left * distanceItem;

        }
    }
}