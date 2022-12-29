// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IGrantedTokenHelper
    {    
        Task<IEnumerable<Token>> GetTokensByAuthorizationCode(string code, CancellationToken cancellationToken);
        Task<bool> RemoveToken(string token, CancellationToken cancellationToken);
        Task<bool> RemoveTokens(IEnumerable<Token> tokens, CancellationToken cancellationToken);
        SecurityTokenDescriptor BuildAccessToken(string clientId,IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName);
        SecurityTokenDescriptor BuildAccessToken(string clientId,IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName, double validityPeriodsInSeconds);
        Task<bool> AddAccessToken(string token, string clientId, string authorizationCode, CancellationToken cancellationToken);
        Task<JsonWebToken> GetAccessToken(string accessToken, CancellationToken cancellationToken);
        Task<bool> TryRemoveAccessToken(string accessToken, string clientId, CancellationToken cancellationToken);
        Task<string> AddRefreshToken(string clientId, string authorizationCode, JsonObject jwsPayload, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task<Token> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task RemoveRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<bool> TryRemoveRefreshToken(string refreshToken, string clientId, CancellationToken cancellationToken);
        Task<string> AddAuthorizationCode(JsonObject request, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task<JsonObject> GetAuthorizationCode(string code, CancellationToken cancellationToken);
        Task RemoveAuthorizationCode(string code, CancellationToken cancellationToken);
    }

    public class GrantedTokenHelper : IGrantedTokenHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ITokenRepository _tokenRepository;
        private readonly OAuthHostOptions _oauthHostOptions;

        public GrantedTokenHelper(IDistributedCache distributedCache, ITokenRepository tokenRepository, IOptions<OAuthHostOptions> oauthHostOptions)
        {
            _distributedCache = distributedCache;
            _tokenRepository = tokenRepository;
            _oauthHostOptions = oauthHostOptions.Value;
        }

        #region Tokens

        public async Task<IEnumerable<Token>> GetTokensByAuthorizationCode(string code, CancellationToken cancellationToken)
        {
            var result = await _tokenRepository.Query().Where(t => t.AuthorizationCode == code).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<bool> RemoveToken(string token, CancellationToken cancellationToken)
        {
            var result = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Id == token, cancellationToken);
            if (result == null)
            {
                return false;
            }

            _tokenRepository.Remove(result);
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        public async Task<bool> RemoveTokens(IEnumerable<Token> tokens, CancellationToken cancellationToken)
        {
            foreach(var token in tokens)
                _tokenRepository.Remove(token);
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        #endregion

        #region Access token

        public SecurityTokenDescriptor BuildAccessToken(string clientId, IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName)
        {
            var claims = new Dictionary<string, object>
            {
                { System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud, audiences },
                { OpenIdConnectParameterNames.ClientId, clientId },
                { OpenIdConnectParameterNames.Scope, BuildScopeClaim(scopes) }
            };
            return new SecurityTokenDescriptor
            {
                Issuer = issuerName,
                Claims = claims
            };
        }

        public SecurityTokenDescriptor BuildAccessToken(string clientId, IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName, double validityPeriodsInSeconds)
        {
            var descriptor = BuildAccessToken(clientId, audiences, scopes, issuerName);
            AddExpirationAndIssueTime(descriptor, validityPeriodsInSeconds);
            return descriptor;
        }

        public async Task<bool> AddAccessToken(string token, string clientId, string authorizationCode, CancellationToken cancellationToken)
        {
            _tokenRepository.Add(new Token
            {
                Id = token,
                ClientId = clientId,
                CreateDateTime = DateTime.UtcNow,
                TokenType = DTOs.TokenResponseParameters.AccessToken,
                AuthorizationCode = authorizationCode
            });
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        public async Task<JsonWebToken> GetAccessToken(string accessToken, CancellationToken cancellationToken)
        {
            var result = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Id == accessToken, cancellationToken);
            if (result == null) return null;
            var handler = new JsonWebTokenHandler();
            return handler.ReadJsonWebToken(result.Id);
        }

        public async Task<bool> TryRemoveAccessToken(string accessToken, string clientId, CancellationToken cancellationToken)
        {
            var result = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Id == accessToken, cancellationToken);
            if (result == null) return false;
            if (result.ClientId != clientId) throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.UNAUTHORIZED_CLIENT);
            _tokenRepository.Remove(result);
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        #endregion

        #region Refresh token

        public async Task<Token> GetRefreshToken(string refreshToken, CancellationToken token)
        {
            var cache = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Id == refreshToken, token);
            if (cache == null) return null;
            return cache;
        }

        public async Task<string> AddRefreshToken(string clientId, string authorizationCode, JsonObject request, double validityPeriodsInSeconds, CancellationToken cancellationToken)
        {
            var refreshToken = Guid.NewGuid().ToString();
            _tokenRepository.Add(new Token
            {
                Id = refreshToken,
                TokenType = DTOs.TokenResponseParameters.RefreshToken,
                ClientId = clientId,
                Data = request.ToString(),
                AuthorizationCode = authorizationCode,
                ExpirationTime = DateTime.UtcNow.AddSeconds(validityPeriodsInSeconds),
                CreateDateTime = DateTime.UtcNow,
            });
            await _tokenRepository.SaveChanges(cancellationToken);
            return refreshToken;
        }

        public Task RemoveRefreshToken(string refreshToken, CancellationToken token)
        {
            return _distributedCache.RemoveAsync(refreshToken, token);
        }

        public async Task<bool> TryRemoveRefreshToken(string refreshToken, string clientId, CancellationToken cancellationToken)
        {
            var result = await _tokenRepository.Query().FirstOrDefaultAsync(r => r.Id == refreshToken, cancellationToken);
            if (result == null)
            {
                return false;
            }

            if (result.ClientId != clientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.UNAUTHORIZED_CLIENT);
            }

            _tokenRepository.Remove(result);
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        #endregion

        #region Authorization code

        public async Task<JsonObject> GetAuthorizationCode(string code, CancellationToken token)
        {
            var cache = await _distributedCache.GetAsync(code, token);
            if (cache == null) return null;
            return JsonObject.Parse(Encoding.UTF8.GetString(cache)).AsObject();
        }

        public async Task<string> AddAuthorizationCode(JsonObject request, double validityPeriodsInSeconds, CancellationToken cancellationToken)
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

        public object BuildScopeClaim(IEnumerable<string> scopes)
        {
            if (_oauthHostOptions.IsScopeClaimConcatenationEnabled) return string.Join(" ", scopes);
            return scopes;
        }

        private static void AddExpirationAndIssueTime(SecurityTokenDescriptor descriptor, double validityPeriodsInSeconds)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(validityPeriodsInSeconds);
            descriptor.Expires = expirationDateTime;
            descriptor.IssuedAt = currentDateTime;
        }
    }
}