using System.Linq;
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
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private GameLogicHandler gameLogicHandler;

        [Header("UI")]
        [SerializeField] private GameplayScreen gameplayScreen;
        [SerializeField] private GameViewport gameViewport;

        [Header("Game config")]
        [SerializeField]
        private GameConfig gameConfig;
        public GameConfig GameConfig => gameConfig;


        public LevelGenerator LevelGenerator => levelGenerator;
        public GameLogicHandler GameLogicHandler => gameLogicHandler;
        public GameViewport GameViewport => gameViewport;
        public GameState GameState => gameState;
        // private int level;
        private int level;
        public int Level => level;
        private GameState gameState;
        private EventBinding<GameStateChangeEvent> gameStateChangeEvent;

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
            level = MySonatFramework.userDataService.GetLevel(GameMode.SkewerJam);
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


            // PanelManager.Instance.OpenPanelByName<PopupLoading>("PopupLoading_SkewerJam", new UIData().Add("Time", 1f));
            Debug.Log("<color=green>[GameController]</color> PlayLevel: " + level);
            // SonatUtils.DelayCall(0.75f, () =>
            // {
            //     var bgm = UnityEngine.Random.Range(0, 2) == 0 ? AudioId.BGM_Ingame_Halloween_Grill_sort : AudioId.BGM_Ingame_Halloween_01_Grill_sort;
            //     MySonatFramework.GetService<AudioService>().PlayMusic(bgm);
            // }, this);


            this.level = level;
            ChangeGameState(GameState.Loading);

            Debug.Log("<color=green>[GameController]</color> PlayLevel: " + level);
            InitLevel();

            await levelGenerator.GenerateLevel(level);

            ChangeGameState(GameState.Playing);
            EventBus<LevelStartedEvent>.Raise(new LevelStartedEvent() { level = level, gameMode = GameMode.SkewerJam });

            // if (GameplayHelper.CheckStart() == false)
            // {
            //     PanelManager.Instance.OpenPanel<PopupWarningEnergy_SkewerJam>(new UIData().Add("GamePlacement", GamePlacement.Gameplay_SkewerJam));
            // }
        }

        public void InitLevel()
        {
            gameplayScreen.InitLevel(level);
            levelGenerator.Init();
            gameLogicHandler.Init();
        }

        public void ClearLevel()
        {
            levelGenerator.Clear();
            gameLogicHandler.Clear();
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

            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { level = level, gameMode = GameMode.SkewerJam, success = true });
            var newLevel = MySonatFramework.userDataService.GetLevel(GameMode.SkewerJam) + 1;
            MySonatFramework.userDataService.SaveLevel(newLevel, GameMode.SkewerJam);

            // GameplayStateSaver.Instance.SetStatus(); // không lưu trạng thái
            ChangeGameState(GameState.GameOver);
            GameplayHelper.IsWin = true;
            // PopupToast.Cretate("You Win");

            await UniTask.Delay(2000);
            // MySonatFramework.audioService.StopMusic();
            PopupPreWin popupPreWin = await PanelManager.Instance.OpenPanelAsync<PopupPreWin_SkewerJam>();
            await UniTask.Delay((int)(popupPreWin.delay * 1000));

            gameplayScreen.HideCurrencies();

            var log = new EarnResourceLogData()
            {
                spendType = "pumpkin",
                spendId = "pumpkin",
                source = "gameplay"
            };
            // MySonatFramework.GetService<InventoryService>().AddResource(GameResource.Energy, gameLogicHandler.Pumpkin, log, false);

            var data = new WinPanelBase.Data()
            {
                level = level,
                reward = new ResourceData() { resource = GameResource.Coin, quantity = gameLogicHandler.Pumpkin },
                nextLevel = () => NextLevel()
            };
            PanelManager.Instance.OpenForget<WinPanel_SkewerJam>(data);
            NextLevel();
        }

        public async UniTaskVoid Stuck(StuckType stuckType)
        {
            Debug.Log("<color=red>[GameController]</color> Stuck: " + stuckType);
            if (gameState == GameState.GameOver) return;

            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent()
            {
                level = level,
                gameMode = GameMode.SkewerJam,
                success = false
            });

            ChangeGameState(GameState.GameOver);


            await UniTask.Delay(3000);
            Lose(stuckType);
            // var showPopupContinue = GameLogicHandler.WaitingGrillManager.ListWaitingGrills.Where(e => e.IsActive == false).Count() > 0;
            // PopupContinue.Data data = new PopupContinue.Data()
            // {
            //     onPlayOn = (by, objectParams) => Revive(stuckType, by, objectParams).Forget(),
            //     onClose = () => Lose(stuckType).Forget(),
            //     stuckType = stuckType
            // };
            // data.Add("PopupContinueName", "PopupContinue_SkewerJam");
            // data.Add("UseUILevel", false);
            // data.Add("ShowPopupContinue", showPopupContinue);

            // PanelManager.Instance.OpenForget<PopupWarningStuck>(data);


        }

        private async UniTaskVoid Revive(StuckType stuckType, string by, object[] objectParams = null)
        {
            ChangeGameState(GameState.Playing);

            EventBus<LevelStartedEvent>.Raise(new LevelStartedEvent() { level = level, gameMode = GameMode.SkewerJam });
            await UniTask.Delay(1000);
            switch (stuckType)
            {
                case StuckType.OutOfMove:
                    switch (by)
                    {
                        case "play_on_add_trays":
                            // var orderManager = GameLogicHandler.OrderManager;
                            // orderManager.Unlock();

                            // cộng thêm 2 plates

                            var waitingManager = GameLogicHandler.WaitingGrillManager;
                            waitingManager.Unlock();
                            await UniTask.Delay(1000);
                            waitingManager.Unlock();
                            break;
                    }

                    break;
                case StuckType.OutOfTime:
                    switch (by)
                    {
                        case "play_on_add_energies":
                            PopupToast.Cretate("Add Energies!");
                            break;
                    }

                    break;
            }
        }

        public async UniTaskVoid Lose(StuckType stuckType)
        {
            EventBus<LevelEndedEvent>.Raise(new LevelEndedEvent() { level = level, gameMode = GameMode.SkewerJam, success = false });
            PanelManager.Instance.OpenPanelByName<PopupLose_SkewerJam>("PopupLose_SkewerJam");
            GameplayHelper.IsWin = false;
        }

        private void NextLevel()
        {
            level = MySonatFramework.userDataService.GetLevel(GameMode.SkewerJam);
            PlayLevel(level).Forget();
        }

        public void Replay()
        {
            PlayLevel(level).Forget();
        }

        public void Continue()
        {
            ChangeGameState(GameState.Playing);
        }

#if UNITY_EDITOR
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.W))
            // {
            //     Win();
            // }

            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     Stuck(StuckType.SkewerJam_OutOfSpace);
            // }

            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     PopupPreWin popupPreWin = PanelManager.Instance.OpenPanelByName<PopupPreWin>("PopupPreWin_SkewerJam");
            // }
        }

#endif
    }
}