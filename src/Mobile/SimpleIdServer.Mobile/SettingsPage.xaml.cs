using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;
public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsPageViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}