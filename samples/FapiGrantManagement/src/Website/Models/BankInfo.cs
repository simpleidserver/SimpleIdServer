namespace Website.Models;

public class BankInfo
{
    public string Name { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string AuthorizationUrl { get; set; } = null!;
    public string TokenUrl { get; set; } = null!;
    public string GrantId { get; set; } = null!;
}