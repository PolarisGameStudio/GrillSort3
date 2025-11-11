using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SonatFramework.Scripts.Helper;
using TMPro;
using UnityEngine;

public class UITagInPack : MonoBehaviour
{
    [SerializeField] private TagType tagType;
    [SerializeField] private TMP_Text text;
    [SerializeField, ShowIf("@tagType == TagType.Sale || tagType == TagType.Off")] private int value;

    private void OnEnable()
    {
        switch (tagType)
        {
            case TagType.New:
                SetTagNew();
                break;
            case TagType.Sale:
                SetTagSale();
                break;
            case TagType.Off:
                SetTagOff();
                break;
        }
    }

    private void SetTagNew()
    {
        text.SetLocalize("New");
    }

    private void SetTagSale()
    {
        text.SetLocalize("Sale");
        text.SetLocalizeParam("VALUE", value.ToString());
    }

    private void SetTagOff()
    {
        text.SetLocalize("Off");
        text.SetLocalizeParam("VALUE", value.ToString());
    }
}

[Serializable]
public enum TagType
{
    New,
    Sale,
    Off
}
