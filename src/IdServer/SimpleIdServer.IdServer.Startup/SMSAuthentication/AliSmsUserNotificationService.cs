using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Sms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.SMSAuthentication;

public class AliSmsUserNotificationService : ISmsUserNotificationService
{
    private readonly IConfiguration _configuration;

    public AliSmsUserNotificationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Name => IdServer.Sms.Constants.AMR;

    public Task Send(string title, string body, Dictionary<string, string> data, User user)
    {
        var phoneNumber = user.OAuthUserClaims.First(c => c.Name == JwtRegisteredClaimNames.PhoneNumber).Value;
        return Send(title, body, data, phoneNumber);
    }

    public Task Send(string title, string body, Dictionary<string, string> data, string destination)
    {
        var smsHostOptions = GetOptions();
        // Add logic to send the SMS.
        return Task.CompletedTask;
    }

    private AliSmsOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(AliSmsOptions).Name);
        return section.Get<AliSmsOptions>();
    }
}
