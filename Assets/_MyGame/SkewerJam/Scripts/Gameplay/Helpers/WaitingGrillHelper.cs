using System.Linq;
using MyGame.SkewerJam.Objects;

namespace MyGame.SkewerJam.Gameplay.Helpers
{
    public static class WaitingGrillHelper
    {
        private const int MAX_BLOCK_CLICK_TO_WARNING = 2; // Số lần chặn click sẽ là MAX_BLOCK_CLICK_TO_WARNING -1
        private const float MAX_WARNING = MAX_BLOCK_CLICK_TO_WARNING + 1;

        public static bool IsWarning = false;
        private static int _countWarning = 0;


        private static WaitingGrillManager waitingGrillManager => GameController.Instance.GameLogicHandler.WaitingGrillManager;

        public static bool CheckWarning()
        {
            var listWaitingGrill = waitingGrillManager.ListWaitingGrills;
            var listEmptyWaitingGrill = listWaitingGrill.Where(e => e.IsActive && e.GetSlots().Where(s => s.GetItem() == null).Count() > 0).ToList();
            return listEmptyWaitingGrill.Count == 1;
        }

        public static bool CheckWarningCount()
        {
            return _countWarning < MAX_WARNING;
        }


        public static bool Warning()
        {
            if (CheckWarningCount() == false) return false;

            IsWarning = true;
            var listWaitingGrill = waitingGrillManager.ListWaitingGrills;
            var listEmptyWaitingGrill = listWaitingGrill.Where(e => e.IsActive && e.GetSlots().Where(s => s.GetItem() == null).Count() > 0).ToList();
            listEmptyWaitingGrill[0].Visual.PlayWarning();
            _countWarning++;
            return true;
        }

        public static void ResetWarning()
        {
            IsWarning = false;
            _countWarning = 0;
        }
    }
}