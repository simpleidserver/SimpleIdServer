// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class AccessTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IdServerHostOptions _options;

        public AccessTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IJwtBuilder jwtBuilder, IOptions<IdServerHostOptions> options)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _jwtBuilder = jwtBuilder;
            _options = options.Value;
        }

        public string Name => TokenResponseParameters.AccessToken;

        public async virtual Task Build(BuildTokenParameter parameter, HandlerContext handlerContext, CancellationToken cancellationToken, bool useOriginalRequest = false)
        {
            var tokenDescriptor = BuildOpenIdPayload(parameter.Scopes, parameter.Audiences, parameter.Claims, parameter.AuthorizationDetails, handlerContext);
            if(parameter.AdditionalClaims != null)
                foreach(var claim in parameter.AdditionalClaims)
                    tokenDescriptor.Claims.Add(claim.Key, claim.Value);

            await SetResponse(handlerContext.Realm, parameter.GrantId, handlerContext, tokenDescriptor, cancellationToken);
        }
        
        protected virtual SecurityTokenDescriptor BuildOpenIdPayload(IEnumerable<string> scopes, IEnumerable<string> resources, IEnumerable<AuthorizedClaim> claims, ICollection<AuthorizationData> authorizationDetails, HandlerContext handlerContext)
        {
            var jwsPayload = BuildTokenDescriptor(scopes, authorizationDetails, resources, handlerContext);
            if (handlerContext.User != null)
            {
                jwsPayload.Claims.Add(JwtRegisteredClaimNames.Sub, handlerContext.User.Name);
                var activeSession = handlerContext.Session;
                if (activeSession != null)
                    jwsPayload.Claims.Add(JwtRegisteredClaimNames.AuthTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());
            }

            if (claims != null && claims.Any())
            {
                var dic = new Dictionary<string, object>();
                foreach (var claim in claims)
                    claim.Serialize(dic);

                jwsPayload.Claims.Add(AuthorizationRequestParameters.Claims, dic);
            }

            return jwsPayload;
        }

        protected virtual SecurityTokenDescriptor BuildTokenDescriptor(IEnumerable<string> scopes, ICollection<AuthorizationData> authorizationDetails, IEnumerable<string> audiences, HandlerContext handlerContext)
        {
            var tokenDescriptor = _grantedTokenHelper.BuildAccessToken(handlerContext.Client.ClientId, audiences, scopes, authorizationDetails, handlerContext.GetIssuer(), handlerContext.Client.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds);
            var cnf = new Dictionary<string, object>();
            if (handlerContext.Request.Certificate != null)
            {
                var thumbprint = Hash(handlerContext.Request.Certificate.RawData);
                cnf.Add(JsonWebKeyParameterNames.X5tS256, thumbprint);
            }

            if (handlerContext.DPOPProof != null)
            {
                var publicKey = handlerContext.DPOPProof.PublicKey();
                cnf.Add(AdditionalJsonWebKeyParameterNames.Jkt, publicKey.CreateThumbprint());
            }

            if(cnf.Any())
                tokenDescriptor.Claims.Add(ConfirmationClaimTypes.Cnf, cnf);

            return tokenDescriptor;
        }

        protected async Task SetResponse(string realm, string grantId, HandlerContext handlerContext, SecurityTokenDescriptor securityTokenDescriptor, CancellationToken cancellationToken)
        {
            var authorizationCode = string.Empty;
            if (!handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode))
                authorizationCode = handlerContext.Request.RequestData.GetAuthorizationCode();

            var id = Guid.NewGuid().ToString();
            var accessToken = await _jwtBuilder.BuildAccessToken(realm, handlerContext.Client, securityTokenDescriptor, cancellationToken);
            if (handlerContext.Client.AccessTokenType == AccessTokenTypes.Jwt)
            {
                await _grantedTokenHelper.AddJwtAccessToken(accessToken, handlerContext.Client.ClientId, authorizationCode, grantId, cancellationToken);
                id = accessToken;
            }
            else
            {
                await _grantedTokenHelper.AddReferenceAccessToken(id, accessToken, handlerContext.Client.ClientId, authorizationCode, grantId, cancellationToken);
            }

            handlerContext.Response.Add(TokenResponseParameters.AccessToken, id);
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