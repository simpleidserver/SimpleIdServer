// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Jwt
{
    public interface IJwtParser
    {
        bool IsJweToken(string jwe);
        bool IsJwsToken(string jws);
        Task<string> Decrypt(string jwe);
        string Decrypt(string jwe, JsonWebKey jsonWebKey);
        Task<string> DecryptWithPassword(string jwe, string password);
        Task<string> Decrypt(string jwe, string clientId);
        Task<string> Decrypt(string jwe, OAuthClient client);
        Task<string> Decrypt(string jwe, string clientId, string password);
        JwsHeader ExtractJwsHeader(string jws);
        JweHeader ExtractJweHeader(string jwe);
        JwsPayload ExtractJwsPayload(string jws);
        Task<JwsPayload> Unsign(string jws);
        Task<JwsPayload> Unsign(string jws, string clientId);
        Task<JwsPayload> Unsign(string jws, OAuthClient client);
        JwsPayload Unsign(string jws, JsonWebKey jwk);
    }


    public class JwtParser : IJwtParser
    {
        private readonly IJweGenerator _jweGenerator;
        private readonly IJwsGenerator _jwsGenerator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IJsonWebKeyQueryRepository _jsonWebKeyRepository;

        public JwtParser(
            IJweGenerator jweGenerator,
            IJwsGenerator jwsGenerator,
            IHttpClientFactory httpClientFactory,
            IOAuthClientQueryRepository oauthClientRepository,
            IJsonWebKeyQueryRepository jsonWebKeyRepository)
        {
            _jweGenerator = jweGenerator;
            _jwsGenerator = jwsGenerator;
            _httpClientFactory = httpClientFactory;
            _oauthClientRepository = oauthClientRepository;
            _jsonWebKeyRepository = jsonWebKeyRepository;
        }

        public bool IsJweToken(string jwe)
        {
            IEnumerable<string> parts = null;
            return _jweGenerator.IsValid(jwe, out parts);
        }

        public bool IsJwsToken(string jws)
        {
            IEnumerable<string> parts = null;
            return _jwsGenerator.IsValid(jws, out parts);
        }

        public async Task<string> Decrypt(string jwe)
        {
            if (string.IsNullOrWhiteSpace(jwe))
            {
                throw new ArgumentNullException(nameof(jwe));
            }

            var protectedHeader = _jweGenerator.ExtractHeader(jwe);
            if (protectedHeader == null)
            {
                return string.Empty;
            }

            var jsonWebKey = await _jsonWebKeyRepository.FindJsonWebKeyById(protectedHeader.Kid);
            if (jsonWebKey == null)
            {
                return string.Empty;
            }

            return _jweGenerator.Decrypt(jwe, jsonWebKey);
        }

        public string Decrypt(string jwe, JsonWebKey jsonWebKey)
        {
            return _jweGenerator.Decrypt(jwe, jsonWebKey);
        }

        public async Task<string> DecryptWithPassword(string jwe, string password)
        {
            if (string.IsNullOrWhiteSpace(jwe))
            {
                throw new ArgumentNullException(nameof(jwe));
            }

            var protectedHeader = _jweGenerator.ExtractHeader(jwe);
            if (protectedHeader == null)
            {
                return string.Empty;
            }

            var jsonWebKey = await _jsonWebKeyRepository.FindJsonWebKeyById(protectedHeader.Kid);
            if (jsonWebKey == null)
            {
                return string.Empty;
            }

            return _jweGenerator.Decrypt(jwe, jsonWebKey, password);
        }

        public async Task<string> Decrypt(string jwe, string clientId)
        {
            var jsonWebKey = await GetJsonWebKeyToDecrypt(jwe, clientId);
            if (jsonWebKey == null)
            {
                return string.Empty;
            }

            return _jweGenerator.Decrypt(jwe, jsonWebKey);
        }

        public async Task<string> Decrypt(string jwe, OAuthClient client)
        {
            var jsonWebKey = await GetJsonWebKeyToDecrypt(jwe, client);
            if (jsonWebKey == null)
            {
                return string.Empty;
            }

            return _jweGenerator.Decrypt(jwe, jsonWebKey);
        }

        public async Task<string> Decrypt(string jwe, string clientId, string password)
        {
            var jsonWebKey = await GetJsonWebKeyToDecrypt(jwe, clientId);
            if (jsonWebKey == null)
            {
                return string.Empty;
            }

            return _jweGenerator.Decrypt(jwe, jsonWebKey, password);
        }

        public JwsHeader ExtractJwsHeader(string jws)
        {
            return _jwsGenerator.ExtractHeader(jws);
        }

        public JweHeader ExtractJweHeader(string jwe)
        {
            return _jweGenerator.ExtractHeader(jwe);
        }

        public JwsPayload ExtractJwsPayload(string jws)
        {
            return _jwsGenerator.ExtractPayload(jws);
        }

        public async Task<JwsPayload> Unsign(string jws)
        {
            if (string.IsNullOrWhiteSpace(jws))
            {
                throw new ArgumentNullException(nameof(jws));
            }

            var protectedHeader = _jwsGenerator.ExtractHeader(jws);
            if (protectedHeader == null)
            {
                return null;
            }

            var jsonWebKey = await _jsonWebKeyRepository.FindJsonWebKeyById(protectedHeader.Kid);
            return _jwsGenerator.ExtractPayload(jws, jsonWebKey);
        }

        public async Task<JwsPayload> Unsign(string jws, string clientId)
        {
            var client = await _oauthClientRepository.FindOAuthClientById(clientId);
            if (client == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            return await Unsign(jws, client);
        }

        public async Task<JwsPayload> Unsign(string jws, OAuthClient client)
        {
            var protectedHeader = _jwsGenerator.ExtractHeader(jws);
            if (protectedHeader == null)
            {
                return null;
            }

            var jsonWebKey = await GetJsonWebKeyFromClient(client, protectedHeader.Kid);
            return Unsign(jws, jsonWebKey);
        }

        public JwsPayload Unsign(string jws, JsonWebKey jwk)
        {
            return _jwsGenerator.ExtractPayload(jws, jwk);
        }

        private async Task<JsonWebKey> GetJsonWebKeyToDecrypt(string jwe, string clientId)
        {
            var client = await _oauthClientRepository.FindOAuthClientById(clientId);
            if (client == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            return await GetJsonWebKeyToDecrypt(jwe, client);
        }

        private Task<JsonWebKey> GetJsonWebKeyToDecrypt(string jwe, OAuthClient client)
        {
            var protectedHeader = _jweGenerator.ExtractHeader(jwe);
            if (protectedHeader == null)
            {
                return null;
            }

            return GetJsonWebKeyFromClient(client, protectedHeader.Kid);
        }

        private async Task<JsonWebKey> GetJsonWebKeyFromClient(OAuthClient client, string kid)
        {
            var jsonWebKeys = await client.ResolveJsonWebKeys(_httpClientFactory);
            return jsonWebKeys.FirstOrDefault(j => j.Kid == kid);
        }
    }
}
