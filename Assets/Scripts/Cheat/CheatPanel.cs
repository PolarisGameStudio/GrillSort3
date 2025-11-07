using System.Collections.Generic;
using MyGame.Modules.CardCollection;
using MyGame.Modules.QuestEvent;
using Sonat;
using Sonat.DebugViewModule;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.GameDataManagement;
using SonatFramework.Systems.SceneManagement;
using SonatFramework.Systems.TimeManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatPanel : Panel
{
    public GameObject panel;
    public TMP_Dropdown cheatOptsDropdown;
    public TMP_Dropdown[] cheatDropdowns;
    public TMP_InputField[] inputValues;

    public TMP_InputField inputPassword;

    //public InputField ipAdressInput;
    public GameObject lockObj;
    public TMP_InputField timeNow;
    public Toggle localizeToggle;
    public GameObject fpsLog;
    public GameObject downloadProcess;
    private readonly Service<DataService> dataService = new Service<DataService>();
    private PlayerPrefString ipAdress;

    public override void OnSetup()
    {
        base.OnSetup();
        InitOpts();
        OnDropdown(0);
        OnInputField(1);
        localizeToggle.isOn = dataService.Instance.GetInt("USING_LOCAL_TIME", 0) == 1;
        localizeToggle.onValueChanged.AddListener(SwitchLocalTime);
#if UNITY_EDITOR
        lockObj.SetActive(false);
#else
        if (PlayerPrefs.HasKey("SONAT_CHEATED"))
        {
            CheatManager.unlocked = true;
            lockObj.SetActive(false);
        }
        else
        {
            lockObj.SetActive(true);
        }
#endif
        ipAdress = new PlayerPrefString("IP_ADDRESS", "");
        //ipAdressInput.text = ipAdress.Value;
    }

    public void Unlock()
    {
        if (inputPassword.text.Equals("Sonat@111"))
        {
            lockObj.SetActive(false);
            CheatManager.unlocked = true;
            PlayerPrefs.SetInt("SONAT_CHEATED", 1);
        }
    }

    public override void Open(UIData uiData)
    {
        base.Open(uiData);
        panel.SetActive(true);
    }

    public override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        // DOVirtual.DelayedCall(0.2f, () =>
        // {
        // 	PanelManager.Instance.MoveBackPanel(this);
        // });
    }

    public override void OnFocus()
    {
    }

    public override void OnFocusLost()
    {
    }

    public override void Close()
    {
        base.Close();
    }

    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
    }

    private CheatOption GetCheatOpt()
    {
        CheatOption cheatOption = cheatOptsDropdown.options[cheatOptsDropdown.value].text.ToEnum<CheatOption>();
        return cheatOption;
    }

    private void OnCheatOptChange(int value)
    {
        CheatOption cheatOption = GetCheatOpt();
        switch (cheatOption)
        {
            case CheatOption.Level:
                OnDropdown(0);
                OnInputField(1);
                break;
            case CheatOption.Win:
            case CheatOption.Lose:
            case CheatOption.WinState:
                OnDropdown(0);
                OnInputField(0);
                break;
            case CheatOption.Resource:
                OnDropdown(1);
                OnInputField(1);
                InitResourceDropdown();
                break;
            case CheatOption.RemoteConfig:
            case CheatOption.PlayerPrefs:
                OnDropdown(0);
                OnInputField(2);
                break;
            case CheatOption.GDLevel:
                OnDropdown(1);
                OnInputField(0);
                InitGDLevelDropdown();
                break;
            case CheatOption.StarChest:
                OnInputField(2);
                break;
            case CheatOption.LevelChest:
                OnInputField(2);
                break;
            case CheatOption.TransportTracking:
                OnDropdown(0);
                OnInputField(1);
                inputValues[0].text = ipAdress.Value;
                break;
            case CheatOption.CardCollection:
                OnDropdown(1);
                InitCardCollectionDropdown();
                break;
            case CheatOption.QuestEvent:
                OnInputField(1);
                break;
        }
    }

    private CheatLevelSource GetCheatLevelSource()
    {
        CheatLevelSource levelSource = cheatDropdowns[0].options[cheatDropdowns[0].value].text.ToEnum<CheatLevelSource>();
        return levelSource;
    }

    private void OnCheatGDLevelChange(int value)
    {
        CheatLevelSource levelSource = GetCheatLevelSource();

        switch (levelSource)
        {
            case CheatLevelSource.Resources:
                OnInputField(0);
                break;
            case CheatLevelSource.Drive:
                OnInputField(1);
                break;
        }
    }


    public void InitOpts()
    {
        List<TMP_Dropdown.OptionData> opts = new List<TMP_Dropdown.OptionData>();
        for (CheatOption i = CheatOption.Level; i < CheatOption.MAX; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData()
            {
                text = i.ToString()
            };
            opts.Add(data);
        }

        cheatOptsDropdown.ClearOptions();
        cheatOptsDropdown.options = opts;
        cheatOptsDropdown.onValueChanged.RemoveAllListeners();
        cheatOptsDropdown.onValueChanged.AddListener(OnCheatOptChange);
        OnCheatOptChange(0);
    }

    public void InitResourceDropdown()
    {
        List<TMP_Dropdown.OptionData> opts = new List<TMP_Dropdown.OptionData>();

        for (GameResource i = GameResource.Coin; i < GameResource.MAX; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData()
            {
                text = i.ToString()
            };
            opts.Add(data);
        }

        cheatDropdowns[0].onValueChanged.RemoveAllListeners();
        cheatDropdowns[0].ClearOptions();
        cheatDropdowns[0].options = opts;
    }

    public void InitGDLevelDropdown()
    {
        int cheatLevelSource = PlayerPrefs.GetInt("SONAT_CHEATED_LEVELSOURCE", 0);
        List<TMP_Dropdown.OptionData> opts = new List<TMP_Dropdown.OptionData>();

        for (CheatLevelSource i = CheatLevelSource.Resources; i < CheatLevelSource.MAX; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData()
            {
                text = i.ToString()
            };
            opts.Add(data);
        }

        cheatDropdowns[0].onValueChanged.RemoveAllListeners();
        cheatDropdowns[0].onValueChanged.AddListener(OnCheatGDLevelChange);
        cheatDropdowns[0].ClearOptions();
        cheatDropdowns[0].options = opts;
        cheatDropdowns[0].value = cheatLevelSource;
    }

    public void OnCheatClick()
    {
        Cheat();
    }

    private void Cheat()
    {
        CheatOption cheatOption = GetCheatOpt();
        switch (cheatOption)
        {
            case CheatOption.Win:
                panel.SetActive(false);
                CheatManager.CheatWin();
                break;
            case CheatOption.WinState:
                panel.SetActive(false);
                CheatManager.CheatWinState();
                break;
            case CheatOption.Lose:
                panel.SetActive(false);
                CheatManager.CheatLose();
                break;
            case CheatOption.Level:
                panel.SetActive(false);
                if (int.TryParse(inputValues[0].text, out var level))
                {
                    CheatManager.CheatLevel(level);
                }

                break;
            case CheatOption.Resource:
                if (int.TryParse(inputValues[0].text, out var value))
                {
                    GameResource resource = cheatDropdowns[0].options[cheatDropdowns[0].value].text.ToEnum<GameResource>();
                    CheatManager.CheatResource(resource, value);
                }

                break;
            case CheatOption.RemoteConfig:
                CheatManager.CheatRemoteConfig(inputValues[0].text, inputValues[1].text);
                break;
            case CheatOption.PlayerPrefs:
                CheatManager.CheatPlayerPrefs(inputValues[0].text, inputValues[1].text);
                break;
            case CheatOption.TransportTracking:
                StartTransportDebugView();
                break;
            // case CheatOption.StarChest:
            // 	if (int.TryParse(inputValues[0].text, out int chestID) && int.TryParse(inputValues[1].text, out int curStar))
            // 	{
            //                  panel.SetActive(false);
            //                  CheatManager.CheatStarChest(chestID, curStar);
            //              }
            //              break;
            //          case CheatOption.LevelChest:
            //              if (int.TryParse(inputValues[0].text, out chestID) && int.TryParse(inputValues[1].text, out int curLevel))
            //              {
            //                  panel.SetActive(false);
            //                  CheatManager.CheatLevelChest(chestID, curLevel);
            //              }
            //              break;
            case CheatOption.CardCollection:
                var optionValue = cheatDropdowns[0].options[cheatDropdowns[0].value].text;
                if (optionValue.StartsWith("Album_"))
                {
                    var albumType = optionValue.ToEnum<AlbumType>();
                    CheatManager.CheatCardCollection(albumType);
                }
                else if (optionValue.StartsWith("Card_"))
                {
                    var cardType = optionValue.ToEnum<CardType>();
                    CheatManager.CheatCardCollection(cardType);
                }
                break;
            case CheatOption.QuestEvent:
                if (int.TryParse(inputValues[0].text, out var numItem))
                {
                    if (MySonatFramework.GetService<SceneService>().GetCurrentGamePlacement() == GamePlacement.Gameplay_SkewerJam)
                    {
                        MySonatFramework.GetService<QuestEventService>().AddNumItemInGame(numItem, Vector3.zero);
                    }
                    else
                    {
                        PopupToast.Cretate("Vào gameplay đi ạ !!!");
                    }
                }
                break;
        }
    }

    private void OnDropdown(int number)
    {
        for (int i = 0; i < cheatDropdowns.Length; i++)
        {
            cheatDropdowns[i].gameObject.SetActive(i < number);
        }
    }

    private void OnInputField(int number)
    {
        for (int i = 0; i < inputValues.Length; i++)
        {
            inputValues[i].gameObject.SetActive(i < number);
            inputValues[i].text = "";
        }
    }

    private void SwitchLocalTime(bool state)
    {
        SonatSystem.GetService<SonatTimeService>().SetUsingLocalTime(state);
    }

    public void OnOffCheat()
    {
        panel.SetActive(!panel.activeInHierarchy);
        if (!panel.activeInHierarchy)
        {
            // EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent() { gameState = gameStateBefore });
        }
        else
        {
            EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent() { gameState = GameState.Paused });
        }
    }

    public void ShowFPSLog()
    {
        fpsLog.gameObject.SetActive(true);
    }

    public void ShowDebugLog()
    {
#if sonat_sdk_v2
        SonatDebugView.OpenDebugLogScreen();
#endif
    }

    public void CheckSdkInspector()
    {
#if sonat_sdk_v2
        SonatSdkInspector.GetInstance().ShowInfo();
#endif
    }

    public void StartTransportDebugView()
    {
        ipAdress.Value = inputValues[0].text;
#if sonat_sdk_v2
        SonatDebugView.StartTransportDebugView(ipAdress.Value);
#endif
        Close();
    }

    public void StopTransportDebugView()
    {
        SonatDebugView.StopTransportDebugView();
    }

    // private void OnEnable()
    // {
    // 	StartCoroutine(CountTime());
    // }
    //
    // IEnumerator CountTime()
    // {
    // 	long now = SonatSystemManager.serviceManager.Get<ITimeService>().GetUnixTimeSeconds();
    // 	while (true)
    // 	{
    // 		timeNow.text = now.ToString();
    // 		yield return new WaitForSeconds(1);
    // 		now++;
    // 	}
    // }
    public void InitCardCollectionDropdown()
    {
        List<TMP_Dropdown.OptionData> opts = new List<TMP_Dropdown.OptionData>();

        for (AlbumType i = AlbumType.Album_0; i < AlbumType.MAX; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData()
            {
                text = i.ToString()
            };
            opts.Add(data);
        }

        for (CardType i = CardType.None + 1; i < CardType.MAX; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData()
            {
                text = i.ToString()
            };
            opts.Add(data);
        }

        cheatDropdowns[0].onValueChanged.RemoveAllListeners();
        cheatDropdowns[0].ClearOptions();
        cheatDropdowns[0].options = opts;
    }

}