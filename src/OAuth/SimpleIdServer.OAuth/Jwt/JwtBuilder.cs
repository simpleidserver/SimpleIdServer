// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Jwt
{
    public interface IJwtBuilder
    {
        Task<string> BuildAccessToken(Client client, SecurityTokenDescriptor securityTokenDescriptor, CancellationToken cancellationToken);
        Task<string> BuildClientToken(Client client, SecurityTokenDescriptor securityTokenDescriptor, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken);
        string Sign(SecurityTokenDescriptor securityTokenDescriptor, string jwsAlg);
        string Encrypt(string jws, string jweAlg, string jweEnc);
        string Encrypt(string jws, EncryptingCredentials encryptionKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password);
    }

    public class JwtBuilder : IJwtBuilder
    {
        private readonly IKeyStore _keyStore;
        private readonly IClientHelper _clientHelper;
        private readonly OAuthHostOptions _options;

        public JwtBuilder(IKeyStore keyStore, IClientHelper clientHelper, IOptions<OAuthHostOptions> options)
        {
            _keyStore = keyStore;
            _clientHelper = clientHelper;
            _options = options.Value;
        }

        public Task<string> BuildAccessToken(Client client, SecurityTokenDescriptor securityTokenDescriptor, CancellationToken cancellationToken) => BuildClientToken(client, securityTokenDescriptor, client.TokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg, client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, cancellationToken);

        public async Task<string> BuildClientToken(Client client, SecurityTokenDescriptor securityTokenDescriptor, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken)
        {
            var jws = Sign(securityTokenDescriptor, sigAlg);
            if (string.IsNullOrWhiteSpace(encAlg)) return jws;
            var jsonWebKeys = await _clientHelper.ResolveJsonWebKeys(client, cancellationToken);
            var jsonWebKey = jsonWebKeys.FirstOrDefault(j => j.Use == JsonWebKeyUseNames.Enc && j.Alg == encAlg);
            if (jsonWebKey == null) return jws;
            var credentials = new EncryptingCredentials(jsonWebKey, encAlg, enc);
            return Encrypt(jws, credentials);
        }

        public string Sign(SecurityTokenDescriptor securityTokenDescriptor, string jwsAlg)
        {
            var signingKeys = _keyStore.GetAllSigningKeys();
            var signingKey = signingKeys.First(s => s.Algorithm == jwsAlg);
            var handler = new JsonWebTokenHandler();
            securityTokenDescriptor.SigningCredentials = signingKey;
            return handler.CreateToken(securityTokenDescriptor);
        }

        public string Encrypt(string jws, string jweAlg, string jweEnc)
        {
            var encryptionKeys = _keyStore.GetAllEncryptingKeys();
            var encryptionKey = encryptionKeys.First(e => e.Enc == jweEnc && e.Alg == jweAlg);
            return Encrypt(jws, encryptionKey);
        }

        public string Encrypt(string jws, EncryptingCredentials encryptionKey)
        {
            var handler = new JsonWebTokenHandler();
            return handler.EncryptToken(jws, encryptionKey);
        }

        public string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey)
        {
            var credentials = new EncryptingCredentials(jsonWebKey, jsonWebKey.Alg, jweEnc);
            return Encrypt(jws, credentials);
        }

        public string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password)
        {
            var credentials = new EncryptingCredentials(jsonWebKey, jsonWebKey.Alg, jweEnc);
            var handler = new JsonWebTokenHandler();
            return handler.EncryptToken(jws, credentials);
        }
    }
}
