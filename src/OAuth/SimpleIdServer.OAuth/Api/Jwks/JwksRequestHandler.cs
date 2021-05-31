// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Jwks
{
    public interface IJwksRequestHandler
    {
        Task<JObject> Get(CancellationToken cancellationToken);
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

        public async Task<JObject> Get(CancellationToken cancellationToken)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.GetActiveJsonWebKeys(cancellationToken);
            var keys = new JArray();
            foreach(var jsonWebKey in jsonWebKeys)
            {
                keys.Add(jsonWebKey.GetPublicJwt());
            }

            var result = new JObject
            {
                { "keys", keys }
            };
            return result;
        }

        public async Task<bool> Rotate(CancellationToken cancellationToken)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.GetNotRotatedJsonWebKeys(cancellationToken);
            foreach (var jsonWebKey in jsonWebKeys)
            {
                var newJsonWebKey = jsonWebKey.Rotate(_options.JWKExpirationTimeInSeconds);
                await _jsonWebKeyRepository.Update(jsonWebKey, cancellationToken);
                await _jsonWebKeyRepository.Add(newJsonWebKey, cancellationToken);
            }

            await _jsonWebKeyRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
