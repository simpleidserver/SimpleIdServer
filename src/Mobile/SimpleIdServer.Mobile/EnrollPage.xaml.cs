namespace SimpleIdServer.Mobile;
public partial class EnrollPage : ContentPage
{
    public EnrollPage()
	{
		InitializeComponent();
	}

	private async void OnEnrollClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("scanqrcode");
    }
}