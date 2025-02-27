namespace FormBuilder.Startup.Config;

public class StorageConfiguration
{
    public StorageTypes Type { get; set; }
    public string ConnectionString { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
