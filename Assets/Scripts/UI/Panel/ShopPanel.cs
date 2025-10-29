using SonatFramework.Scripts.Feature.Shop.UI;
using SonatFramework.Scripts.SonatSDKAdapterModule;
using SonatFramework.Scripts.UIModule;

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
        MySonatFramework.TryShowBanner();
    }
}
