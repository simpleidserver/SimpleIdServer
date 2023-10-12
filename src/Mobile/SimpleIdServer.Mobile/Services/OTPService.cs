using SimpleIdServer.Mobile.Models;
using System.Text.RegularExpressions;
using System.Web;

namespace SimpleIdServer.Mobile.Services;

public interface IOTPService
{
    bool TryParse(string code, out OTPCode result);
}

public class OTPService : IOTPService
{
    public bool TryParse(string code, out OTPCode result)
    {
        result = null;
        var regex = new Regex(@"otpauth:\/\/(hotp|totp)\/(\w*):(\w*)((\?|&|\w)*=(\w)*)*");
        if (!regex.IsMatch(code)) return false;
        var url = new Uri(code);
        var query = url.Query;
        var nameValueCollection = HttpUtility.ParseQueryString(query);
        var secret = nameValueCollection.Get("secret")?.ToString();
        var issuer = nameValueCollection.Get("issuer")?.ToString();
        var counter = nameValueCollection.Get("counter")?.ToString();
        var period = nameValueCollection.Get("period")?.ToString();
        var splitted = url.AbsolutePath.Split(':');
        var algorithm = splitted[0].TrimStart('/');
        var name = splitted[1];
        result = new OTPCode
        {
            Type = code.StartsWith("otpauth://totp") ? OTPCodeTypes.TOTP : OTPCodeTypes.HOTP,
            Algorithm = algorithm,
            Issuer = issuer,
            Secret = secret,
            Name = name
        };
        if (!string.IsNullOrWhiteSpace(counter) && int.TryParse(counter, out int r)) result.Counter = r;
        if (!string.IsNullOrWhiteSpace(period) && int.TryParse(period, out int p)) result.Period = p;
        result.Id = $"{issuer}:{name}";
        return true;
    }
}
