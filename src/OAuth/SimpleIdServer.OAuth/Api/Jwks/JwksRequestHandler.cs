// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.Store;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Jwks
{
    public interface IJwksRequestHandler
    {
        Task<JsonObject> Get(CancellationToken cancellationToken);
        Task<bool> Rotate(CancellationToken token);
    }

    public class JwksRequestHandler : IJwksRequestHandler
    {
        private readonly IJsonWebKeyRepository _jsonWebKeyRepository;
        private readonly OAuthHostOptions _options;

        public JwksRequestHandler(
            IJsonWebKeyRepository jsonWebKeyRepository, 
            IOptions<OAuthHostOptions> options)
        {
            _jsonWebKeyRepository = jsonWebKeyRepository;
            _options = options.Value;
        }

        public async Task<JsonObject> Get(CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            var jsonWebKeys = await _jsonWebKeyRepository.Query()
                .Include(j => j.KeyOperationLst)
                .AsNoTracking()
                .Where(j => j.ExpirationDateTime == null || currentDateTime < j.ExpirationDateTime)
                .ToListAsync(cancellationToken);
            var keys = new JsonArray();
            foreach(var jsonWebKey in jsonWebKeys)
                keys.Add(jsonWebKey.GetPublicJwt());
            var result = new JsonObject
            {
                ["keys"] = keys
            };
            return result;
        }

        public async Task<bool> Rotate(CancellationToken cancellationToken)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.Query()
                .Where(j => string.IsNullOrWhiteSpace(j.RotationJWKId))
                .ToListAsync(cancellationToken);
            foreach (var jsonWebKey in jsonWebKeys)
            {
                var newJsonWebKey = jsonWebKey.Rotate(_options.JWKExpirationTimeInSeconds);
                _jsonWebKeyRepository.Add(newJsonWebKey);
            }

            await _jsonWebKeyRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
