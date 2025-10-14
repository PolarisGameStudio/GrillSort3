using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonToggle : MonoBehaviour
{
    [SerializeField] private GameObject activeObj;
    [SerializeField] private GameObject inactiveObj;

    void OnEnable()
    {
        SetOn(false);
    }

    public void SetOn(bool isOn)
    {
        activeObj.SetActive(isOn);
        inactiveObj.SetActive(!isOn);
    }

    public void OnClick()
    {

    }
}
