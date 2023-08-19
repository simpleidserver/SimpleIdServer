using SQLite;

namespace SimpleIdServer.Mobile.Models;

public class MobileSettings
{
    [PrimaryKey]
    public string Id { get; set; }
    public bool IsDeveloperModeEnabled { get; set; }
}
