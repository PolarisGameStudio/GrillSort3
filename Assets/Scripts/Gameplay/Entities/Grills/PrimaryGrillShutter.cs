using System;
using Cysharp.Threading.Tasks;
using Gameplay.Entities;
using Gameplay.LevelData;
using MyGame.SkewerJam.Gameplay;
using UnityEngine;

public class PrimaryGrillShutter : PrimaryGrill
{
    [SerializeField] private GrillVisualShutter visualShutter;

    private const int MoveToChangeState = 3;

    private bool isClosed = false;

    private int moveCount = 0;

    public override async UniTask SetData(GrillData grillData)
    {
        _ = base.SetData(grillData);
        isClosed = grillData.isLock;
        visualShutter.SetUpShutter(isClosed);
        SetLockItems(isClosed);

        moveCount = isClosed ? 0 : 0;
    }

    public void OnEnable()
    {
        grillBaseBehaviorSO.eventSystemSO.RegisterEvents_OnCollectItem(OnCollectItem);
        grillBaseBehaviorSO.eventSystemSO.RegisterEvents_OnDropItem(OnItemDropped);
    }

    public void OnDisable()
    {
        grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);
        grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnDropItem(OnItemDropped);
    }

    private void OnItemDropped(Item item, bool fromWaitingGrill, bool toOrder)
    {
        if (completed == true) return;
        if (fromWaitingGrill == true) return;

        moveCount++;
        Debug.Log("<color=green>[PrimaryGrillShutter]</color> OnItemDropped: " + name + " -move count" + moveCount);
        if (moveCount >= MoveToChangeState)
        {
            isClosed = !isClosed;
            SetLockItems(isClosed);
            visualShutter.SetUpShutter(isClosed);
            moveCount = 0;

            // if (isClosed == true)
            // {
            //     GameController.Instance.GameLogicHandler.TryCheckLoseGame();
            // }
        }
    }

    private void OnCollectItem(int itemId)
    {

    }
}
