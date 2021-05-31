// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Api.Register;
using SimpleIdServer.OpenID.Options;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Api.Register
{
    public class AddOpenBankingApiClientHandler : AddOpenIdClientHandler
    {
        public AddOpenBankingApiClientHandler(
            IOAuthClientRepository oauthClientRepository,
            IJwtParser jwtParser, 
            IHttpClientFactory httpClientFactory, 
            IOAuthClientValidator oauthClientValidator, 
            IOptions<OAuthHostOptions> oauthHostOptions, 
            IOptions<OpenIDHostOptions> openidHostOptions) : base(oauthClientRepository, jwtParser, httpClientFactory, oauthClientValidator, oauthHostOptions, openidHostOptions)
        {
        }

        protected override void SetDefaultScopes(BaseClient oauthClient)
        {
            if (!oauthClient.AllowedScopes.Any())
            {
                oauthClient.AllowedScopes = OauthHostOptions.DefaultScopes.Select(_ => new OAuthScope
                {
                    Name = _
                }).ToList();
            }
        }
    }
}
