using System;
using Cysharp.Threading.Tasks;

namespace MyGame.SkewerJam.UI.Loading
{
    public static class LoadingHelper
    {
        public static void CheckShowLoadingInGameplay()
        {
            if (LoadingInstance.Instance.PlayFromLoadingScene == true)
            {

            }
            else
            {
                LoadingInstance.Instance.ShowLoading();
            }
        }

        public static async UniTask CompleteLoadingInGameplay(Action onComplete)
        {
            if (LoadingInstance.Instance.PlayFromLoadingScene == true)
            {
                LoadingInstance.Instance.PlayFromLoadingScene = false;
                await UniTask.Delay((int)(LoadingInstance.Instance.Config.delayHideLoading * 1000));
                LoadingInstance.Instance.HideLoading();
                await UniTask.Delay(500);
                onComplete?.Invoke();
            }
            else
            {
                await UniTask.Delay(3000);
                onComplete?.Invoke();
            }
        }
    }
}