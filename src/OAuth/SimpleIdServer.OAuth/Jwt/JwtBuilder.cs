// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Domains;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.Store;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Jwt
{
    public interface IJwtBuilder
    {
        Task<string> BuildAccessToken(Client client, JwsPayload jwsPayload, CancellationToken cancellationToken);
        Task<string> BuildClientToken(Client client, JwsPayload jwsPayload, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken);
        Task<string> Sign(JwsPayload jwsPayload, string jwsAlg, CancellationToken cancellationToken);
        string Sign(JwsPayload jwsPayload, JsonWebKey jsonWebKey, string jwsAlg);
        Task<string> Encrypt(string jws, string jweAlg, string jweEnc, CancellationToken cancellationToken);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password);
    }

    public class JwtBuilder : IJwtBuilder
    {
        private readonly IJsonWebKeyRepository _jsonWebKeyRepository;
        private readonly IJwsGenerator _jwsGenerator;
        private readonly IJweGenerator _jweGenerator;
        private readonly IClientHelper _clientHelper;

        public JwtBuilder(IJsonWebKeyRepository jsonWebKeyRepository, IJwsGenerator jwsGenerator, IJweGenerator jweGenerator, IClientHelper clientHelper)
        {
            _jsonWebKeyRepository = jsonWebKeyRepository;
            _jwsGenerator = jwsGenerator;
            _jweGenerator = jweGenerator;
            _clientHelper = clientHelper;
        }

        public Task<string> BuildAccessToken(Client client, JwsPayload jwsPayload, CancellationToken cancellationToken)
        {
            return BuildClientToken(client, jwsPayload, client.TokenSignedResponseAlg, client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, cancellationToken);
        }

        public async Task<string> BuildClientToken(Client client, JwsPayload jwsPayload, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken)
        {
            var jwt = await Sign(jwsPayload, sigAlg, cancellationToken);
            if (string.IsNullOrWhiteSpace(encAlg))
            {
                return jwt;
            }

            var jsonWebKeys = await _clientHelper.ResolveJsonWebKeys(client);
            var jsonWebKey = jsonWebKeys.FirstOrDefault(j => j.Use == Usages.ENC && j.Alg == encAlg);
            if (jsonWebKey == null)
            {
                return jwt;
            }

            return _jweGenerator.Build(jwt, encAlg, enc, jsonWebKey);
        }

        public async Task<string> Sign(JwsPayload jwsPayload, string jwsAlg, CancellationToken cancellationToken)
        {
            var operations = new[]
            {
                KeyOperations.Sign
            };
            var currentDateTime = DateTime.UtcNow;
            int nbOperations = operations.Count();
            var jsonWebKeys = await _jsonWebKeyRepository.Query().Where(j =>
                (j.ExpirationDateTime == null || currentDateTime < j.ExpirationDateTime) &&
                (j.Use == Usages.SIG && j.Alg == jwsAlg && j.KeyOps.Where(k => operations.Contains(k)).Count() == nbOperations))
                .ToListAsync(cancellationToken);
            return Sign(jwsPayload, jsonWebKeys.FirstOrDefault(), jwsAlg);
        }

        public string Sign(JwsPayload jwsPayload, JsonWebKey jsonWebKey, string jwsAlg)
        {
            var serializedPayload = JsonSerializer.Serialize(jwsPayload);
            return _jwsGenerator.Build(serializedPayload, jwsAlg, jsonWebKey);
        }

        public async Task<string> Encrypt(string jws, string jweAlg, string jweEnc, CancellationToken cancellationToken)
        {
            var operations = new[]
            {
                KeyOperations.Encrypt
            };
            var currentDateTime = DateTime.UtcNow;
            int nbOperations = operations.Count();
            var jsonWebKeys = await _jsonWebKeyRepository.Query().Where(j =>
                (j.ExpirationDateTime == null || currentDateTime < j.ExpirationDateTime) &&
                (j.Use == Usages.ENC && j.Alg == jweAlg && j.KeyOps.Where(k => operations.Contains(k)).Count() == nbOperations))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            if (!jsonWebKeys.Any())
            {
                return jws;
            }

            return Encrypt(jws, jweEnc, jsonWebKeys.First());
        }

        public string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey) => _jweGenerator.Build(jws, jsonWebKey.Alg, jweEnc, jsonWebKey);

        public string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password) => _jweGenerator.Build(jws, jsonWebKey.Alg, jweEnc, jsonWebKey, password);
    }
}
