using DG.Tweening;
using Gameplay.Entities;
using Gameplay.LevelData;
using UnityEngine;

public class ItemWrapped : Item
{
    [SerializeField] private GameObject wrappedObject;
    [SerializeField] private GameObject itemObject;
    [SerializeField] private SpriteRenderer wrappedSprite;

    [Header("Visual")]
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float delay = 0.5f;

    public override void SetItemData(ItemData data, SlotBase slot)
    {
        base.SetItemData(data, slot);

        // If item is on primary grill, unwrap it immediately
        if (isPrimary)
        {
            UnWrappedItem();
        }
        else
        {
            WrappedItem(true);
        }
    }

    public override void MoveToPrimary(SlotBase slot, int index)
    {
        UnWrappedItem();
        base.MoveToPrimary(slot, index);
    }

    public override void MoveToSub(SlotBase slot, int index)
    {
        WrappedItem(true);
        base.MoveToSub(slot, index);
    }

    private void UnWrappedItem()
    {
        itemObject.SetActive(true);
        wrappedSprite.DOFade(0, duration).SetDelay(delay).OnComplete(() =>
        {
            wrappedObject.SetActive(false);
        });
    }

    private void WrappedItem(bool wrapped = true)
    {
        wrappedSprite.DOKill();
        wrappedSprite.color = new Color(wrappedSprite.color.r, wrappedSprite.color.g, wrappedSprite.color.b, 1);

        wrappedObject.SetActive(wrapped);
        itemObject.SetActive(!wrapped);
    }

    public override void OnReturnObj()
    {
        base.OnReturnObj();

        wrappedSprite.DOKill();
        wrappedSprite.color = new Color(wrappedSprite.color.r, wrappedSprite.color.g, wrappedSprite.color.b, 1);
    }
}

