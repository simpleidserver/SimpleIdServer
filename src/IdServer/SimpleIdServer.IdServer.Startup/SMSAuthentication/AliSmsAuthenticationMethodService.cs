using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Startup.SMSAuthentication
{
    public class AliSmsAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => SimpleIdServer.IdServer.Sms.Constants.AMR;
        public string Name => "Sms";
        public Type? OptionsType => typeof(AliSmsOptions);
        public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION | AuthenticationMethodCapabilities.PUSHNOTIFICATION;
        public bool IsCredentialExists(User user) => user.OAuthUserClaims.Any(c => c.Type == JwtRegisteredClaimNames.PhoneNumber);
    }
}
