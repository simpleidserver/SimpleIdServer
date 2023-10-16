using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class ViewCredentialListPage : ContentPage
{
    private readonly ViewCredentialListViewModel _viewModel;

	public ViewCredentialListPage(ViewCredentialListViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
#if ANDROID
        InitializeComponent();
#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.Load();
    }
}