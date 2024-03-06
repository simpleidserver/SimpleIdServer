using Microsoft.Extensions.Options;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Mobile;

public partial class App : Application
{
    private readonly CredentialListState _credentialListState;
	private readonly OtpListState _otpListState;
	private readonly IServiceProvider _serviceProvider;
    public static MobileDatabase _database;

    public static GotifyNotificationListener Listener = GotifyNotificationListener.New();

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
        await InitGotify();
        Listener.NotificationReceived += async (sender, e) =>
        {
            await HandleNotificationReceived(sender, e);
        };
    }

    private async Task HandleNotificationReceived(object sender, NotificationEventArgs e)
    {
        var notificationPage = await NavigationService.DisplayModal<NotificationPage>();
        await Task.Delay(1000);
        notificationPage.Display(e.Notification);
    }

    private async Task InitGotify()
	{
		var mobileSettings = await Database.GetMobileSettings();
        var opts = _serviceProvider.GetRequiredService<IOptions<MobileOptions>>();
        if (string.IsNullOrWhiteSpace(mobileSettings.GotifyPushToken))
        {
            using (var httpClient = _serviceProvider.GetRequiredService<Factories.IHttpClientFactory>().Build())
            {
                var msg = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{opts.Value.IdServerUrl}/gotifyconnections"),
                    Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post
                };
                var httpResult = await httpClient.SendAsync(msg);
                var content = await httpResult.Content.ReadAsStringAsync();
                var jObj = JsonObject.Parse(content);
                var pushToken = jObj["token"].ToString();
                mobileSettings.GotifyPushToken = pushToken;
                await Database.UpdateMobileSettings(mobileSettings);
            }
        }

        var instance = GotifyNotificationListener.New();
        await instance.Start(
            opts.Value.WsServer,
            mobileSettings.GotifyPushToken, 
            CancellationToken.None);
	}
}