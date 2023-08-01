namespace SimpleIdServer.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

		Routing.RegisterRoute("enroll", typeof(EnrollPage));
		Routing.RegisterRoute("scanqrcode", typeof(QRCodeScannerPage));
	}
}
