using SonatFramework.Scripts.Helper;
using TMPro;
using UnityEngine;

namespace MyGame.SkewerJam.Utils
{
    public class UITextGroup : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] texts;

        public void SetText(string content)
        {
            foreach (var text in texts)
            {
                text.SetLocalize(content);
            }
        }
    }
}
