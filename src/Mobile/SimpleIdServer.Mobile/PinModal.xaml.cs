using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SimpleIdServer.Mobile.ViewModels;

namespace SimpleIdServer.Mobile;

public partial class PinModal : ContentPage
{
    private PinModalViewModel _viewModel;

	public PinModal(PinModalViewModel viewModel)
    {
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
	}

    public string Pin
    {
        get
        {
            return _viewModel?.Pin;
        }
    }

    public PinModalViewModel ViewModel
    {
        get
        {
            return _viewModel;
        }
    }
}