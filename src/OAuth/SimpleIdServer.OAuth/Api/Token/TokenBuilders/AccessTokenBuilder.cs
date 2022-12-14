// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class AccessTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IClientRepository _clientRepository;

        public AccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IJwtBuilder jwtBuilder, IClientRepository clientRepository)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _jwtBuilder = jwtBuilder;
            _clientRepository = clientRepository;
        }

        public string Name => TokenResponseParameters.AccessToken;

        public virtual Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            return Build(scopes, new Dictionary<string, object>(), handlerContext, cancellationToken);
        }
        
        public async virtual Task Build(IEnumerable<string> scopes, Dictionary<string, object> claims, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var tokenDescriptor = await BuildTokenDescriptor(scopes, handlerContext, cancellationToken);
            foreach(var claim in claims) tokenDescriptor.Claims.Add(claim);
            await SetResponse(handlerContext, tokenDescriptor, cancellationToken);
        }

        public virtual Task Refresh(JsonObject previousRequest, HandlerContext currentContext, CancellationToken cancellationToken)
        {
            var scopes = previousRequest.GetScopesFromAuthorizationRequest();
            return Build(scopes, currentContext, cancellationToken);
        }

        protected virtual async Task<SecurityTokenDescriptor> BuildTokenDescriptor(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var audiences = await _clientRepository.Query().Include(c => c.Scopes).Where(c => c.Scopes.Any(s => scopes.Contains(s.Scope))).Select(c => c.ClientId).ToListAsync(cancellationToken);
            if (!audiences.Contains(handlerContext.Client.ClientId)) audiences.Add(handlerContext.Client.ClientId);
            var tokenDescriptor = _grantedTokenHelper.BuildAccessToken(audiences, scopes, handlerContext.Request.IssuerName, handlerContext.Client.TokenExpirationTimeInSeconds);
            if (handlerContext.Request.Certificate != null)
            {
                var thumbprint = Hash(handlerContext.Request.Certificate.RawData);
                tokenDescriptor.Claims.Add(ConfirmationClaimTypes.Cnf, new JsonObject
                {
                    { JsonWebKeyParameterNames.X5tS256, thumbprint }
                });
            }

            return tokenDescriptor;
        }

        protected async Task SetResponse(HandlerContext handlerContext, SecurityTokenDescriptor securityTokenDescriptor, CancellationToken cancellationToken)
        {
            var authorizationCode = string.Empty;
            if (!handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode))
            {
                authorizationCode = handlerContext.Request.RequestData.GetAuthorizationCode();
            }

            var accessToken = await _jwtBuilder.BuildAccessToken(handlerContext.Client, securityTokenDescriptor, cancellationToken);
            await _grantedTokenHelper.AddAccessToken(accessToken, handlerContext.Client.ClientId, authorizationCode, cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.AccessToken, accessToken);
        }

        private static string Hash(byte[] payload)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashed = sha256.ComputeHash(payload);
                return Base64UrlTextEncoder.Encode(hashed);
            }
        }
    }
}