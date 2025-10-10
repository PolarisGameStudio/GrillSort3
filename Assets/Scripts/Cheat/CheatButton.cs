using SonatFramework.Scripts.UIModule;
using UnityEngine;

public class CheatButton : MonoBehaviour
{
    private int count;
    public Panel needOff;

    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickCheat()
	{
		if (Time.time - time > 1)
		{
			count = 0;
		}
		time = Time.time;
        count++;
        if(count >= 5 || CheatManager.unlocked)
		{
			if (needOff)
			{
                needOff.Close();
			}
            PanelManager.Instance.OpenForget<CheatPanel>();
		}
	}
}
