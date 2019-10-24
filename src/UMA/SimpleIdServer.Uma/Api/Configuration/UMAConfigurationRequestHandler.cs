using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.Uma.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Configuration
{
    public class UMAConfigurationRequestHandler : ConfigurationRequestHandler
    {
        public UMAConfigurationRequestHandler(IOAuthScopeQueryRepository oauthScopeRepository, IEnumerable<IResponseTypeHandler> authorizationGrantTypeHandlers, IEnumerable<IOAuthResponseMode> oauthResponseModes, IEnumerable<IGrantTypeHandler> grantTypeHandlers, IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers, IEnumerable<ISignHandler> signHandlers) : base(oauthScopeRepository, authorizationGrantTypeHandlers, oauthResponseModes, grantTypeHandlers, oauthClientAuthenticationHandlers, signHandlers)
        {
        }

        public override async Task Enrich(JObject jObj, string issuer)
        {
            await base.Enrich(jObj, issuer);
            jObj.Add(UMAConfigurationNames.PermissionEndpoint, $"{issuer}/{UMAConstants.EndPoints.PermissionsAPI}");
            jObj.Add(UMAConfigurationNames.ResourceRegistrationEndpoint, $"{issuer}/{UMAConstants.EndPoints.ResourcesAPI}");
        }
    }
}
