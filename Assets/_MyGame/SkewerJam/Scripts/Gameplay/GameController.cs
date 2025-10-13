using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyGame.SkewerJam.Gameplay.Helpers;
using MyGame.SkewerJam.Scripts.SO.SkewerJam.Gameplay;
using Sonat.Enums;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Scripts.Utils;
using SonatFramework.Systems.AudioManagement;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.InventoryManagement;
using SonatFramework.Systems.InventoryManagement.GameResources;
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

        [Header("UI")]
        [SerializeField] private GameplayScreen gameplayScreen;
        [SerializeField] private GameViewport gameViewport;

        [Header("Game config")]
        [SerializeField]
        private GameplayConfig_SkewerJam gameConfig;
        public GameplayConfig_SkewerJam GameConfig => gameConfig;


        public LevelGenerator LevelGenerator => levelGenerator;
        public GameLogicHandler GameLogicHandler => gameLogicHandler;
        public GameViewport GameViewport => gameViewport;
        public GameState GameState => gameState;
        public ComboManager ComboManager => comboManager;
        // private int level;
        private int level;
        public int Level => level;
        private GameState gameState;
        private EventBinding<GameStateChangeEvent> gameStateChangeEvent;
        private AudioId bgm;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Initialize();
            PlayLevel(level).Forget();
            PanelManager.Instance.OnPanelsUpdated += OnPanelsUpdated;
        }

        private void OnPanelsUpdated()
        {
            if (PanelManager.Instance.HasAnyPopupPauseGame())
            {
                ChangeGameState(GameState.Paused);
            }
        }

        void OnDestroy()
        {
            if (PanelManager.Instance != null)
                PanelManager.Instance.OnPanelsUpdated -= OnPanelsUpdated;
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
            ClearLevel();

            MySonatFramework.GetService<UserDataService>().SaveLevel(level, GameMode.Classic);
            var popupLoading = PanelManager.Instance.OpenPanelByName<PopupLoading>("PopupLoading", new UIData().Add("Time", 2f));
            Debug.Log("<color=green>[GameController]</color> PlayLevel: " + level);
            SonatUtils.DelayCall(0.75f, () =>
            {
                bgm = UnityEngine.Random.Range(0, 2) == 0 ? AudioId.BGM_Ingame_Halloween_Grill_sort : AudioId.BGM_Ingame_Halloween_01_Grill_sort;
                MySonatFramework.GetService<AudioService>().PlayMusic(bgm);
            }, this);


            this.level = level;
            ChangeGameState(GameState.Loading);

            Debug.Log("<color=green>[GameController]</color> PlayLevel: " + level);
            InitLevel();

            await levelGenerator.GenerateLevel(level);

            ChangeGameState(GameState.Playing);
            EventBus<LevelStartedEvent>.Raise(new LevelStartedEvent() { level = level, gameMode = GameMode.Classic });

            PlayStartGame().Forget();
            // if (GameplayHelper.CheckStart() == false)
            // {
            //     PanelManager.Instance.OpenPanel<PopupWarningEnergy_SkewerJam>(new UIData().Add("GamePlacement", GamePlacement.Gameplay_SkewerJam));
            // }
        }

        public async UniTask PlayStartGame()
        {
            await UniTask.Delay(1500);
            foreach (var grill in gameLogicHandler.GrillManager.ListGrills)
            {
                grill.GrillVisual.OpenGrill();
            }

            await UniTask.Delay(500);
            await gameLogicHandler.OrderManager.PlayAppearOrders();
        }

        public void InitLevel()
        {
            gameplayScreen.InitLevel(level);
            levelGenerator.Init();
            gameLogicHandler.Init();
            comboManager.Initialize();
        }

        public void ClearLevel()
        {
            levelGenerator.Clear();
            gameLogicHandler.Clear();
            comboManager.Clear();
        }

        #endregion

        public void ChangeGameState(GameState newGameState)
        {
            EventBus<GameStateChangeEvent>.Raise(new GameStateChangeEvent() { gameState = newGameState });
        }

        public async UniTaskVoid Win()
        {
            Debug.Log("<color=green>[GameController]</color> Win");
            if (gameState == GameState.GameOver) return;

            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { level = level, gameMode = GameMode.Classic, success = true });
            var newLevel = MySonatFramework.userDataService.GetLevel(GameMode.Classic) + 1;
            MySonatFramework.userDataService.SaveLevel(newLevel, GameMode.Classic);

            // GameplayStateSaver.Instance.SetStatus(); // không lưu trạng thái
            ChangeGameState(GameState.GameOver);
            GameplayHelper.IsWin = true;
            // PopupToast.Cretate("You Win");

            await UniTask.Delay(1000);
            // // MySonatFramework.audioService.StopMusic();
            // PopupPreWin popupPreWin = await PanelManager.Instance.OpenPanelAsync<PopupPreWin_SkewerJam>();
            // await UniTask.Delay((int)(popupPreWin.delay * 1000));

            gameplayScreen.HideCurrencies();

            var log = new EarnResourceLogData()
            {
                spendType = "pumpkin",
                spendId = "pumpkin",
                source = "gameplay"
            };
            var winReward = gameConfig.GetWinReward(LevelDifficulty.Normal);
            MySonatFramework.GetService<InventoryService>().AddResource(winReward.resource, winReward.quantity, log, false);

            var data = new WinPanelBase.Data()
            {
                level = level,
                reward = winReward,
                nextLevel = () => NextLevel()
            };
            PanelManager.Instance.OpenForget<WinPanel_SkewerJam>(data);
            // NextLevel();
        }

        public async UniTaskVoid Stuck(StuckType stuckType)
        {
            Debug.Log("<color=red>[GameController]</color> Stuck: " + stuckType);
            if (gameState == GameState.GameOver) return;

            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent()
            {
                level = level,
                gameMode = GameMode.Classic,
                success = false
            });

            ChangeGameState(GameState.GameOver);


            await UniTask.Delay(1000);
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
            return GameLogicHandler.OrderManager.ListOrders.Where(e => e.IsActive == false).Count() > 0;
        }

        private async UniTaskVoid Revive(StuckType stuckType, string by, object[] objectParams = null)
        {
            ChangeGameState(GameState.Playing);
            MySonatFramework.GetService<AudioService>().PlayMusic(bgm);
            EventBus<LevelStartedEvent>.Raise(new LevelStartedEvent() { level = level, gameMode = GameMode.Classic });
            await UniTask.Delay(1000);
            switch (stuckType)
            {
                case StuckType.OutOfMove:
                    switch (by)
                    {
                        case "play_on_add_trays":
                            var orderManager = GameLogicHandler.OrderManager;
                            orderManager.Unlock(isRescue: true);

                            // // cộng thêm 2 platesvar waitingManager = GameLogicHandler.WaitingGrillManager;
                            // var waitingManager = GameLogicHandler.WaitingGrillManager;
                            // await waitingManager.AddPlate();
                            // await waitingManager.AddPlate();
                            break;
                    }

                    break;
            }
        }

        public async UniTaskVoid Lose(StuckType stuckType)
        {
            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { level = level, gameMode = GameMode.Classic, success = false });
            PanelManager.Instance.OpenPanelByName<PopupLose_SkewerJam>("PopupLose_SkewerJam");
            GameplayHelper.IsWin = false;
        }

        private void NextLevel()
        {
            level = MySonatFramework.userDataService.GetLevel(GameMode.Classic);
            PlayLevel(level).Forget();
        }

        public void Replay()
        {
            PlayLevel(level).Forget();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Win();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Stuck(StuckType.OutOfMove);
            }
        }

#endif
    }
}