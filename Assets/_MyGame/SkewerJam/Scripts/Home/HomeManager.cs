using Cysharp.Threading.Tasks;
using SonatFramework.Systems;
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
