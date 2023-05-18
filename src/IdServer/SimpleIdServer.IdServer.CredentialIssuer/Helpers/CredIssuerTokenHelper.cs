// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Helpers
{
    public interface ICredIssuerTokenHelper
    {
        Task AddCredentialNonce(string credentialNonce, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task AddAuthCode(string credIssuerState, string clientId, double validityPeriodsInSeconds, CancellationToken cancellationToken);
    }

    public class CredIssuerTokenHelper : ICredIssuerTokenHelper
    {
        private readonly IDistributedCache _distributedCache;

        public CredIssuerTokenHelper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task AddCredentialNonce(string credentialNonce, double validityPeriodsInSeconds, CancellationToken cancellationToken)
        {
            await _distributedCache.SetAsync(credentialNonce, Encoding.UTF8.GetBytes(string.Empty), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(validityPeriodsInSeconds)
            }, cancellationToken);
        }

        public async Task AddAuthCode(string credIssuerState, string clientId, double validityPeriodsInSeconds, CancellationToken cancellationToken)
        {
            var credOffer = new CredIssuerState { IssuerState = credIssuerState, ClientId = clientId };
            var serializedCredOffer = JsonSerializer.Serialize(credOffer);
            await _distributedCache.SetAsync(credIssuerState, Encoding.UTF8.GetBytes(serializedCredOffer), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(validityPeriodsInSeconds)
            }, cancellationToken);
        }
    }
}
