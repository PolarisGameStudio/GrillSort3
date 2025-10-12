using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterSpatulaBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterSpatulaBehaviorSO")]
    public class BoosterSpatulaBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterSpatula;

        public override UniTask UseBooster()
        {
            Debug.Log("<color=yellow>BoosterSpatulaBehaviorSO: </color> UseBooster Spatula");
            return UniTask.CompletedTask;
        }
    }
}