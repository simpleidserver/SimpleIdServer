using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class ProfilePage : ContentPage
{
	private readonly ProfileViewModel _viewModel;

	public ProfilePage(ProfileViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		_viewModel.Stop();
    }
}