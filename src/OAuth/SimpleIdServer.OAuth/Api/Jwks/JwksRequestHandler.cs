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
        Task<JObject> Get();
        Task<bool> Rotate(CancellationToken token);
    }

    public class JwksRequestHandler : IJwksRequestHandler
    {
        private readonly IJsonWebKeyQueryRepository _jsonWebKeyQueryRepository;
        private readonly IJsonWebKeyCommandRepository _jsonWebKeyCommandRepository;
        private readonly OAuthHostOptions _options;

        public JwksRequestHandler(
            IJsonWebKeyQueryRepository jsonWebKeyQueryRepository, 
            IJsonWebKeyCommandRepository jsonWebKeyCommandRepository,
            IOptions<OAuthHostOptions> options)
        {
            _jsonWebKeyQueryRepository = jsonWebKeyQueryRepository;
            _jsonWebKeyCommandRepository = jsonWebKeyCommandRepository;
            _options = options.Value;
        }

        public async Task<JObject> Get()
        {
            var jsonWebKeys = await _jsonWebKeyQueryRepository.GetActiveJsonWebKeys();
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
            var jsonWebKeys = await _jsonWebKeyQueryRepository.GetNotRotatedJsonWebKeys(cancellationToken);
            foreach (var jsonWebKey in jsonWebKeys)
            {
                var newJsonWebKey = jsonWebKey.Rotate(_options.AuthorizationCodeExpirationInSeconds);
                await _jsonWebKeyCommandRepository.Update(jsonWebKey, cancellationToken);
                _jsonWebKeyCommandRepository.Add(newJsonWebKey);
            }

            await _jsonWebKeyCommandRepository.SaveChanges(cancellationToken);
            return true;
        }
    }
}
