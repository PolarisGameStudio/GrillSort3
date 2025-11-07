using UnityEngine;

namespace MyGame.SkewerJam.UI.Loading
{
    [CreateAssetMenu(fileName = "LoadingConfigSO", menuName = "MyGame/SkewerJam/Loading/LoadingConfigSO")]
    public class LoadingConfigSO : ScriptableObject
    {
        public float delayHideLoading = 0.25f;
    }
}