using System.Collections;
using Gameplay.Entities;
using Manager;
using MyGame.SkewerJam.Gameplay;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using UnityEngine;

public class PopupTutorialGameplay : Panel
{
    [SerializeField] private Transform hand;
    [SerializeField] private int maxTutorialOrder = 3;
    [SerializeField] private float delayStartTutorial = 0.5f;

    private EventBinding<LevelEndedEvent> levelEndedEventBinding;
    private EventBinding<LevelQuitEvent> levelQuitEventBinding;
    private EventBinding<LevelStuckEvent> levelStuckEventBinding;

    private int currentTutorialOrder = 0;
    private ItemId currentTargetItemId = ItemId.None;
    private Coroutine coroutineTutorial;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);
        hand.gameObject.SetActive(false);
        currentTutorialOrder = 0;
        currentTargetItemId = ItemId.None;
    }

    public override void OnOpenCompleted()
    {
        base.OnOpenCompleted();

        var gameLogicHandler = GameController.Instance.GameLogicHandler;
        gameLogicHandler.OnCollectItem += OnCollectItem;
        gameLogicHandler.OnItemStartSwitch += OnItemStartSwitch;
        gameLogicHandler.OnItemEndSwitch += OnItemEndSwitch;
        levelEndedEventBinding = new EventBinding<LevelEndedEvent>(OnLevelEnded);
        levelQuitEventBinding = new EventBinding<LevelQuitEvent>(OnLevelQuit);
        levelStuckEventBinding = new EventBinding<LevelStuckEvent>(OnLevelStuck);
        StartTutorial();
    }

    public override void Close()
    {
        base.Close();

        var gameLogicHandler = GameController.Instance.GameLogicHandler;
        gameLogicHandler.OnCollectItem -= OnCollectItem;
        gameLogicHandler.OnItemStartSwitch -= OnItemStartSwitch;
        gameLogicHandler.OnItemEndSwitch -= OnItemEndSwitch;
        EventBus<LevelEndedEvent>.Deregister(levelEndedEventBinding);
        EventBus<LevelQuitEvent>.Deregister(levelQuitEventBinding);
        EventBus<LevelStuckEvent>.Deregister(levelStuckEventBinding);
    }

    private void OnLevelEnded(LevelEndedEvent @event)
    {
        Close();
    }
    private void OnLevelQuit(LevelQuitEvent @event)
    {
        Close();
    }
    private void OnLevelStuck(LevelStuckEvent @event)
    {
        Close();
    }

    private void OnCollectItem(int id)
    {
        currentTargetItemId = ItemId.None;
        currentTutorialOrder += 1;
        if (currentTutorialOrder >= maxTutorialOrder)
        {
            Close();
        }
    }

    private void OnItemStartSwitch(Item item, SlotBase slot)
    {
        hand.gameObject.SetActive(false);
    }

    private void OnItemEndSwitch(Item item, SlotBase slot)
    {
        if (coroutineTutorial != null)
        {
            StopCoroutine(coroutineTutorial);
        }
        coroutineTutorial = null;
        coroutineTutorial = StartCoroutine(IeStartTutorial());
    }

    private IEnumerator IeStartTutorial()
    {
        yield return new WaitForSeconds(delayStartTutorial);
        StartTutorial();
    }

    private void StartTutorial()
    {
        Item item = GetItemForOrder();
        if (item != null)
        {
            hand.gameObject.SetActive(true);
            hand.transform.position = item.transform.position;
            // hand.DOMove(slot.transform.position, 1.35f).From(item.transform.position).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
        }
    }

    private Item GetItemForOrder()
    {
        if (currentTargetItemId == ItemId.None)
        {
            var orderManager = GameController.Instance.GameLogicHandler.OrderManager;
            var listTargetItemIds = orderManager.GetTargetItemIds();
            currentTargetItemId = listTargetItemIds[UnityEngine.Random.Range(0, listTargetItemIds.Count)];
        }

        var grillManager = GameController.Instance.GameLogicHandler.GrillManager;
        foreach (var grill in grillManager.ListGrills)
        {
            foreach (var slot in grill.GetSlots())
            {
                var item = slot.GetItem();
                if (item != null && (ItemId)item.id == currentTargetItemId)
                {
                    return item;
                }
            }
        }
        return null;
    }
}