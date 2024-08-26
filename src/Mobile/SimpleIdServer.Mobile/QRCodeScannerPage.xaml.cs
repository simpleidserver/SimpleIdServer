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
        viewModel.Closed += HandleClosed;
        InitializeComponent();
    }

    private void HandleClosed(object sender, EventArgs e)
    {
        if (this.cameraBarCodeReader.Handler != null)
            this.cameraBarCodeReader.Handler.DisconnectHandler();
    }
}