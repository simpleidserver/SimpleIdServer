using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class QRCodeScannerPage : ContentPage
{
	public QRCodeScannerPage(QRCodeScannerViewModel viewModel)
    {
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
        BindingContext = viewModel;
        InitializeComponent();
        this.Disappearing += HandleDisappearing;
    }

    private void HandleDisappearing(object sender, EventArgs e)
    {
        this.cameraBarCodeReader.Handler.DisconnectHandler();
    }
}