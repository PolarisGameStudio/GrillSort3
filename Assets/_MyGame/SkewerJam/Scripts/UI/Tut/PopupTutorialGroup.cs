using System;
using System.Collections;
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
    private Coroutine coroutine;

    public override void Open(UIData uiData)
    {
        base.Open(uiData);


        SwitchState(0);
        WaitNextState();
    }

    private void WaitNextState()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(NextState());
    }

    private IEnumerator NextState()
    {
        yield return new WaitForSeconds(delaySwitchState);
        SwitchState(stateIndex + 1);
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
        if (stateIndex >= listGameObjectsState.Count) return;

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

        WaitNextState();
    }

    [Serializable]
    public class ListObjectsInState
    {
        public List<GameObject> listObjects;
    }
}