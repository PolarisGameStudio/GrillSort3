using Cysharp.Threading.Tasks;
using Sonat.Enums;
using SonatFramework.Systems;
using SonatFramework.Systems.AudioManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : SingletonSimple<HomeManager>
{
    public UINavigateBarSlide uINavigateBar;

    private bool running = false;

    private void Awake()
    {
        if (uINavigateBar == null)
            uINavigateBar = GetComponentInChildren<UINavigateBarSlide>();
        //OnCompleteAlbum();

        MySonatFramework.GetService<AudioService>().PlayMusic(AudioId.BGM_Ingame_Summer_Grill3);
    }
    public async UniTask SwitchTab(Sonat.Enums.NavigationType navigation, float delay = 0)
    {
        await UniTask.WaitForSeconds(delay);

        uINavigateBar.SwitchTab(navigation);
    }

    private void OnEnable()
    {
        running = false;
    }

    private void OnDisable()
    {
        running = false;

    }
}
