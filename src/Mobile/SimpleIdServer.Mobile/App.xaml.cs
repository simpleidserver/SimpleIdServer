namespace SimpleIdServer.Mobile;

public partial class App : Application
{
	public static MobileDatabase _database;

	public static MobileDatabase Database
	{
		get
		{
			if (_database != null) return _database;
			_database = new MobileDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sidDB.db3"));
			return _database;
		}
	}

	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

		Routing.RegisterRoute("enrollscanqrcode", typeof(QRCodeScannerPage));
		Routing.RegisterRoute("viewotplist", typeof(ViewOtpListPage));
    }
}
