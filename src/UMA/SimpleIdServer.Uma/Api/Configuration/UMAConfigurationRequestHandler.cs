using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.Uma.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Configuration
{
    public class UMAConfigurationRequestHandler : ConfigurationRequestHandler
    {
        public UMAConfigurationRequestHandler(
            IOAuthScopeRepository oauthScopeRepository, 
            IEnumerable<IResponseTypeHandler> authorizationGrantTypeHandlers, 
            IEnumerable<IOAuthResponseMode> oauthResponseModes, 
            IEnumerable<IGrantTypeHandler> grantTypeHandlers, 
            IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers,
            IEnumerable<ISignHandler> signHandlers,
            IOAuthWorkflowConverter oauthWorkflowConverter,
            IOptions<OAuthHostOptions> options) 
            : base(oauthScopeRepository, authorizationGrantTypeHandlers, oauthResponseModes, grantTypeHandlers, oauthClientAuthenticationHandlers, signHandlers, oauthWorkflowConverter, options)
        {
        }

        public override async Task Enrich(JObject jObj, string issuer, CancellationToken cancellationToken)
        {
            await base.Enrich(jObj, issuer, cancellationToken);
            jObj.Add(UMAConfigurationNames.PermissionEndpoint, $"{issuer}/{UMAConstants.EndPoints.PermissionsAPI}");
            jObj.Add(UMAConfigurationNames.ResourceRegistrationEndpoint, $"{issuer}/{UMAConstants.EndPoints.ResourcesAPI}");
        }
    }
}
