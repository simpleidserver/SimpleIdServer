using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;

namespace SimpleIdServer.Mobile;

public partial class DidsPage : ContentPage
{
	public DidsPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        var view = sender as Button;
        FlyoutBase.ShowAtta
        var h = view.Handler;
        var pv = h.PlatformView;
        string ss = "";
        /*
        element.ContextFlyout.ShowAt(element, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions
        {
            Position = new Windows.Foundation.Point(point.Value.X, point.Value.Y),
            //Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom
        });
        */
    }
}