using I2.Loc;
using SonatFramework.Systems;
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Modules.CardCollection
{
    public class PanelCardCollection : MonoBehaviour
    {
        [SerializeField] private GameObject lockedObj;
        [SerializeField] private GameObject unlockedObj;
        [SerializeField] private LocalizationParamsManager levelUnlockParamsManager;

        [Space]
        [Header("Events")]
        [SerializeField] private UnityEvent onUpdateDataEvent;

        private readonly Service<CardCollectionService> _cardCollectionService = new();

        private void OnEnable()
        {
            if (_cardCollectionService.Instance.IsUnlocked() == false)
            {
                SetUnlockState(false);
                levelUnlockParamsManager.SetParameterValue("VALUE", $"{_cardCollectionService.Instance.GetConfig().unlockLevel}");
                return;
            }

            SetUnlockState(true);
            UpdateData();

            // _cardCollectionService.Instance.OnNewCardCountChanged += UpdateData;
            // _cardCollectionService.Instance.OnChangeData += UpdateData;

        }

        private void OnDisable()
        {
            // _cardCollectionService.Instance.OnNewCardCountChanged -= UpdateData;
            // _cardCollectionService.Instance.OnChangeData -= UpdateData;
        }

        private void SetUnlockState(bool isUnlocked)
        {
            lockedObj.SetActive(!isUnlocked);
            unlockedObj.SetActive(isUnlocked);
        }

        public void UpdateData()
        {
            onUpdateDataEvent?.Invoke();
        }
    }
}
