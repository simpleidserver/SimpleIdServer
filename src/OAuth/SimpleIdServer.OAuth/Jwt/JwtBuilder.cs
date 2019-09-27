// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Jwt
{
    public interface IJwtBuilder
    {
        Task<string> BuildAccessToken(OAuthClient client, JwsPayload jwsPayload);
        Task<string> BuildClientToken(OAuthClient client, JwsPayload jwsPayload, string sigAlg, string encAlg, string enc);
        Task<string> Sign(JwsPayload jwsPayload, string jwsAlg);
        string Sign(JwsPayload jwsPayload, JsonWebKey jsonWebKey);
        Task<string> Encrypt(string jws, string jweAlg, string jweEnc);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password);
    }

    public class JwtBuilder : IJwtBuilder
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonWebKeyQueryRepository _jsonWebKeyRepository;
        private readonly IJwsGenerator _jwsGenerator;
        private readonly IJweGenerator _jweGenerator;

        public JwtBuilder(IHttpClientFactory httpClientFactory, IJsonWebKeyQueryRepository jsonWebKeyRepository, IJwsGenerator jwsGenerator, IJweGenerator jweGenerator)
        {
            _httpClientFactory = httpClientFactory;
            _jsonWebKeyRepository = jsonWebKeyRepository;
            _jwsGenerator = jwsGenerator;
            _jweGenerator = jweGenerator;
        }

        public Task<string> BuildAccessToken(OAuthClient client, JwsPayload jwsPayload)
        {
            return BuildClientToken(client, jwsPayload, client.TokenSignedResponseAlg, client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc);
        }

        public async Task<string> BuildClientToken(OAuthClient client, JwsPayload jwsPayload, string sigAlg, string encAlg, string enc)
        {
            var jwt = await Sign(jwsPayload, sigAlg);
            if (string.IsNullOrWhiteSpace(encAlg))
            {
                return jwt;
            }

            var jsonWebKeys = await client.ResolveJsonWebKeys(_httpClientFactory);
            var jsonWebKey = jsonWebKeys.FirstOrDefault(j => j.Use == Usages.ENC && j.Alg == encAlg);
            if (jsonWebKey == null)
            {
                return jwt;
            }

            return _jweGenerator.Build(jwt, encAlg, enc, jsonWebKey);
        }

        public async Task<string> Sign(JwsPayload jwsPayload, string jwsAlg)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.FindJsonWebKeys(Usages.SIG, jwsAlg, new[]
            {
                KeyOperations.Sign
            });
            return Sign(jwsPayload, jsonWebKeys.First());
        }

        public string Sign(JwsPayload jwsPayload, JsonWebKey jsonWebKey)
        {
            var serializedPayload = JsonConvert.SerializeObject(jwsPayload);
            return _jwsGenerator.Build(serializedPayload, jsonWebKey.Alg, jsonWebKey);
        }

        public async Task<string> Encrypt(string jws, string jweAlg, string jweEnc)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.FindJsonWebKeys(Usages.ENC, jweAlg, new[]
            {
                KeyOperations.Encrypt
            });
            if (!jsonWebKeys.Any())
            {
                return jws;
            }

            return Encrypt(jws, jweEnc, jsonWebKeys.First());
        }

        public string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey)
        {
            return _jweGenerator.Build(
                jws,
                jsonWebKey.Alg,
                jweEnc,
                jsonWebKey);
        }

        public string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password)
        {
            return _jweGenerator.Build(
                jws,
                jsonWebKey.Alg,
                jweEnc,
                jsonWebKey,
                password);
        }
    }
}
