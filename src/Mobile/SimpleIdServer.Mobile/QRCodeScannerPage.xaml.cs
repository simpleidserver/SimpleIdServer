using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class QRCodeScannerPage : ContentPage
{
	public QRCodeScannerPage(QRCodeScannerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}