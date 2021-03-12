// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Api.Configuration
{
    public class OpenBankingApiConfigurationRequestHandler : ConfigurationRequestHandler
    {
        public OpenBankingApiConfigurationRequestHandler(IOAuthScopeQueryRepository oauthScopeRepository, IEnumerable<IResponseTypeHandler> authorizationGrantTypeHandlers, IEnumerable<IOAuthResponseMode> oauthResponseModes, IEnumerable<IGrantTypeHandler> grantTypeHandlers, IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers, IEnumerable<ISignHandler> signHandlers, IOptions<OAuthHostOptions> options) : base(oauthScopeRepository, authorizationGrantTypeHandlers, oauthResponseModes, grantTypeHandlers, oauthClientAuthenticationHandlers, signHandlers, options)
        {
        }

        public override async Task Enrich(JObject jObj, string issuer)
        {
            await base.Enrich(jObj, issuer);
            jObj.Remove(OAuthConfigurationNames.ResponseTypesSupported);
            jObj.Add(OAuthConfigurationNames.ResponseTypesSupported, new JArray(SIDOpenIdConstants.HybridWorkflows.Select(_ => string.Join(" ", _))));
        }
    }
}
