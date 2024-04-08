using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;
public partial class EnrollPage : ContentPage
{
	private readonly EnrollViewModel _viewModel;

    public EnrollPage(EnrollViewModel viewModel)
	{
        _viewModel = viewModel;
        BindingContext = _viewModel;
		InitializeComponent();
	}
}