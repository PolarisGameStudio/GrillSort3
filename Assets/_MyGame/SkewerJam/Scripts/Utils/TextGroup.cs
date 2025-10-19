using TMPro;
using UnityEngine;

namespace MyGame.SkewerJam.Utils
{
    public class TextGroup : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] texts;

        public void SetText(string content)
        {
            foreach (var text in texts)
            {
                text.text = content;
            }
        }
    }
}
