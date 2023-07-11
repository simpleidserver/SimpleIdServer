// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.DPoP
{
    public class DPoPHandler
    {
        public bool CanReadToken(string dpop)
        {
            var handler = new JsonWebTokenHandler();
            return handler.CanReadToken(dpop); 
        }

        public DPoPValidationResult Validate(string dpop, IEnumerable<string> supportedAlgs, string httpMethod, string httpRequest, double validityTimeSeconds)
        {
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(dpop)) return DPoPValidationResult.Error("DPoP Proof must be a Json Web Token");
            var jwt = handler.ReadJsonWebToken(dpop);
            handler.ReadToken(dpop);
            if (jwt.Typ != DPoPConstants.DPoPTyp) return DPoPValidationResult.Error($"the typ must be equals to {DPoPConstants.DPoPTyp}");
            if (jwt.Alg == "none") return DPoPValidationResult.Error("the alg cannot be equals to none");
            if (!supportedAlgs.Contains(jwt.Alg)) return DPoPValidationResult.Error($"the alg {jwt.Alg} is not supported");
            string jwkJson;
            if (!jwt.TryGetHeaderValue(DPoPConstants.Jwk, out jwkJson)) return DPoPValidationResult.Error("the public key is missing");
            var htm = jwt.Htm();
            if (string.IsNullOrWhiteSpace(htm)) return DPoPValidationResult.Error($"the parameter {DPoPConstants.DPoPClaims.Htm} is missing");
            var htu = jwt.Htu();
            if (string.IsNullOrWhiteSpace(htu)) return DPoPValidationResult.Error($"the parameter {DPoPConstants.DPoPClaims.Htu} is missing");
            if (htm != httpMethod) return DPoPValidationResult.Error($"the {DPoPConstants.DPoPClaims.Htm} parameter must be equals to {httpMethod}");
            if (htu != httpRequest) return DPoPValidationResult.Error($"the {DPoPConstants.DPoPClaims.Htu} parameter must be equals to {httpRequest}");
            if (!IsSigValid()) return DPoPValidationResult.Error("the DPoP signature is not valid");
            var valParameters = new TokenValidationParameters
            {
                ValidateLifetime = true
            };
            try
            {
                Validators.ValidateLifetime(jwt.ValidFrom, jwt.ValidTo, jwt, valParameters);
            }
            catch
            {
                return DPoPValidationResult.Error("DPoP is expired");
            }

            var lifetime = (jwt.ValidTo - jwt.IssuedAt).TotalSeconds;
            if (lifetime > validityTimeSeconds) return DPoPValidationResult.Error($"the DPoP cannot have a validity superior to {validityTimeSeconds} seconds");
            return DPoPValidationResult.Ok(jwt);

            bool IsSigValid()
            {
                var encodedPayload = Encoding.UTF8.GetBytes(jwt.EncodedHeader + "." + jwt.EncodedPayload);
                var sig = Base64UrlEncoder.DecodeBytes(jwt.EncodedSignature);
                var jwk = JsonExtensions.DeserializeFromJson<JsonWebKey>(jwkJson);
                var verifier = jwk.CryptoProviderFactory.CreateForVerifying(jwk, jwt.Alg);
                var isValid = verifier.Verify(encodedPayload, sig);
                return isValid;
            }
        }

        public DPoPGenerationResult CreateRSA(IEnumerable<Claim> claims, string alg = SecurityAlgorithms.RsaSha256, double expiresInSeconds = 200)
        {
            var rsa = RSA.Create();
            var securityKey = new RsaSecurityKey(rsa);
            return Create(claims, securityKey, alg, expiresInSeconds);
        }

        public DPoPGenerationResult CreateRSA(string htm, string htu, string alg = SecurityAlgorithms.RsaSha256, double expiresInSeconds = 200)
        {
            var rsa = RSA.Create();
            var securityKey = new RsaSecurityKey(rsa);
            return Create(htm, htu, securityKey, alg, expiresInSeconds);
        }

        public DPoPGenerationResult CreateES(IEnumerable<Claim> claims, string alg = SecurityAlgorithms.EcdsaSha256, double expiresInSeconds = 200)
        {
            var curve = ECCurve.NamedCurves.nistP256;
            if (alg == SecurityAlgorithms.EcdsaSha384) curve = ECCurve.NamedCurves.nistP384;
            else if (alg == SecurityAlgorithms.EcdsaSha512) curve = ECCurve.NamedCurves.nistP521;
            var securityKey = new ECDsaSecurityKey(ECDsa.Create(curve));
            return Create(claims, securityKey, alg, expiresInSeconds);
        }

        public DPoPGenerationResult Create(string htm, string htu, SecurityKey securityKey, string alg, double expiresInSeconds = 200)
        {
            var claims = new List<Claim>
            {
                new Claim(DPoPConstants.DPoPClaims.Htm, htm),
                new Claim(DPoPConstants.DPoPClaims.Htu, htu)
            };
            return Create(claims, securityKey, alg, expiresInSeconds);
        }

        public DPoPGenerationResult Create(IEnumerable<Claim> claims, SecurityKey securityKey, string alg, double expiresInSeconds = 200)
        {
            var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(securityKey);
            var token = CreateToken();
            return new DPoPGenerationResult(jwk, token);

            string CreateToken()
            {
                var signingCredentials = new SigningCredentials(securityKey, alg);
                var publicKey = signingCredentials.SerializePublicJWK(null);
                var publicKeyJson = JsonNode.Parse(JsonExtensions.SerializeToJson(publicKey)).AsObject();
                var jwk = new JsonObject();
                foreach (var record in publicKeyJson) jwk.Add(record.Key, record.Value.AsValue().GetValue<string>());
                var header = new JsonObject
                {
                    { Microsoft.IdentityModel.JsonWebTokens.JwtHeaderParameterNames.Alg, alg },
                    { DPoPConstants.Jwk, jwk },
                    { Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Typ, DPoPConstants.DPoPTyp }
                };

                var payload = new JsonObject();
                var iat = DateTime.UtcNow;
                var exp = iat.AddSeconds(expiresInSeconds);
                payload.Add(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(iat));
                payload.Add(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Nbf, EpochTime.GetIntDate(iat));
                payload.Add(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Exp, EpochTime.GetIntDate(exp));
                foreach (var cl in claims) payload.Add(cl.Type, cl.Value);

                var rawHeader = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(header.ToJsonString()));
                var rawPayload = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(payload.ToJsonString()));
                var content = $"{rawHeader}.{rawPayload}";
                var sig = securityKey.CryptoProviderFactory.CreateForSigning(securityKey, alg);
                var sigPayload = sig.Sign(Encoding.UTF8.GetBytes(content));
                return $"{content}.{Base64UrlEncoder.Encode(sigPayload)}";
            }
        }
    }
}
