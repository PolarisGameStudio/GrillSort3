using System.Collections;
using System.Collections.Generic;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.UserData;
using UnityEngine;

public class TabHome : UITabBase
{
    [SerializeField] private UILevelView[] levelViews;

    void OnEnable()
    {
        var level = MySonatFramework.GetService<UserDataService>().GetLevel();
        foreach (var levelView in levelViews)
        {
            levelView.SetData(level);
            level++;
        }
    }
}
