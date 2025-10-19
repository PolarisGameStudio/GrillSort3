using Sirenix.OdinInspector;
using SonatFramework.Scripts.UIModule;
using UnityEngine;

namespace MyGame.SkewerJam.UI.Tut
{
    public class AnimationController : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private GameObject[] listGameObjects;

        [Header("Tween")]
        [SerializeField] private float duration = 1;
        [SerializeField] private float startDelay = 0.3f;
        [SerializeField] private float delayBetween = 0.3f;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        [Button("Setup Tween Elements")]
        public void Setup()
        {
            var delay = startDelay;
            foreach (var obj in listGameObjects)
            {
                if (obj.GetComponent<UITweenElement>() == null)
                {
                    obj.AddComponent<UITweenElement>();
                }
                var tweenElements = obj.GetComponents<UITweenElement>();
                foreach (var tweenElement in tweenElements)
                {
                    if (tweenElement.tweenData != null)
                    {

                    }
                    else
                    {
                        tweenElement.tweenData = new TweenData();
                    }

                    tweenElement.tweenData.target = obj.transform;
                    tweenElement.tweenData.custom = true;
                    var config = new TweenConfig()
                    {
                        tweenType = UITweenType.Scale,
                        duration = duration,
                        delay = delay,
                        curve = curve
                    };
                    tweenElement.tweenData.config = config;
                }
                delay += delayBetween;
            }
            Debug.Log("<color=green>Setup Tween Elements</color>");
        }
#endif
    }
}
