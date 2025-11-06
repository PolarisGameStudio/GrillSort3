using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUIManager : MonoBehaviour
{
    [SerializeField] private GameObject blockUI;

    private HashSet<string> _hashSetServiceBlockUI = new();
    private Coroutine _blockUICoroutine;

    public void Initialize()
    {
        Clear();
        StartBlockUI();
    }

    private void BlockUI()
    {
        blockUI.SetActive(true);
    }

    private void UnlockUI()
    {
        blockUI.SetActive(false);
    }

    public void Clear()
    {
        _hashSetServiceBlockUI.Clear();
        if (_blockUICoroutine != null)
        {
            StopCoroutine(_blockUICoroutine);
            _blockUICoroutine = null;
        }
    }

    public void StartBlockUI()
    {
        if (_blockUICoroutine != null)
        {
            StopCoroutine(_blockUICoroutine);
            _blockUICoroutine = null;
        }
        _blockUICoroutine = StartCoroutine(WaitBlockUIAtHome());
    }

    private IEnumerator WaitBlockUIAtHome()
    {
        BlockUI();
        yield return new WaitUntil(() => _hashSetServiceBlockUI.Count == 0);
        UnlockUI();
    }

    public void RegisterBlockUI(string service)
    {
        if (_hashSetServiceBlockUI.Contains(service))
        {
            Debug.LogError($"BlockUIManager: Service {service} already registered");
            return;
        }
        _hashSetServiceBlockUI.Add(service);
        StartBlockUI();
    }

    public void DeregisterBlockUI(string service)
    {
        if (_hashSetServiceBlockUI.Contains(service) == false)
        {
            // Debug.LogError($"BlockUIManager: Service {service} not registered");
            // return;
        }
        _hashSetServiceBlockUI.Remove(service);
    }
}