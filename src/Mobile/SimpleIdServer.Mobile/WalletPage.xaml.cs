using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;
public partial class WalletPage : ContentPage
{
	public WalletPage(WalletViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}