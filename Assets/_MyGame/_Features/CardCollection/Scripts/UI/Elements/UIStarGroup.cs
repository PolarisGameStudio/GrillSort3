using System.Collections.Generic;
using SonatFramework.Systems;
using SonatFramework.Systems.ObjectPooling;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    public class UIStarGroup : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Transform customContainer;

        private readonly Service<PoolingContainerService> _poolingService = new();
        private readonly Service<CardCollectionService> _cardCollectionService = new();
        private List<UIStar> stars = new();

        public void Setup(int star)
        {
            _poolingService.Instance.CleanContainer(container);
            stars.Clear();

            var maxStarView = _cardCollectionService.Instance.GetConfig().maxStarView;
            var starView = Mathf.Min(star, maxStarView);
            if (starView == 5)
            {
                customContainer.gameObject.SetActive(true);
                container.gameObject.SetActive(false);
                foreach (Transform child in customContainer)
                {
                    if (child.TryGetComponent<UIStar>(out var starObj))
                    {
                        stars.Add(starObj);
                    }
                }
            }
            else
            {
                customContainer.gameObject.SetActive(false);
                container.gameObject.SetActive(true);
                for (int i = 0; i < starView; i++)
                {
                    var starObj = _poolingService.Instance.CreateObject<UIStar>(container);
                    stars.Add(starObj);
                }
            }

        }

        public void SetData(bool isOn)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].SetData(isOn);
            }
        }
    }
}