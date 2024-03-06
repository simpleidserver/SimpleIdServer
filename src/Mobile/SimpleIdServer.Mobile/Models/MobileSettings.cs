using SQLite;

namespace SimpleIdServer.Mobile.Models;

public class MobileSettings
{
    [PrimaryKey]
    public string Id { get; set; }
    public string NotificationMode { get; set; }
    public string GotifyPushToken { get; set; }
}
