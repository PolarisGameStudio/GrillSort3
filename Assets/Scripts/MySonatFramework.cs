using SonatFramework.Scripts.Feature.Tracking;
using SonatFramework.Systems.AudioManagement;

public class MySonatFramework : SonatFramework.Systems.SonatSystem
{
    public static MySonatFramework instance;
    public static CustomTrackingService customTrackingService;
    public static AudioService audioService;

    private void Awake()
    {
        instance = this;
    }
}