using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Scripts.SO.SkewerJam.Gameplay;
using MyGame.SkewerJam.UI.Loading;
using Sonat.Enums;
using SonatFramework.Scripts.Helper;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.UserData;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private GameLogicHandler gameLogicHandler;
        [SerializeField] private ComboManager comboManager;
        [SerializeField] private TutorialManager tutorialManager;

        [Header("UI")]
        [SerializeField] private GameplayScreen gameplayScreen;
        [SerializeField] private GameViewport gameViewport;

        [Header("Game config")]
        [SerializeField]
        private GameplayConfig_SkewerJam gameConfig;
        public GameplayConfig_SkewerJam GameConfig => gameConfig;

        [Header("SEA")]
        [SerializeField] private float delaySoundIngame = 3.5f;


        public LevelGenerator LevelGenerator => levelGenerator;
        public GameLogicHandler GameLogicHandler => gameLogicHandler;
        public GameViewport GameViewport => gameViewport;
        public GameState GameState => gameState;
        public ComboManager ComboManager => comboManager;
        public GameResult GameResult => gameResult;
        public GameplayScreen GameplayScreen => gameplayScreen;


        // private int level;
        private int level;
        public int Level => level;
        private GameState gameState;
        private GameResult gameResult;
        private EventBinding<GameStateChangeEvent> gameStateChangeEvent;
        private EventBinding<PanelUpdatedEvent> onPanelsUpdatedEvent;
        private AudioId bgm;

        private IntDataPref _checkRewardFreeLives = new IntDataPref("check_reward_free_lives");

        public static event Action OnPlayTutorial;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Initialize();
            PlayLevel(level).Forget();
            onPanelsUpdatedEvent = new EventBinding<PanelUpdatedEvent>(OnPanelsUpdated);
        }

        private void OnPanelsUpdated(PanelUpdatedEvent eventData)
        {
            if (PanelManager.Instance.HasAnyPopupPauseGame())
            {
                ChangeGameState(GameState.Paused);
            }
        }

        void OnDestroy()
        {
            EventBus<PanelUpdatedEvent>.Deregister(onPanelsUpdatedEvent);
        }

        private void Initialize()
        {
            gameViewport.Init();
            level = MySonatFramework.userDataService.GetLevel(GameMode.Classic);
            gameStateChangeEvent = new EventBinding<GameStateChangeEvent>(OnGameStateChangedEvent);
        }

        private void OnGameStateChangedEvent(GameStateChangeEvent @event)
        {
            gameState = @event.gameState;
            Debug.Log("<color=green>[GameController]</color> OnGameStateChangedEvent: " + gameState);
        }

        #region Load level
        public async UniTask PlayLevel(int level, bool force = false)
        {
            SonatUtils.ExecuteNextFrame(() =>
            {
                ChangeGameState(GameState.Loading);
            }, 2);
            ClearLevel();

            MySonatFramework.GetService<UserDataService>().SaveLevel(level, GameMode.Classic);

            LoadingHelper.CheckShowLoadingInGameplay();

            Debug.Log("<color=green>[GameController]</color> PlayLevel: " + level);
            SonatUtils.DelayCall(delaySoundIngame, () =>
            {
                bgm = GameplayHelper.GetBGMIngame();
                MySonatFramework.GetService<AudioService>().PlayMusic(bgm);
            }, this);


            this.level = level;

            InitLevel();

            await levelGenerator.GenerateLevel(level);
            EventBus<LevelStartedEvent>.Raise(new LevelStartedEvent() { level = level, gameMode = GameMode.Classic });

            LoadingHelper.CompleteLoadingInGameplay(() => PlayStartGame(() =>
            {
                ChangeGameState(GameState.Playing);

                OnPlayTutorial?.Invoke();
            }).Forget());
        }

        public async UniTask PlayStartGame(Action onComplete = null)
        {
            foreach (var grill in gameLogicHandler.GrillManager.ListGrills)
            {
                grill.OpenGrillWhenStart();
            }

            await UniTask.Delay(500);
            await gameLogicHandler.OrderManager.PlayAppearOrders();
            onComplete?.Invoke();
        }

        public void InitLevel()
        {
            gameplayScreen.InitLevel(level);
            levelGenerator.Init();
            gameLogicHandler.Init();
            comboManager.Initialize();

            tutorialManager.Init();
        }

        public void ClearLevel()
        {
            gameplayScreen.ClearLevel();

            levelGenerator.Clear();
            gameLogicHandler.Clear();
            comboManager.Clear();

            tutorialManager.Clear();
            gameResult = GameResult.None;
        }

        #endregion

        public void ChangeGameState(GameState newGameState)
        {
            EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent() { gameState = newGameState });
        }

        public void SetWin(bool forcePlayWin = false)
        {
            Debug.Log("<color=green>[GameController]</color> Win");
            if (gameState == GameState.GameOver) return;

            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { level = level, gameMode = GameMode.Classic, success = true });

            // GameplayStateSaver.Instance.SetStatus(); // không lưu trạng thái
            ChangeGameState(GameState.GameOver);

            gameResult = GameResult.Win;

            if (forcePlayWin)
            {
                Win();
            }
        }

        public void TryWin()
        {
            if (gameResult == GameResult.Win)
            {
                gameResult = GameResult.None;
                Win();
            }
        }

        public async UniTaskVoid Win()
        {
            await UniTask.Delay(1500);
            gameplayScreen.HideCurrencies();

            var log = new EarnResourceLogData()
            {
                spendType = "pumpkin",
                spendId = "pumpkin",
                source = "gameplay"
            };
            var winReward = gameConfig.GetWinReward(LevelDifficulty.Easy1);
            MySonatFramework.GetService<InventoryService>().AddResource(winReward.resource, winReward.quantity, log, false);

            var data = new WinPanelBase.Data()
            {
                level = level,
                reward = winReward,
                nextLevel = () => NextLevel()
            };
            PanelManager.Instance.OpenForget<WinPanel_SkewerJam>(data);
            // NextLevel();

            if (level == 5)
            {
                _checkRewardFreeLives.Value = 1;
            }
        }

        public async UniTaskVoid Stuck(StuckType stuckType)
        {
            Debug.Log("<color=red>[GameController]</color> Stuck: " + stuckType);
            if (gameState == GameState.GameOver) return;

            EventBus<LevelStuckEvent>.Raise(new LevelStuckEvent()
            {
                level = level,
                gameMode = GameMode.Classic,
                cause = stuckType.ToString()
            });

            ChangeGameState(GameState.GameOver);


            await UniTask.Delay(500);
            // var showPopupContinue = GameLogicHandler.WaitingGrillManager.ListWaitingGrills.Where(e => e.IsActive == false).Count() > 0;
            if (CanRevive())
            {
                PopupContinue.Data data = new PopupContinue.Data()
                {
                    onPlayOn = (by, objectParams) => Revive(stuckType, by, objectParams).Forget(),
                    onClose = () => Lose(stuckType).Forget(),
                    stuckType = stuckType
                };
                PanelManager.Instance.OpenForget<PopupSoClose>(data);
            }
            else
            {
                Lose(stuckType).Forget();
            }

        }

        private bool CanRevive()
        {
            return true;
            // return GameLogicHandler.OrderManager.ListOrders.Where(e => e.IsActive == false).Count() > 0;
        }

        private async UniTaskVoid Revive(StuckType stuckType, string by, object[] objectParams = null)
        {
            ChangeGameState(GameState.Playing);
            MySonatFramework.GetService<AudioService>().PlayMusic(bgm);
            EventBus<LevelContinueEvent>.Raise(new LevelContinueEvent() { by = by });
            await UniTask.Delay(1000);
            switch (stuckType)
            {
                case StuckType.OutOfMove:
                    switch (by)
                    {
                        case "play_on_add_trays":
                            if (GameLogicHandler.OrderManager.ListOrders.Where(e => e.IsActive == false).Count() > 0)
                            {
                                var orderManager = GameLogicHandler.OrderManager;
                                orderManager.Unlock(isRescue: true);
                            }
                            else
                            {
                                // Sử dụng spatula
                                var boosterManager = GameLogicHandler.BoosterManager;
                                boosterManager.ForceUseBooster(GameResource.BoosterSpatula).Forget();

                                // order tiếp theo cũng phải là rescue
                                gameLogicHandler.OrderManager.IsForceRescue = true;
                            }
                            break;
                        case "play_on_clear_one_plate":
                            var waitingGrillManager = GameLogicHandler.WaitingGrillManager;
                            var success = waitingGrillManager.ClearOnePlate();
                            if (success == false)
                            {
                                Lose(stuckType).Forget();
                            }
                            break;
                    }

                    break;
            }
        }

        public async UniTaskVoid Lose(StuckType stuckType)
        {
            MySonatFramework.livesService.ReduceLive(1, "lose");
            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { level = level, gameMode = GameMode.Classic, success = false });
            PanelManager.Instance.OpenPanelByName<PopupLose_SkewerJam>("PopupLose_SkewerJam");
        }

        private void NextLevel()
        {
            level = MySonatFramework.userDataService.GetLevel(GameMode.Classic);

            if (level >= GameRemoteConfigValue.levelForceHome)
            {
                SonatSDKAdapter.ShowInterAds("go_home", () =>
                {
                    GameplayHelper.GoHome();
                });
            }
            else
            {
                PlayLevel(level).Forget();
            }
        }

        public void Replay()
        {
            PlayLevel(level).Forget();
        }

        public bool CheckBlockUI()
        {
            var boosterManager = GameLogicHandler.BoosterManager;
            return gameState != GameState.Playing || gameLogicHandler.BlockUIWhenEnd || boosterManager.IsForceUseBooster();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PanelManager.Instance.OpenPanel<PopupHightlightGameplay>();
            }
        }

#endif
    }
}