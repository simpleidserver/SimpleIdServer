// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IGrantedTokenHelper
    {    
        Task<SearchResult<Token>> SearchTokens(SearchTokenParameter parameter, CancellationToken cancellationToken);
        Task<bool> RemoveTokens(IEnumerable<Token> tokens, CancellationToken cancellationToken);
        Task<bool> AddToken(Token token, CancellationToken cancellationToken);
        JwsPayload BuildAccessToken(IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName);
        JwsPayload BuildAccessToken(IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName, double validityPeriodsInSeconds);
        Task<bool> AddAccessToken(string token, string clientId, string authorizationCode, CancellationToken cancellationToken);
        Task<JwsPayload> GetAccessToken(string accessToken, CancellationToken cancellationToken);
        Task<bool> TryRemoveAccessToken(string accessToken, string clientId, CancellationToken cancellationToken);
        void RefreshAccessToken(JwsPayload jwsPayload, double validityPeriodsInSeconds);
        Task<string> AddRefreshToken(string clientId, string authorizationCode, JObject jwsPayload, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task<Token> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task RemoveRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<bool> TryRemoveRefreshToken(string refreshToken, string clientId, CancellationToken cancellationToken);
        Task<string> AddAuthorizationCode(JObject request, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task<JObject> GetAuthorizationCode(string code, CancellationToken cancellationToken);
        Task RemoveAuthorizationCode(string code, CancellationToken cancellationToken);
    }

    public class GrantedTokenHelper : IGrantedTokenHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ITokenCommandRepository _tokenCommandRepository;
        private readonly ITokenQueryRepository _tokenQueryRepository;

        public GrantedTokenHelper(IDistributedCache distributedCache, ITokenCommandRepository tokenCommandRepository, ITokenQueryRepository tokenQueryRepository)
        {
            _distributedCache = distributedCache;
            _tokenCommandRepository = tokenCommandRepository;
            _tokenQueryRepository = tokenQueryRepository;
        }

        #region Tokens

        public Task<SearchResult<Token>> SearchTokens(SearchTokenParameter parameter, CancellationToken cancellationToken)
        {
            return _tokenQueryRepository.Find(parameter, cancellationToken);
        }

        public async Task<bool> RemoveTokens(IEnumerable<Token> tokens, CancellationToken cancellationToken)
        {
            foreach(var token in tokens)
            {
                await _tokenCommandRepository.Delete(token, cancellationToken);
            }

            await _tokenCommandRepository.SaveChanges(cancellationToken);
            return true;
        }

        public Task<bool> AddToken(Token token, CancellationToken cancellationToken)
        {
            return _tokenCommandRepository.Add(token, cancellationToken);
        }

        #endregion

        #region Access token

        public JwsPayload BuildAccessToken(IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName)
        {
            return new JwsPayload
            {
                { OAuthClaims.Audiences, audiences },
                { OAuthClaims.Issuer, issuerName },
                { OAuthClaims.Scopes, scopes }
            };
        }

        public JwsPayload BuildAccessToken(IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName, double validityPeriodsInSeconds)
        {
            var jwsPayload = BuildAccessToken(audiences, scopes, issuerName);
            AddExpirationAndIssueTime(jwsPayload, validityPeriodsInSeconds);
            return jwsPayload;
        }

        public async Task<bool> AddAccessToken(string token, string clientId, string authorizationCode, CancellationToken cancellationToken)
        {
            await _tokenCommandRepository.Add(new Token
            {
                Id = token,
                ClientId = clientId,
                CreateDateTime = DateTime.UtcNow,
                TokenType = DTOs.TokenResponseParameters.AccessToken,
                AuthorizationCode = authorizationCode
            }, cancellationToken);
            await _tokenCommandRepository.SaveChanges(cancellationToken);
            return true;
        }

        public async Task<JwsPayload> GetAccessToken(string accessToken, CancellationToken cancellationToken)
        {
            var result = await _tokenQueryRepository.Get(accessToken, cancellationToken);
            if (result == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JwsPayload>(result.Data);
        }

        public async Task<bool> TryRemoveAccessToken(string accessToken, string clientId, CancellationToken cancellationToken)
        {
            var result = await _tokenQueryRepository.Get(accessToken,cancellationToken);
            if (result == null)
            {
                return false;
            }

            if (result.ClientId != clientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.UNAUTHORIZED_CLIENT);
            }

            await _tokenCommandRepository.Delete(result, cancellationToken);
            await _tokenCommandRepository.SaveChanges(cancellationToken);
            return true;
        }

        public void RefreshAccessToken(JwsPayload jwsPayload, double validityPeriodsInSeconds)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(validityPeriodsInSeconds);
            jwsPayload[OAuthClaims.Iat] = currentDateTime.ConvertToUnixTimestamp();
            jwsPayload[OAuthClaims.ExpirationTime] = expirationDateTime.ConvertToUnixTimestamp();
        }

        #endregion

        #region Refresh token

        public async Task<Token> GetRefreshToken(string refreshToken, CancellationToken token)
        {
            var cache = await _tokenQueryRepository.Get(refreshToken, token);
            if (cache == null)
            {
                return null;
            }

            return cache;
        }

        public async Task<string> AddRefreshToken(string clientId, string authorizationCode, JObject request, double validityPeriodsInSeconds, CancellationToken cancellationToken)
        {
            var refreshToken = Guid.NewGuid().ToString();
            await _tokenCommandRepository.Add(new Token
            {
                Id = refreshToken,
                TokenType = DTOs.TokenResponseParameters.RefreshToken,
                ClientId = clientId,
                Data = request.ToString(),
                AuthorizationCode = authorizationCode,
                ExpirationTime = DateTime.UtcNow.AddSeconds(validityPeriodsInSeconds),
                CreateDateTime = DateTime.UtcNow,
            }, cancellationToken);
            await _tokenCommandRepository.SaveChanges(cancellationToken);
            return refreshToken;
        }

        public Task RemoveRefreshToken(string refreshToken, CancellationToken token)
        {
            return _distributedCache.RemoveAsync(refreshToken, token);
        }

        public async Task<bool> TryRemoveRefreshToken(string refreshToken, string clientId, CancellationToken cancellationToken)
        {
            var result = await _tokenQueryRepository.Get(refreshToken, cancellationToken);
            if (result == null)
            {
                return false;
            }

            if (result.ClientId != clientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.UNAUTHORIZED_CLIENT);
            }

            await _tokenCommandRepository.Delete(result, cancellationToken);
            await _tokenCommandRepository.SaveChanges(cancellationToken);
            return true;
        }

        #endregion

        #region Authorization code

        public async Task<JObject> GetAuthorizationCode(string code, CancellationToken token)
        {
            var cache = await _distributedCache.GetAsync(code, token);
            if (cache == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(cache));
        }

        public async Task<string> AddAuthorizationCode(JObject request, double validityPeriodsInSeconds, CancellationToken cancellationToken)
        {
            var code = Guid.NewGuid().ToString();
            await _distributedCache.SetAsync(code, Encoding.UTF8.GetBytes(request.ToString()), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(validityPeriodsInSeconds)
            }, cancellationToken);
            return code;
        }

        public Task RemoveAuthorizationCode(string code, CancellationToken cancellationToken)
        {
            return _distributedCache.RemoveAsync(code, cancellationToken);
        }

        #endregion

        private static void AddExpirationAndIssueTime(JwsPayload jwsPayload, double validityPeriodsInSeconds)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(validityPeriodsInSeconds);
            jwsPayload.Add(OAuthClaims.Iat, currentDateTime.ConvertToUnixTimestamp());
            jwsPayload.Add(OAuthClaims.ExpirationTime, expirationDateTime.ConvertToUnixTimestamp());
        }
    }
}