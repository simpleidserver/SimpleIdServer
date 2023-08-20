using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class QRCodeInputPage : ContentPage
{
	public QRCodeInputPage(QRCodeScannerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}