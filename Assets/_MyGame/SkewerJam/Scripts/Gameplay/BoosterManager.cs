using System.Linq;
using Cysharp.Threading.Tasks;
using MyGame.SO.Boosters;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class BoosterManager : MonoBehaviour
    {
        private const string LOG_TAG = "<color=yellow>BoosterLogicHandler: </color>";
        [SerializeField] private BaseBoosterBehaviorSO[] boosterBehaviors;

        public async UniTask UseBooster(GameResource boosterType)
        {
            var boosterBehavior = boosterBehaviors.FirstOrDefault(e => e.boosterType == boosterType);
            if (boosterBehavior == null)
            {
                Debug.Log($"{LOG_TAG} SBooster behavior not found: {boosterType}");
            }
            await boosterBehavior.UseBooster();
        }
    }
}