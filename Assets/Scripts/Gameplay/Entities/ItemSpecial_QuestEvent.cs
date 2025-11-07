using DG.Tweening;
using Gameplay.Entities;
using MyGame.Modules.QuestEvent;
using MyGame.Modules.SubInventory;
using SonatFramework.Systems;
using SonatFramework.Systems.EventBus;
using SonatFramework.Systems.SettingsManagement.Vibation;

public class ItemSpecial_QuestEvent : Item
{
    public override bool OnCustomSelect()
    {
        transform.DOKill();
        if (Slot != null)
        {
            Slot.ItemOut();
        }

        SetSlot(null);
        SetSelected(true);

        MySonatFramework.GetService<VibrationService>().Vibrate(50);
        MyGame.SkewerJam.Gameplay.GameFactory.Instance.ReturnEntity(this);

        MySonatFramework.GetService<QuestEventService>().AddNumItemInGame(1, transform.position);

        return true;

    }
}
