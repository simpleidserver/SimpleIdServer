using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class QRCodeInputPage : ContentPage
{
    public QRCodeInputPage(QRCodeInputViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}