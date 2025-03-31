using Microsoft.AspNetCore.Authentication.Negotiate;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace SimpleIdServer.IdServer.Light.Startup.Converters;

public class NegotiateOptionsLite : IDynamicAuthenticationOptions<NegotiateOptions>
{
    public NegotiateOptions Convert()
    {
        return new NegotiateOptions();
    }
}
