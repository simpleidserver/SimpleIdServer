namespace SimpleIdServer.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		RegisterRoutes();
	}

	private void RegisterRoutes()
    {
        Routing.RegisterRoute("settings/profile", typeof(ProfilePage));
        Routing.RegisterRoute("settings/dids", typeof(DidsPage));
		Routing.RegisterRoute("settings/info", typeof(InfoPage));
    }
}
