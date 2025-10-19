using System;
using System.Collections.Generic;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using UnityEngine;

public class PopupTutorialGroup : PopupTutorialObstacle
{
    [Header("Popup Tutorial Group")]
    [SerializeField] private float delaySwitchState = 7f;
    [SerializeField] private float delayClose2 = 3;

    [SerializeField] private List<ListObjectsInState> listGameObjectsState;

    private int stateIndex = 0;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);


        SwitchState(0);
        SonatUtils.DelayCall(delaySwitchState, () =>
        {
            SwitchState(stateIndex + 1);
        }, this);
    }

    public override void OnClickClose()
    {
        if (canClickClose)
        {
            if (stateIndex + 1 >= listGameObjectsState.Count)
            {
                Close();
                return;
            }
            else
            {
                SwitchState(stateIndex + 1);
            }
        }
    }

    private void SwitchState(int stateIndex)
    {
        this.stateIndex = stateIndex;
        foreach (var state in listGameObjectsState)
        {
            foreach (var obj in state.listObjects)
            {
                obj.SetActive(false);
            }
        }

        foreach (var obj in listGameObjectsState[stateIndex].listObjects)
        {
            obj.SetActive(true);
        }

        canClickClose = false;
        SonatUtils.DelayCall(delayClose2, () =>
        {
            canClickClose = true;
        }, this);
    }

    [Serializable]
    public class ListObjectsInState
    {
        public List<GameObject> listObjects;
    }
}