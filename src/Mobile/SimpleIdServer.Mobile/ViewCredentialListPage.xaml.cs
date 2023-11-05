using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class ViewCredentialListPage : ContentPage
{
    private readonly ViewCredentialListViewModel _viewModel;

	public ViewCredentialListPage(ViewCredentialListViewModel viewModel)
    {
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
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