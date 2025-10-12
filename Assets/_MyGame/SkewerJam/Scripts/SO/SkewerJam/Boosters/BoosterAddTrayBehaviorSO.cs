using Cysharp.Threading.Tasks;
using MyGame.SO.Boosters;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterAddTrayBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterAddTrayBehaviorSO")]
    public class BoosterAddTrayBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterAddPlate;

        public override UniTask UseBooster()
        {
            Debug.Log("<color=yellow>BoosterAddTrayBehaviorSO: </color> UseBooster AddTray");
            return UniTask.CompletedTask;
        }
    }
}