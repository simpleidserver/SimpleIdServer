// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    public interface IConfigurationRequestHandler
    {
        Task Enrich(JObject jObj, string issuer, CancellationToken cancellationToken);
    }

    public class ConfigurationRequestHandler : IConfigurationRequestHandler
    {
        private readonly IOAuthScopeRepository _oauthScopeRepository;
        private readonly IEnumerable<IResponseTypeHandler> _authorizationGrantTypeHandlers;
        private readonly IEnumerable<IOAuthResponseMode> _oauthResponseModes;
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _oauthClientAuthenticationHandlers;
        private readonly IEnumerable<ISignHandler> _signHandlers;
        private readonly IOAuthWorkflowConverter _oauthWorkflowConverter;
        private readonly OAuthHostOptions _options;

        public ConfigurationRequestHandler(
            IOAuthScopeRepository oauthScopeRepository, 
            IEnumerable<IResponseTypeHandler> authorizationGrantTypeHandlers, 
            IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IEnumerable<IGrantTypeHandler> grantTypeHandlers, 
            IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers,
            IEnumerable<ISignHandler> signHandlers,
            IOAuthWorkflowConverter oauthWorkflowConverter,
            IOptions<OAuthHostOptions> options)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _authorizationGrantTypeHandlers = authorizationGrantTypeHandlers;
            _oauthResponseModes = oauthResponseModes;
            _grantTypeHandlers = grantTypeHandlers;
            _oauthClientAuthenticationHandlers = oauthClientAuthenticationHandlers;
            _signHandlers = signHandlers;
            _oauthWorkflowConverter = oauthWorkflowConverter;
            _options = options.Value;
        }

        protected IOAuthWorkflowConverter WorkflowConverter => _oauthWorkflowConverter;

        public virtual async Task Enrich(JObject jObj, string issuer, CancellationToken cancellationToken)
        {
            jObj.Add(OAuthConfigurationNames.TlsClientCertificateBoundAccessTokens, true);
            jObj.Add(OAuthConfigurationNames.ScopesSupported, JArray.FromObject((await _oauthScopeRepository.GetAllOAuthScopesExposed(cancellationToken)).Select(s => s.Name).ToList()));
            jObj.Add(OAuthConfigurationNames.ResponseTypesSupported, JArray.FromObject(GetResponseTypes()));
            jObj.Add(OAuthConfigurationNames.ResponseModesSupported, JArray.FromObject(_oauthResponseModes.Select(s => s.ResponseMode)));
            jObj.Add(OAuthConfigurationNames.GrantTypesSupported, JArray.FromObject(GetGrantTypes()));
            jObj.Add(OAuthConfigurationNames.TokenEndpointAuthMethodsSupported, JArray.FromObject(_oauthClientAuthenticationHandlers.Select(r => r.AuthMethod)));
            jObj.Add(OAuthConfigurationNames.TokenEndpointAuthSigningAlgValuesSupported, JArray.FromObject(_signHandlers.Select(s => s.AlgName)));
            if (_options.MtlsEnabled)
            {
                jObj.Add(OAuthConfigurationNames.MtlsEndpointAliases, new JObject
                {
                    { OAuthConfigurationNames.TokenEndpoint, $"{issuer}/{Constants.EndPoints.MtlsToken}" }
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