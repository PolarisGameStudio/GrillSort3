using Sonat.Enums;
using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;
using SonatFramework.Systems.SceneManagement;

public class ShopPanel : ShopPanelBase
{
    public override void Open(UIData uiData)
    {
        base.Open(uiData);

        SonatSDKAdapter.SetBanner(false);
    }

    public override void Close()
    {
        base.Close();

        if (MySonatFramework.GetService<SonatSceneService>().GetCurrentGamePlacement() == GamePlacement.Gameplay_SkewerJam)
        {
            MySonatFramework.TryShowBanner();
        }
    }
}
