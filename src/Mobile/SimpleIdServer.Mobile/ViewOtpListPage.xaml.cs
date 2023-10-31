using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class ViewOtpListPage : ContentPage
{
    private readonly ViewOtpListViewModel _viewModel;

	public ViewOtpListPage(ViewOtpListViewModel viewModel)
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