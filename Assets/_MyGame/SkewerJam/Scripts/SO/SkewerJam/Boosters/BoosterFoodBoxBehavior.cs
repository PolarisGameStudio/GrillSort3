using MyGame.SO.Boosters;
using Sonat.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGame.SkewerJamSO.Boosters
{
    [CreateAssetMenu(fileName = "BoosterFoodBoxBehaviorSO", menuName = "MyGame/SkewerJam/Boosters/BoosterFoodBoxBehaviorSO")]
    public class BoosterFoodBoxBehaviorSO : BaseBoosterBehaviorSO
    {
        public override GameResource boosterType => GameResource.BoosterFoodBox;

        public override UniTask UseBooster()
        {
            Debug.Log("<color=yellow>BoosterFoodBoxBehaviorSO: </color> UseBooster FoodBox");
            return UniTask.CompletedTask;
        }
    }
}