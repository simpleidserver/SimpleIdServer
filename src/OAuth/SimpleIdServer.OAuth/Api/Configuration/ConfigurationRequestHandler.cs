// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    public interface IConfigurationRequestHandler
    {
        Task Enrich(JObject jObj);
    }

    public class ConfigurationRequestHandler : IConfigurationRequestHandler
    {
        private readonly IOAuthScopeQueryRepository _oauthScopeRepository;
        private readonly IEnumerable<IResponseTypeHandler> _authorizationGrantTypeHandlers;
        private readonly IEnumerable<IOAuthResponseMode> _oauthResponseModes;
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _oauthClientAuthenticationHandlers;
        private readonly IEnumerable<ISignHandler> _signHandlers;

        public ConfigurationRequestHandler(IOAuthScopeQueryRepository oauthScopeRepository, IEnumerable<IResponseTypeHandler> authorizationGrantTypeHandlers, IEnumerable<IOAuthResponseMode> oauthResponseModes,
            IEnumerable<IGrantTypeHandler> grantTypeHandlers, IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers,
            IEnumerable<ISignHandler> signHandlers)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _authorizationGrantTypeHandlers = authorizationGrantTypeHandlers;
            _oauthResponseModes = oauthResponseModes;
            _grantTypeHandlers = grantTypeHandlers;
            _oauthClientAuthenticationHandlers = oauthClientAuthenticationHandlers;
            _signHandlers = signHandlers;
        }

        public async Task Enrich(JObject jObj)
        {
            jObj.Add(OAuthConfigurationNames.ScopesSupported, JArray.FromObject((await _oauthScopeRepository.GetAllOAuthScopesExposed()).Select(s => s.Name).ToList()));
            jObj.Add(OAuthConfigurationNames.ResponseTypesSupported, JArray.FromObject(_authorizationGrantTypeHandlers.Select(s => s.ResponseType).Distinct()));
            jObj.Add(OAuthConfigurationNames.ResponseModesSupported, JArray.FromObject(_oauthResponseModes.Select(s => s.ResponseMode)));
            jObj.Add(OAuthConfigurationNames.GrantTypesSupported, JArray.FromObject(_grantTypeHandlers.Select(r => r.GrantType)));
            jObj.Add(OAuthConfigurationNames.TokenEndpointAuthMethodsSupported, JArray.FromObject(_oauthClientAuthenticationHandlers.Select(r => r.AuthMethod)));
            jObj.Add(OAuthConfigurationNames.TokenEndpointAuthSigningAlgValuesSupported, JArray.FromObject(_signHandlers.Select(s => s.AlgName)));
        }
    }
}