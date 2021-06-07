// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IGetOAuthClientHandler
    {
        Task<JObject> Handle(string clientId, string issuer, CancellationToken cancellationToken);
    }

    public class GetOAuthClientHandler : IGetOAuthClientHandler
    {
        private readonly IOAuthClientRepository _oauthClientRepository;

        public GetOAuthClientHandler(IOAuthClientRepository oauthClientRepository)
        {
            _oauthClientRepository = oauthClientRepository;
        }

        public async Task<JObject> Handle(string clientId, string issuer, CancellationToken cancellationToken)
        {
            var oauthClient = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
            if (oauthClient == null)
            {
                return null;
            }

            return ToDto(oauthClient, issuer);
        }

        protected virtual JObject ToDto(BaseClient client, string issuer)
        {
            return (client as OAuthClient).ToDto(issuer);
        }
    }
}
