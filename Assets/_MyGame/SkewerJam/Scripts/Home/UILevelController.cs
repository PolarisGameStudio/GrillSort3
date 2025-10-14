using SonatFramework.Systems.UserData;
using UnityEngine;

public class UILevelController : MonoBehaviour
{
    [SerializeField] private UILevelView[] levelViews;

    public void OnEnable()
    {
        var level = MySonatFramework.GetService<UserDataService>().GetLevel();
        foreach (var levelView in levelViews)
        {
            levelView.SetData(level);
            level++;
        }
    }
}