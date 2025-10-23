using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UiVersionText : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<TMP_Text>().text = "v" + Application.version;
    }
}
