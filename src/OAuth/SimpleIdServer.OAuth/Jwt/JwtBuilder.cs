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
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Jwt
{
    public interface IJwtBuilder
    {
        Task<string> BuildAccessToken(BaseClient client, JwsPayload jwsPayload, CancellationToken cancellationToken);
        Task<string> BuildClientToken(BaseClient client, JwsPayload jwsPayload, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken);
        Task<string> Sign(JwsPayload jwsPayload, string jwsAlg, CancellationToken cancellationToken);
        string Sign(JwsPayload jwsPayload, JsonWebKey jsonWebKey, string jwsAlg);
        Task<string> Encrypt(string jws, string jweAlg, string jweEnc, CancellationToken cancellationToken);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password);
    }

    public class JwtBuilder : IJwtBuilder
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonWebKeyRepository _jsonWebKeyRepository;
        private readonly IJwsGenerator _jwsGenerator;
        private readonly IJweGenerator _jweGenerator;

        public JwtBuilder(IHttpClientFactory httpClientFactory, IJsonWebKeyRepository jsonWebKeyRepository, IJwsGenerator jwsGenerator, IJweGenerator jweGenerator)
        {
            _httpClientFactory = httpClientFactory;
            _jsonWebKeyRepository = jsonWebKeyRepository;
            _jwsGenerator = jwsGenerator;
            _jweGenerator = jweGenerator;
        }

        public Task<string> BuildAccessToken(BaseClient client, JwsPayload jwsPayload, CancellationToken cancellationToken)
        {
            return BuildClientToken(client, jwsPayload, client.TokenSignedResponseAlg, client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, cancellationToken);
        }

        public async Task<string> BuildClientToken(BaseClient client, JwsPayload jwsPayload, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken)
        {
            var jwt = await Sign(jwsPayload, sigAlg, cancellationToken);
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

        public async Task<string> Sign(JwsPayload jwsPayload, string jwsAlg, CancellationToken cancellationToken)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.FindJsonWebKeys(Usages.SIG, jwsAlg, new[]
            {
                KeyOperations.Sign
            }, cancellationToken);
            return Sign(jwsPayload, jsonWebKeys.FirstOrDefault(), jwsAlg);
        }

        public string Sign(JwsPayload jwsPayload, JsonWebKey jsonWebKey, string jwsAlg)
        {
            var serializedPayload = JsonConvert.SerializeObject(jwsPayload);
            return _jwsGenerator.Build(serializedPayload, jwsAlg, jsonWebKey);
        }

        public async Task<string> Encrypt(string jws, string jweAlg, string jweEnc, CancellationToken cancellationToken)
        {
            var jsonWebKeys = await _jsonWebKeyRepository.FindJsonWebKeys(Usages.ENC, jweAlg, new[]
            {
                KeyOperations.Encrypt
            }, cancellationToken);
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
