// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IGrantedTokenHelper
    {    
        Task<IEnumerable<Token>> GetTokensByAuthorizationCode(string code, CancellationToken cancellationToken);
        Task<bool> RemoveToken(string token, CancellationToken cancellationToken);
        Task<bool> RemoveTokens(IEnumerable<Token> tokens, CancellationToken cancellationToken);
        SecurityTokenDescriptor BuildAccessToken(string clientId,IEnumerable<string> audiences, IEnumerable<string> scopes, ICollection<AuthorizationData> authorizationDetails, string issuerName);
        SecurityTokenDescriptor BuildAccessToken(string clientId,IEnumerable<string> audiences, IEnumerable<string> scopes, ICollection<AuthorizationData> authorizationDetails, string issuerName, double validityPeriodsInSeconds);
        Task<bool> AddJwtAccessToken(string token, string clientId, string authorizationCode, string grantId, CancellationToken cancellationToken);
        Task<bool> AddReferenceAccessToken(string id, string token, string clientId, string authorizationCode, string grantId, CancellationToken cancellationToken);
        Task<JsonWebToken> GetAccessToken(string accessToken, CancellationToken cancellationToken);
        Task<bool> TryRemoveAccessToken(string accessToken, string clientId, CancellationToken cancellationToken);
        Task<string> AddRefreshToken(string clientId, string authorizationCode, string grantId, JsonObject currentRequest, JsonObject originalRequest, double validityPeriodsInSeconds, string jkt, string sessionId, CancellationToken cancellationToken);
        Task<Token> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task RemoveRefreshToken(string refreshToken, CancellationToken cancellationToken);
        Task<bool> TryRemoveRefreshToken(string refreshToken, string clientId, CancellationToken cancellationToken);
        Task<string> AddAuthorizationCode(JsonObject originalRequest, string grantId, double validityPeriodsInSeconds, string dpopJkt, string sessionId, CancellationToken cancellationToken);
        Task<AuthCode> GetAuthorizationCode(string code, CancellationToken cancellationToken);
        Task RemoveAuthorizationCode(string code, CancellationToken cancellationToken);
        Task AddPreAuthCode(string preAuthorizationCode, string pin, string clientId, string userId, double validityPeriodsInSeconds, CancellationToken cancellationToken);
        Task<PreAuthCode> GetPreAuthCode(string preAuthorizationCode, CancellationToken cancellationToken);
        Task RemovePreAuthCode(string preAuthorizationCode, CancellationToken cancellationToken);
    }

    public class AuthCode
    {
        public JsonObject OriginalRequest { get; set; }
        public string GrantId { get; set; }
        public string DPOPJkt { get; set; }
        public string SessionId { get; set; }
    }

    public class GrantedTokenHelper : IGrantedTokenHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ITokenRepository _tokenRepository;
        private readonly IdServerHostOptions _oauthHostOptions;

        public GrantedTokenHelper(IDistributedCache distributedCache, ITokenRepository tokenRepository, IOptions<IdServerHostOptions> oauthHostOptions)
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

        public SecurityTokenDescriptor BuildAccessToken(string clientId, IEnumerable<string> audiences, IEnumerable<string> scopes, ICollection<AuthorizationData> authorizationDetails, string issuerName)
        {
            if (audiences == null || !audiences.Any()) audiences = new List<string> { clientId };
            var claims = new Dictionary<string, object>
            {
                { System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud, audiences },
                { OpenIdConnectParameterNames.ClientId, clientId },
                { OpenIdConnectParameterNames.Scope, BuildScopeClaim(scopes) }
            };
            if(authorizationDetails != null && authorizationDetails.Any())
                claims.Add(AuthorizationRequestParameters.AuthorizationDetails, authorizationDetails.Select(d => d.Serialize()));

            return new SecurityTokenDescriptor
            {
                Issuer = issuerName,
                Claims = claims
            };
        }

        public SecurityTokenDescriptor BuildAccessToken(string clientId, IEnumerable<string> audiences, IEnumerable<string> scopes, ICollection<AuthorizationData> authorizationDetails, string issuerName, double validityPeriodsInSeconds)
        {
            var descriptor = BuildAccessToken(clientId, audiences, scopes, authorizationDetails, issuerName);
            AddExpirationAndIssueTime(descriptor, validityPeriodsInSeconds);
            return descriptor;
        }

        public async Task<bool> AddJwtAccessToken(string token, string clientId, string authorizationCode, string grantId, CancellationToken cancellationToken)
        {
            _tokenRepository.Add(new Token
            {
                Id = token,
                ClientId = clientId,
                CreateDateTime = DateTime.UtcNow,
                TokenType = DTOs.TokenResponseParameters.AccessToken,
                AccessTokenType = AccessTokenTypes.Jwt,
                AuthorizationCode = authorizationCode,
                GrantId = grantId
            });
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        public async Task<bool> AddReferenceAccessToken(string id, string token, string clientId, string authorizationCode, string grantId, CancellationToken cancellationToken)
        {
            _tokenRepository.Add(new Token
            {
                Id = id,
                ClientId = clientId,
                CreateDateTime = DateTime.UtcNow,
                TokenType = DTOs.TokenResponseParameters.AccessToken,
                AccessTokenType = AccessTokenTypes.Reference,
                AuthorizationCode = authorizationCode,
                GrantId = grantId,
                Data = token
            });
            await _tokenRepository.SaveChanges(cancellationToken);
            return true;
        }

        public async Task<JsonWebToken> GetAccessToken(string accessToken, CancellationToken cancellationToken)
        {
            var result = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Id == accessToken, cancellationToken);
            if (result == null) return null;
            var handler = new JsonWebTokenHandler();
            if(result.AccessTokenType == AccessTokenTypes.Jwt) return handler.ReadJsonWebToken(result.Id);
            return handler.ReadJsonWebToken(result.Data);
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

        public async Task<string> AddRefreshToken(string clientId, string authorizationCode, string grantId, JsonObject request, JsonObject originalRequest, double validityPeriodsInSeconds, string jkt, string sessionId, CancellationToken cancellationToken)
        {
            var refreshToken = Guid.NewGuid().ToString();
            _tokenRepository.Add(new Token
            {
                Id = refreshToken,
                TokenType = DTOs.TokenResponseParameters.RefreshToken,
                ClientId = clientId,
                Data = request.ToString(),
                OriginalData = originalRequest?.ToString(),
                AuthorizationCode = authorizationCode,
                ExpirationTime = DateTime.UtcNow.AddSeconds(validityPeriodsInSeconds),
                CreateDateTime = DateTime.UtcNow,
                GrantId = grantId,
                SessionId = sessionId,
                Jkt = jkt
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

        public async Task<AuthCode> GetAuthorizationCode(string code, CancellationToken token)
        {
            var cache = await _distributedCache.GetAsync(code, token);
            if (cache == null) return null;
            return JsonSerializer.Deserialize<AuthCode>(Encoding.UTF8.GetString(cache));
        }

        public async Task<string> AddAuthorizationCode(JsonObject originalRequest, string grantId, double validityPeriodsInSeconds, string dpopJkt, string sessionId, CancellationToken cancellationToken)
        {
            var code = Guid.NewGuid().ToString();
            var serializedAuthCode = JsonSerializer.Serialize(new AuthCode
            {
                GrantId = grantId,
                OriginalRequest = originalRequest,
                DPOPJkt = dpopJkt,
                SessionId = sessionId
            });
            await _distributedCache.SetAsync(code, Encoding.UTF8.GetBytes(serializedAuthCode), new DistributedCacheEntryOptions
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

        #region Pre Authorization Code

        public async Task AddPreAuthCode(string preAuthorizationCode, string pin, string clientId, string userId, double validityPeriodsInSeconds, CancellationToken cancellationToken)
        {
            var credOffer = new PreAuthCode { ClientId = clientId, Pin = pin, Code = preAuthorizationCode, UserId = userId };
            var serializedCredOffer = JsonSerializer.Serialize(credOffer);
            await _distributedCache.SetAsync(preAuthorizationCode, Encoding.UTF8.GetBytes(serializedCredOffer), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(validityPeriodsInSeconds)
            }, cancellationToken);
        }

        public Task RemovePreAuthCode(string preAuthorizationCode, CancellationToken cancellationToken) => _distributedCache.RemoveAsync(preAuthorizationCode, cancellationToken);

        public async Task<PreAuthCode> GetPreAuthCode(string preAuthorizationCode, CancellationToken cancellationToken)
        {
            var cachedToken = await _distributedCache.GetAsync(preAuthorizationCode, cancellationToken);
            if (cachedToken == null) return null;
            return JsonSerializer.Deserialize<PreAuthCode>(Encoding.UTF8.GetString(cachedToken));
        }

        #endregion

        #region User code

        public async Task AddUserCode(string userId, string realm, string code, double expirationTimeInSeconds, CancellationToken cancellationToken)
        {

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