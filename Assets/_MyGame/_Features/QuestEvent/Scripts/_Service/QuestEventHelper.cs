namespace MyGame.Modules.QuestEvent
{
    public static class QuestEventHelper
    {


        public static int GetQuestIndexView()
        {
            var claimedQuestIndex = MySonatFramework.GetService<QuestEventService>().ClaimedQuestIndex;
            return claimedQuestIndex + 1;
        }

        public static int GetNumberQuestView()
        {
            var questEventService = MySonatFramework.GetService<QuestEventService>();
            var currentItem = questEventService.CurrentItem;

            if (CheckClaimedQuest())
            {
                return questEventService.config.listMilestones[GetQuestIndexView()].numItem;
            }
            else
            {
                return currentItem;
            }
        }

        public static bool CheckClaimedQuest()
        {
            var questEventService = MySonatFramework.GetService<QuestEventService>();
            return GetQuestIndexView() < questEventService.CurrentQuestIndex;
        }
    }
}