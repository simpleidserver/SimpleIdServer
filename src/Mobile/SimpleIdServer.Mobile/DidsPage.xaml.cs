using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class DidsPage : ContentPage
{
	public DidsPage(DidsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}