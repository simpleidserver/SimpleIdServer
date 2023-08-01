namespace SimpleIdServer.Mobile;
using SimpleIdServer.Mobile.Stores;

public partial class EnrollPage : ContentPage
{
	private readonly ICertificateStore _certificateStore;

    public EnrollPage(ICertificateStore certificateStore)
	{
		InitializeComponent();
		_certificateStore = certificateStore;
	}

	private async void OnEnrollClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("scanqrcode");
		// await Navigation.PushModalAsync(new QRCodeScannerPage(_certificateStore));
    }
}