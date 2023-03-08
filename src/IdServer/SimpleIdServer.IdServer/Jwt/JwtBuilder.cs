// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jwt
{
    public interface IJwtBuilder
    {
        Task<string> BuildAccessToken(string realm, Client client, SecurityTokenDescriptor securityTokenDescriptor, CancellationToken cancellationToken);
        Task<string> BuildClientToken(string realm, Client client, SecurityTokenDescriptor securityTokenDescriptor, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken);
        string Sign(string realm, SecurityTokenDescriptor securityTokenDescriptor, string jwsAlg);
        string Encrypt(string realm, string jws, string jweAlg, string jweEnc);
        string Encrypt(string jws, EncryptingCredentials encryptionKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey);
        string Encrypt(string jws, string jweEnc, JsonWebKey jsonWebKey, string password);
        ReadJsonWebTokenResult ReadSelfIssuedJsonWebToken(string realm, string jwt);
        Task<ReadJsonWebTokenResult> ReadJsonWebToken(string realm, string jwt, Client client, string sigAlg, string encAlg, CancellationToken cancellationToken);
    }

    public class ReadJsonWebTokenResult
    {
        private ReadJsonWebTokenResult() { }

        public string? Error { get; private set; } = null;
        public JsonWebToken Jwt { get; private set; }
        public JsonWebToken EncryptedJwt { get; private set; }

        public static ReadJsonWebTokenResult BuildError(string error) => new ReadJsonWebTokenResult { Error = error };

        public static ReadJsonWebTokenResult Ok(JsonWebToken jwt, JsonWebToken encryptedJwt) => new ReadJsonWebTokenResult { Error = null, Jwt = jwt, EncryptedJwt = encryptedJwt };
    }

    public class JwtBuilder : IJwtBuilder
    {
        private readonly IKeyStore _keyStore;
        private readonly IClientHelper _clientHelper;
        private readonly IdServerHostOptions _options;

        public JwtBuilder(IKeyStore keyStore, IClientHelper clientHelper, IOptions<IdServerHostOptions> options)
        {
            _keyStore = keyStore;
            _clientHelper = clientHelper;
            _options = options.Value;
        }

        public Task<string> BuildAccessToken(string realm, Client client, SecurityTokenDescriptor securityTokenDescriptor, CancellationToken cancellationToken) => BuildClientToken(realm, client, securityTokenDescriptor, client.TokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg, client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, cancellationToken);

        public async Task<string> BuildClientToken(string realm, Client client, SecurityTokenDescriptor securityTokenDescriptor, string sigAlg, string encAlg, string enc, CancellationToken cancellationToken)
        {
            var jws = Sign(realm, securityTokenDescriptor, sigAlg);
            if (string.IsNullOrWhiteSpace(encAlg)) return jws;
            var jsonWebKeys = await _clientHelper.ResolveJsonWebKeys(client, cancellationToken);
            var jsonWebKey = jsonWebKeys.FirstOrDefault(j => j.Use == JsonWebKeyUseNames.Enc && j.Alg == encAlg);
            if (jsonWebKey == null) return jws;
            var credentials = new EncryptingCredentials(jsonWebKey, encAlg, enc);
            return Encrypt(jws, credentials);
        }

        public string Sign(string realm, SecurityTokenDescriptor securityTokenDescriptor, string jwsAlg)
        {
            var handler = new JsonWebTokenHandler();
            if (jwsAlg == SecurityAlgorithms.None) return handler.CreateToken(securityTokenDescriptor);
            var signingKeys = _keyStore.GetAllSigningKeys(realm);
            var signingKey = signingKeys.FirstOrDefault(s => s.Algorithm == jwsAlg);
            if(signingKey == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.NO_JWK_WITH_ALG_SIG, jwsAlg));
            securityTokenDescriptor.SigningCredentials = signingKey;
            return handler.CreateToken(securityTokenDescriptor);
        }

        public string Encrypt(string realm, string jws, string jweAlg, string jweEnc)
        {
            var encryptionKeys = _keyStore.GetAllEncryptingKeys(realm);
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

        public ReadJsonWebTokenResult ReadSelfIssuedJsonWebToken(string realm, string jwt)
        {
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(jwt)) return ReadJsonWebTokenResult.BuildError(ErrorMessages.INVALID_JWT);
            var jsonWebToken = handler.ReadJsonWebToken(jwt);
            JsonWebToken encJwt = null;
            if(jsonWebToken.IsEncrypted)
            {
                var encryptionKeys = _keyStore.GetAllEncryptingKeys(realm);
                var encryptionKey = encryptionKeys.FirstOrDefault(e => jsonWebToken.Kid == e.Key.KeyId);
                if (encryptionKey == null) return ReadJsonWebTokenResult.BuildError(string.Format(ErrorMessages.NO_JWK_FOUND_TO_DECRYPT, jsonWebToken.Kid));
                jwt = handler.DecryptToken(jsonWebToken, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    TokenDecryptionKey = encryptionKey.Key
                });
                encJwt = handler.ReadJsonWebToken(jwt);
                jsonWebToken = encJwt;
            }

            var signKeys = _keyStore.GetAllSigningKeys(realm);
            var sigKey = signKeys.FirstOrDefault(k => k.Kid == jsonWebToken.Kid);
            if (sigKey == null) return ReadJsonWebTokenResult.BuildError(string.Format(ErrorMessages.NO_JWK_FOUND_TO_CHECK_SIG, jsonWebToken.Kid));
            var validationResult = handler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = sigKey.Key
            });
            if (!validationResult.IsValid) return ReadJsonWebTokenResult.BuildError(ErrorMessages.JWT_BAD_SIGNATURE);
            return ReadJsonWebTokenResult.Ok(jsonWebToken, encJwt);
        }

        public async Task<ReadJsonWebTokenResult> ReadJsonWebToken(string realm, string jwt, Client client, string sigAlg, string encAlg, CancellationToken cancellationToken)
        {
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(jwt)) return ReadJsonWebTokenResult.BuildError(ErrorMessages.INVALID_JWT);
            var jsonWebToken = handler.ReadJsonWebToken(jwt);
            JsonWebToken encJwt = null;
            if(
                (!string.IsNullOrWhiteSpace(encAlg) && !jsonWebToken.IsEncrypted) ||
                (!string.IsNullOrWhiteSpace(encAlg) && jsonWebToken.IsEncrypted && jsonWebToken.Alg != encAlg)
            ) return ReadJsonWebTokenResult.BuildError(string.Format(ErrorMessages.JWT_MUST_BE_ENCRYPTED, encAlg));

            if(string.IsNullOrWhiteSpace(encAlg) && jsonWebToken.IsEncrypted) return ReadJsonWebTokenResult.BuildError(ErrorMessages.JWT_CANNOT_BE_ENCRYPTED);

            if (jsonWebToken.IsEncrypted)
            {
                // symmetric key.
                if(jsonWebToken.Alg == Constants.AlgDir)
                {
                    try
                    {
                        var encryptionSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(client.ClientSecret));
                        jwt = handler.DecryptToken(jsonWebToken, new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = false,
                            TokenDecryptionKey = encryptionSecurityKey
                        });
                        encJwt = handler.ReadJsonWebToken(jwt);
                    }
                    catch
                    {
                        return ReadJsonWebTokenResult.BuildError(ErrorMessages.JWT_CANNOT_BE_DECRYPTED);
                    }
                }
                // asymmetric key.
                else
                {
                    var encryptedKeys = _keyStore.GetAllEncryptingKeys(realm);
                    var encryptedKey = encryptedKeys.FirstOrDefault(k => k.Key.KeyId == jsonWebToken.Kid);
                    if (encryptedKey == null) return ReadJsonWebTokenResult.BuildError(string.Format(ErrorMessages.NO_JWK_FOUND_TO_DECRYPT, jsonWebToken.Kid));
                    try
                    {
                        jwt = handler.DecryptToken(jsonWebToken, new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = false,
                            TokenDecryptionKey = encryptedKey.Key
                        });
                        encJwt = handler.ReadJsonWebToken(jwt);
                    }
                    catch
                    {
                        return ReadJsonWebTokenResult.BuildError(ErrorMessages.JWT_CANNOT_BE_DECRYPTED);
                    }
                }

                jsonWebToken = encJwt;
            }

            if(sigAlg != jsonWebToken.Alg) return ReadJsonWebTokenResult.BuildError(string.Format(ErrorMessages.JWT_MUST_BE_SIGNED, sigAlg));

            var jsonWebKey = await _clientHelper.ResolveJsonWebKey(client, jsonWebToken.Kid, cancellationToken);
            if (jsonWebToken == null) return ReadJsonWebTokenResult.BuildError(string.Format(ErrorMessages.NO_JWK_FOUND_TO_CHECK_SIG, jsonWebToken.Kid));
            var validationResult = handler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = jsonWebKey
            });
            if (!validationResult.IsValid) return ReadJsonWebTokenResult.BuildError(ErrorMessages.JWT_BAD_SIGNATURE);
            return ReadJsonWebTokenResult.Ok(jsonWebToken, encJwt);
        }
    }
}
