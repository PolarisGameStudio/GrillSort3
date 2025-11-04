using Base.Singleton;
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.UserData;
using MyGame.SkewerJam.Gameplay;
using MyGame.Modules.CardCollection;

public class CheatManager : Singleton<CheatManager>
{
	public static bool unlocked;
	protected override void OnAwake()
	{
		unlocked = PlayerPrefs.HasKey("SONAT_CHEATED");
	}

	// Start is called before the first frame update
	void Start()
	{

	}
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			CheatPanel cheatPanel = PanelManager.Instance.GetPanel<CheatPanel>();
			if (cheatPanel == null)
			{
				PanelManager.Instance.OpenForget<CheatPanel>();
			}
			else
			{
				cheatPanel.OnOffCheat();
			}
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				CheatWinState();
				return;
			}

			if (Input.GetKeyDown(KeyCode.W))
			{
				CheatWin();
				return;
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				CheatLose();
				return;
			}
		}
	}
#endif

	public static void CheatWinState()
	{
		//GamePlayController.Instance.CheatWinState();
	}
	public static void CheatWin()
	{
		GameController.Instance.SetWin(true);
	}
	public static void CheatLose()
	{
		GameController.Instance.Stuck(StuckType.OutOfMove);
	}

	public static void CheatLevel(int level)
	{
		SonatSystem.GetService<UserDataService>().SaveLevel(level, GameMode.Classic);
		PlayLevel(level).Forget();
	}

	private static async UniTaskVoid PlayLevel(int level)
	{
		// await GameplayController.instance.CloseTower();
		GameController.Instance.PlayLevel(level).Forget();
	}

	public static void CheatResource(GameResource resource, int value)
	{
		var inventoryService = SonatSystem.GetService<InventoryService>();
		inventoryService.SetResource(resource, value);
		inventoryService.NotiUpdateResource(resource);
	}

	public static void CheatRemoteConfig(string key, string value)
	{
		if (int.TryParse(value, out var intValue))
		{
			PlayerPrefs.SetInt($"remote_value_{key}", intValue);
		}
		else if (bool.TryParse(value, out var booValue))
		{
			PlayerPrefs.SetInt($"remote_value_{key}", booValue ? 1 : 0);
		}
		else
		{
			PlayerPrefs.SetString($"remote_value_{key}", value);
		}
	}

	public static void CheatPlayerPrefs(string key, string value)
	{
		if (value.Length < 8 && int.TryParse(value, out var intValue))
		{
			PlayerPrefs.SetInt($"{key}", intValue);
		}
		else if (bool.TryParse(value, out var booValue))
		{
			PlayerPrefs.SetInt($"{key}", booValue ? 1 : 0);
		}
		else
		{
			PlayerPrefs.SetString($"{key}", value);
		}
	}

	public static bool IsOpenCheat()
	{
		return PlayerPrefs.GetInt("SONAT_CHEATED", 0) == 1 || Application.isEditor;
	}

	public static CheatLevelSource GetLevelSource()
	{
		CheatLevelSource cheatLevelSource = (CheatLevelSource)PlayerPrefs.GetInt("SONAT_CHEATED_LEVELSOURCE", 0);

		if (cheatLevelSource == CheatLevelSource.Drive && !IsOpenCheat())
		{
			cheatLevelSource = CheatLevelSource.Resources;
		}

		return cheatLevelSource;
	}

	public static void CheatCardCollection(CardType cardType)
	{
		var cardCollectionService = SonatSystem.GetService<CardCollectionService>();
		cardCollectionService.ForceUnboxPackCard(cardType);
	}
}


public enum CheatOption
{
	Level,
	Win,
	WinState,
	Lose,
	Resource,
	RemoteConfig,
	PlayerPrefs,
	GDLevel,
	StarChest,
	LevelChest,
	TransportTracking,
	CardCollection,
	MAX,
}

public enum CheatLevelSource
{
	Resources,
	Drive,
	MAX,
}