// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Helpers
{
    public interface ICredIssuerTokenHelper
    {
        Task AddCredentialNonce(string credentialNonce, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task<double?> GetCredentialNonce(string credentialNonce, CancellationToken cancellationToken);

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
            await _distributedCache.SetAsync(credentialNonce, Encoding.UTF8.GetBytes(validityPeriodsInSeconds.ToString()), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(validityPeriodsInSeconds)
            }, cancellationToken);
        }

        public async Task<double?> GetCredentialNonce(string credentialNonce, CancellationToken cancellationToken)
        {
            var result = await _distributedCache.GetAsync(credentialNonce, cancellationToken);
            if (result == null) return null;
            return double.Parse(Encoding.UTF8.GetString(result));
        }
    }
}
