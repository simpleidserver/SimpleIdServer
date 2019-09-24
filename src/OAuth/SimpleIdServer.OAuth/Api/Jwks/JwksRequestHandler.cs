// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains.Jwks;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Jwks
{
    public interface IJwksRequestHandler
    {
        Task<JObject> Get();
        Task<bool> Rotate();
    }

    public class JwksRequestHandler : IJwksRequestHandler
    {
        private readonly IJsonWebKeyQueryRepository _jsonWebKeyQueryRepository;
        private readonly IJsonWebKeyCommandRepository _jsonWebKeyCommandRepository;

        public JwksRequestHandler(IJsonWebKeyQueryRepository jsonWebKeyQueryRepository, IJsonWebKeyCommandRepository jsonWebKeyCommandRepository)
        {
            _jsonWebKeyQueryRepository = jsonWebKeyQueryRepository;
            _jsonWebKeyCommandRepository = jsonWebKeyCommandRepository;
        }

        public async Task<JObject> Get()
        {
            var jsonWebKeys = await _jsonWebKeyQueryRepository.GetAllJsonWebKeys();
            var keys = new JArray();
            foreach(var jsonWebKey in jsonWebKeys)
            {
                var key = new JObject();
                keys.Add(jsonWebKey.GetPublicJwt());
            }

            var result = new JObject
            {
                { "keys", keys }
            };
            return result;
        }

        public async Task<bool> Rotate()
        {
            var jsonWebKeys = await _jsonWebKeyQueryRepository.GetAllJsonWebKeys();
            foreach (var jsonWebKey in jsonWebKeys)
            {
                jsonWebKey.Renew();
                _jsonWebKeyCommandRepository.Update(jsonWebKey);
            }

            await _jsonWebKeyCommandRepository.SaveChanges();
            return true;
        }
    }
}
