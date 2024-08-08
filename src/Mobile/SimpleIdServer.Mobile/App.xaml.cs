using Microsoft.Extensions.Options;
using SimpleIdServer.Mobile.Clients;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;

namespace SimpleIdServer.Mobile;

public partial class App : Application
{
    private readonly CredentialListState _credentialListState;
	private readonly OtpListState _otpListState;
    private readonly VerifiableCredentialListState _verifiableCredentialListState;
    private readonly MobileSettingsState _mobileSettingsState;
    private readonly DidRecordState _didRecordState;
    private readonly IServiceProvider _serviceProvider;
    public static MobileDatabase _database;

    public static GotifyNotificationListener Listener = GotifyNotificationListener.New();
    public static CredentialOfferListener CredentialOfferListener = CredentialOfferListener.New();

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

	public App(
        CredentialListState credentialListState, 
        OtpListState otpListState, 
        VerifiableCredentialListState verifiableCredentialListState, 
        MobileSettingsState mobileSettingsState,
        DidRecordState didRecordState,
        IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_credentialListState = credentialListState;
        _otpListState = otpListState;
        _verifiableCredentialListState = verifiableCredentialListState;
        _mobileSettingsState = mobileSettingsState;
        _didRecordState = didRecordState;
        _serviceProvider = serviceProvider;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await Database.Init();
		await _otpListState.Load();
		await _credentialListState.Load();
        await _verifiableCredentialListState.Load();
        await _mobileSettingsState.Load();
        await _didRecordState.Load();
        NavigationService = _serviceProvider.GetRequiredService<INavigationService>();
        await InitGotify();
        Listener.NotificationReceived += async (sender, e) =>
        {
            await HandleNotificationReceived(sender, e);
        };
        CredentialOfferListener.CredentialOfferReceived += (sender, e) =>
        {
            HandleCredentialOfferReceived(sender, e);
        };
    }

    private async Task HandleNotificationReceived(object sender, NotificationEventArgs e)
    {
        var notificationPage = await NavigationService.DisplayModal<NotificationPage>();
        await Task.Delay(1000);
        notificationPage.Display(e.Notification);
    }

    private void HandleCredentialOfferReceived(object sender, CredentialOfferEventArgs e)
    {

    }

    private async Task InitGotify()
	{
        var mobileSettings = _mobileSettingsState.Settings;
        var opts = _serviceProvider.GetRequiredService<IOptions<MobileOptions>>();
        try
        {
            if (string.IsNullOrWhiteSpace(mobileSettings.GotifyPushToken))
            {
                var sidServerClient = _serviceProvider.GetRequiredService<ISidServerClient>();
                var pushToken = await sidServerClient.AddGotifyConnection();
                mobileSettings.GotifyPushToken = pushToken;
            }

            var instance = GotifyNotificationListener.New();
            await instance.Start(
                opts.Value.WsServer,
                mobileSettings.GotifyPushToken, 
                CancellationToken.None);
        }
        catch
        {

        }
    }
}