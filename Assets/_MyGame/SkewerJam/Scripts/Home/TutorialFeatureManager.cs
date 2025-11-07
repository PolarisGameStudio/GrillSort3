using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class TutorialFeatureManager : MonoBehaviour
    {
        private List<TutorialFeatureData> _listTutorialDatas = new();

        public void Initialize()
        {
            _listTutorialDatas.Clear();
        }

        public void AddTutorial(TutorialFeatureData tutorialData)
        {
            _listTutorialDatas.Add(tutorialData);
        }

        public async UniTask PlayTutorial()
        {
            HomeManager.Instance.BlockUIManager.RegisterBlockUI(nameof(TutorialFeatureManager));
            _listTutorialDatas.Sort((a, b) => a.order.CompareTo(b.order));
            foreach (var tutorialData in _listTutorialDatas)
            {
                await tutorialData.action();
                await UniTask.Delay(300);
            }
            HomeManager.Instance.BlockUIManager.DeregisterBlockUI(nameof(TutorialFeatureManager));
        }
    }

    [Serializable]
    public class TutorialFeatureData
    {
        public string featureName;
        public int order;
        public Func<UniTask> action;
    }
}