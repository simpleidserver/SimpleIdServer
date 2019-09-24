// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IGrantedTokenHelper
    {
        JwsPayload BuildAccessToken(IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName);
        JwsPayload BuildAccessToken(IEnumerable<string> audiences, IEnumerable<string> scopes, string issuerName, double validityPeriodsInSeconds);
        void RefreshAccessToken(JwsPayload jwsPayload, double validityPeriodsInSeconds);
        string BuildRefreshToken(JwsPayload jwsPayload, double validityPeriodsInSeconds);
        JwsPayload GetRefreshToken(string refreshToken);
        void RemoveRefreshToken(string refreshToken);
        string BuildAuthorizationCode(JwsPayload jwsPayload);
        JwsPayload GetAuthorizationCode(string code);
        void RemoveAuthorizationCode(string code);
    }

    public class GrantedTokenHelper : IGrantedTokenHelper
    {
        private readonly IDistributedCache _distributedCache;

        public GrantedTokenHelper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

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

        public void RefreshAccessToken(JwsPayload jwsPayload, double validityPeriodsInSeconds)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(validityPeriodsInSeconds);
            jwsPayload[OAuthClaims.Iat] = currentDateTime.ConvertToUnixTimestamp();
            jwsPayload[OAuthClaims.ExpirationTime] = expirationDateTime.ConvertToUnixTimestamp();
        }

        public string BuildRefreshToken(JwsPayload jwsPayload, double validityPeriodsInSeconds)
        {
            var refreshToken = Guid.NewGuid().ToString();
            var json = JsonConvert.SerializeObject(jwsPayload);
            _distributedCache.Set(refreshToken, Encoding.UTF8.GetBytes(json), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(validityPeriodsInSeconds)
            });
            return refreshToken;
        }

        public JwsPayload GetRefreshToken(string refreshToken)
        {
            var cache = _distributedCache.Get(refreshToken);
            if (cache == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JwsPayload>(Encoding.UTF8.GetString(cache));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _distributedCache.Remove(refreshToken);
        }

        public string BuildAuthorizationCode(JwsPayload jwsPayload)
        {
            var json = JsonConvert.SerializeObject(jwsPayload);
            var code = Guid.NewGuid().ToString();
            _distributedCache.Set(code, Encoding.UTF8.GetBytes(json));
            return code;
        }

        public JwsPayload GetAuthorizationCode(string code)
        {
            var cache = _distributedCache.Get(code);
            if (cache == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<JwsPayload>(Encoding.UTF8.GetString(cache));
        }

        public void RemoveAuthorizationCode(string code)
        {
            _distributedCache.Remove(code);
        }

        private void AddExpirationAndIssueTime(JwsPayload jwsPayload, double validityPeriodsInSeconds)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(validityPeriodsInSeconds);
            jwsPayload.Add(OAuthClaims.Iat, currentDateTime.ConvertToUnixTimestamp());
            jwsPayload.Add(OAuthClaims.ExpirationTime, expirationDateTime.ConvertToUnixTimestamp());
        }
    }
}