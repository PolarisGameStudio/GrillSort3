using SonatFramework.Scripts.UIModule.UIElements;
using SonatFramework.Systems;
using UnityEngine;

namespace MyGame.Modules.QuestEvent.UI.Elements
{
    public class UITimeCounter : MonoBehaviour
    {
        [SerializeField] private SonatFramework.Scripts.UIModule.UIElements.UITimeCounter timeCounter;
        private readonly Service<QuestEventService> _questEventService = new();


        private void OnEnable()
        {
            timeCounter.SetData(_questEventService.Instance.GetRemainTime());
        }
    }
}