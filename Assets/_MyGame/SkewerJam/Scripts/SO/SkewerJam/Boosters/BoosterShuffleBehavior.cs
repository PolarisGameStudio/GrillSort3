using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterShuffleBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterShuffleBehaviorSO")]
    public class BoosterShuffleBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterShuffle;

        public override UniTask UseBooster()
        {
            Debug.Log("<color=yellow>BoosterShuffleBehaviorSO: </color> UseBooster Shuffle");
            return UniTask.CompletedTask;
        }
    }
}