using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;

namespace SimpleIdServer.Mobile;

public partial class App : Application
{
    private readonly CredentialListState _credentialListState;
	private readonly OtpListState _otpListState;
	private readonly IServiceProvider _serviceProvider;
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

	public static INavigationService NavigationService { get; private set; }

	public App(CredentialListState credentialListState, OtpListState otpListState, IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_credentialListState = credentialListState;
        _otpListState = otpListState;
		_serviceProvider = serviceProvider;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
		await _otpListState.Load();
		await _credentialListState.Load();
        NavigationService = _serviceProvider.GetRequiredService<INavigationService>();
    }
}
