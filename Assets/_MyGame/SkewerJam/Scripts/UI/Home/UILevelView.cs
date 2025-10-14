using System.Collections;
using System.Collections.Generic;
using SonatFramework.Systems.UserData;
using TMPro;
using UnityEngine;

public class UILevelView : MonoBehaviour
{
    [SerializeField] private TMP_Text[] txtLevels;
    [SerializeField] private GameObject activeObj;
    [SerializeField] private GameObject inactiveObj;

    public void SetData(int level)
    {
        foreach (var txtLevel in txtLevels)
        {
            txtLevel.text = level.ToString();
        }

        var currentLevel = MySonatFramework.GetService<UserDataService>().GetLevel();
        activeObj.SetActive(currentLevel == level);
        inactiveObj.SetActive(currentLevel != level);
    }


}
