using SimpleIdServer.Mobile.Helpers;

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

    public static IPlatformHelpers PlatformHelper;

    public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		MainPage = new AppShell();

		Routing.RegisterRoute("enrollscanqrcode", typeof(QRCodeScannerPage));
        Routing.RegisterRoute("enrollsubmitqrcode", typeof(QRCodeInputPage));

		PlatformHelper = serviceProvider.GetService<IPlatformHelpers>();
    }
}
