using Base.Singleton;
using SonatFramework.Systems.AudioManagement;
using UnityEngine;

public class LoadingInstance : Singleton<LoadingInstance>
{
    [SerializeField] private PopupLoading popupLoading;

    protected override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Show(float time = 3, bool stopMusic = true)
    {
        popupLoading.gameObject.SetActive(true);
        popupLoading.SetTime(time);

        if (stopMusic)
        {
            MySonatFramework.GetService<AudioService>().StopMusic();
        }
    }

    public void Hide()
    {
        popupLoading.gameObject.SetActive(false);
    }
}
