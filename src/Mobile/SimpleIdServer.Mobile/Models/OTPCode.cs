using SQLite;

namespace SimpleIdServer.Mobile.Models;


public record OTPCode
{
    [PrimaryKey]
    public string Id { get; set; }
    public OTPCodeTypes Type { get; set; }
    public string Issuer { get; set; }
    public string Name { get; set; }
    public string Secret { get; set; }
    public string Algorithm { get; set; }
    public int Counter { get; set; }
    public int Period { get; set; }
}

public enum OTPCodeTypes
{
    TOTP = 0,
    HOTP = 1
}