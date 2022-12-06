// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    public interface IConfigurationRequestHandler
    {
        Task Enrich(JsonObject jObj, string issuer, CancellationToken cancellationToken);
    }

    public class ConfigurationRequestHandler : IConfigurationRequestHandler
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IEnumerable<IResponseTypeHandler> _authorizationGrantTypeHandlers;
        private readonly IEnumerable<IOAuthResponseMode> _oauthResponseModes;
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _oauthClientAuthenticationHandlers;
        private readonly IEnumerable<ISignHandler> _signHandlers;
        private readonly IOAuthWorkflowConverter _oauthWorkflowConverter;
        private readonly OAuthHostOptions _options;

        public ConfigurationRequestHandler(
            IScopeRepository scopeRepository, 
            IEnumerable<IResponseTypeHandler> authorizationGrantTypeHandlers, 
            IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IEnumerable<IGrantTypeHandler> grantTypeHandlers, 
            IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers,
            IEnumerable<ISignHandler> signHandlers,
            IOAuthWorkflowConverter oauthWorkflowConverter,
            IOptions<OAuthHostOptions> options)
        {
            _scopeRepository = scopeRepository;
            _authorizationGrantTypeHandlers = authorizationGrantTypeHandlers;
            _oauthResponseModes = oauthResponseModes;
            _grantTypeHandlers = grantTypeHandlers;
            _oauthClientAuthenticationHandlers = oauthClientAuthenticationHandlers;
            _signHandlers = signHandlers;
            _oauthWorkflowConverter = oauthWorkflowConverter;
            _options = options.Value;
        }

        protected IOAuthWorkflowConverter WorkflowConverter => _oauthWorkflowConverter;

        public virtual async Task Enrich(JsonObject jObj, string issuer, CancellationToken cancellationToken)
        {
            var scopes = await _scopeRepository.Query()
                .Include(s => s.Claims)
                .AsNoTracking()
                .Where(s => s.IsExposedInConfigurationEdp)
                .Select(s => s.Name)
                .ToListAsync(cancellationToken);
            jObj.Add(OAuthConfigurationNames.TlsClientCertificateBoundAccessTokens, true);
            jObj.Add(OAuthConfigurationNames.ScopesSupported, JsonSerializer.SerializeToNode(scopes));
            jObj.Add(OAuthConfigurationNames.ResponseTypesSupported, JsonSerializer.SerializeToNode(GetResponseTypes()));
            jObj.Add(OAuthConfigurationNames.ResponseModesSupported, JsonSerializer.SerializeToNode(_oauthResponseModes.Select(s => s.ResponseMode)));
            jObj.Add(OAuthConfigurationNames.GrantTypesSupported, JsonSerializer.SerializeToNode(GetGrantTypes()));
            jObj.Add(OAuthConfigurationNames.TokenEndpointAuthMethodsSupported, JsonSerializer.SerializeToNode(_oauthClientAuthenticationHandlers.Select(r => r.AuthMethod)));
            jObj.Add(OAuthConfigurationNames.TokenEndpointAuthSigningAlgValuesSupported, JsonSerializer.SerializeToNode(_signHandlers.Select(s => s.AlgName)));
            if (_options.MtlsEnabled)
            {
                jObj.Add(OAuthConfigurationNames.MtlsEndpointAliases, new JsonObject
                {
                    [OAuthConfigurationNames.TokenEndpoint] = $"{issuer}/{Constants.EndPoints.MtlsToken}"
                });
            }
        }

        protected List<string> GetGrantTypes()
        {
            var result = new List<string>();
            result.AddRange(_grantTypeHandlers.Select(t => t.GrantType));
            result.AddRange(_authorizationGrantTypeHandlers.Select(t => t.GrantType));
            return result.Distinct().ToList();
        }

        protected List<string> GetResponseTypes()
        {
            var result = new List<string>();
            var responseTypes = _authorizationGrantTypeHandlers.Select(s => s.ResponseType).OrderBy(_ => _).Distinct();
            for (int i = 0; i < responseTypes.Count(); i++)
            {
                for (var y = 1; y <= responseTypes.Count() - i; y++)
                {
                    var responseTypeWorkflow = responseTypes.Skip(i).Take(y);
                    if (_oauthWorkflowConverter.TryGetWorkflow(responseTypeWorkflow, out string workflowName))
                    {
                        var record = string.Join(" ", responseTypeWorkflow.OrderBy(_ => _));
                        if (!result.Contains(record))
                        {
                            result.Add(record);
                        }
                    }
                }
            }

            return result;
        }
    }
}