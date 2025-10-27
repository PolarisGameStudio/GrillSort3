using Cysharp.Threading.Tasks;
using MyGame.SkewerJam.Gameplay;
using Sirenix.OdinInspector;
using Sonat;
using Sonat.Enums;
using Sonat.FirebaseModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.EventBus;
using System.Collections;
using UnityEngine;

public class NoInternet : SingletonSimple<NoInternet>
{
    [SerializeField] private Panel panel;
    [SerializeField] private string homeName = "H";
    [SerializeField] private bool forceInternetOnlyHome = false;
    [SerializeField, ReadOnly] private string currentScreen;

    private Coroutine waitCheckInternet;


    private void Start()
    {
        new EventBinding<LevelStartedEvent>(OnLevelStart);

        new EventBinding<UpdateScreenEvent>(OnUpdateScreen);
        // Nếu đang ở Home thì check ngay
        if (forceInternetOnlyHome)
        {
            if (CheckAdditionalConditions())
            {
                CheckConnectInternet();
            }
        }

    }

    private void OnUpdateScreen(UpdateScreenEvent @event)
    {
        currentScreen = @event.screen;

        // Lấy giá trị từ Remote Config
        //forceInternetOnlyHome = SonatFirebase.remote.GetRemoteBool("force_internet_only_home", false);

        // Nếu chỉ cho phép check ở Home và đang rời Home -> dừng checking
        if (forceInternetOnlyHome && currentScreen != homeName)
        {
            StopWaitCheckInternet();
            return;
        }

        // Nếu đủ điều kiện thì check (ví dụ về lại Home)
        if (CheckAdditionalConditions())
        {
            CheckConnectInternet();
        }
    }

    private void OnLevelStart()
    {
        // Nếu không bật chế độ "chỉ check ở Home" -> giữ logic cũ
        if (!forceInternetOnlyHome && SonatFirebase.remote.GetRemoteBool("internet_connection", true))
        {
            CheckConnectInternet();
        }
        // Nếu forceInternetOnlyHome = true => KHÔNG check khi gameplay
    }

    /// <summary>
    /// Chỉ cho phép check nếu (không bị giới hạn) hoặc (đang ở Home)
    /// </summary>
    private bool CheckAdditionalConditions()
    {
        if (forceInternetOnlyHome && currentScreen != homeName)
            return false;

        return true;
    }

    private void CheckConnectInternet()
    {
        if (!SonatSdkManager.IsInternetConnection())
        {
            StopWaitCheckInternet();

            panel.Open(null);
        }
        else
        {
            WaitCheckInternet();
        }
    }

    private void StopWaitCheckInternet()
    {
        if (waitCheckInternet != null)
        {
            StopCoroutine(waitCheckInternet);
            waitCheckInternet = null;
        }
    }

    public void WaitCheckInternet()
    {
        if (waitCheckInternet == null)
        {
            int timeGap = SonatFirebase.remote.GetRemoteInt("check_internet_time_gap", 1);
            if (timeGap > 0)
                waitCheckInternet = StartCoroutine(WaitCheckConnectInternet(timeGap));
        }
    }

    private IEnumerator WaitCheckConnectInternet(int timeGap)
    {
        yield return new WaitForSeconds(timeGap);
        waitCheckInternet = null;

        // Chỉ tiếp tục check nếu vẫn còn ở Home (khi bật force)
        if (CheckAdditionalConditions())
            CheckConnectInternet();
    }

    public void ClosePanel()
    {
        panel.Close();

        // Nếu đang ở Home thì mới tiếp tục check lại
        if (CheckAdditionalConditions())
            WaitCheckInternet();
    }
}
