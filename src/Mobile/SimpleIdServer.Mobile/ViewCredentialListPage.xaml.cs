using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class ViewCredentialListPage : ContentPage
{
    private readonly ViewCredentialListViewModel _viewModel;

	public ViewCredentialListPage(ViewCredentialListViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
    }
}