// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Management.Handlers;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Management
{
    public class AddOpenIdClientHandler : IAddOAuthClientHandler
    {
        private readonly IOAuthClientRepository _oauthClientRepository;
        private readonly OAuthHostOptions _options;

        public AddOpenIdClientHandler(IOAuthClientRepository oauthClientRepository, IOptions<OAuthHostOptions> options)
        {
            _oauthClientRepository = oauthClientRepository;
            _options = options.Value;
        }

        public async Task<string> Handle(string language, JObject jObj, CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            DateTime? expirationDateTime = null;
            if (_options.ClientSecretExpirationInSeconds != null)
            {
                expirationDateTime = currentDateTime.AddSeconds(_options.ClientSecretExpirationInSeconds.Value);
            }

            var parameter = jObj.ToCreateOpenIdClientParameter();
            var openidClient = OpenIdClient.Create(parameter.ApplicationKind, 
                parameter.ClientName, 
                language, 
                expirationDateTime, 
                _options.DefaultTokenExpirationTimeInSeconds, 
                _options.DefaultRefreshTokenExpirationTimeInSeconds);
            await _oauthClientRepository.Add(openidClient, cancellationToken);
            await _oauthClientRepository.SaveChanges(cancellationToken);
            return openidClient.ClientId;
        }
    }
}
