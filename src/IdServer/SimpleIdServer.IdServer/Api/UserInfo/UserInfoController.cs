// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.ClaimsEnricher;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.UserInfo
{
    public class UserInfoController : Controller
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IScopeRepository _scopeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IClaimsEnricher _claimsEnricher;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;
        private readonly ILogger<UserInfoController> _logger;

        public UserInfoController(
            IJwtBuilder jwtBuilder,
            IScopeRepository scopeRepository,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            ITokenRepository tokenRepository,
            IClaimsEnricher claimsEnricher,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            ILogger<UserInfoController> logger)
        {
            _jwtBuilder = jwtBuilder;
            _scopeRepository = scopeRepository;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _claimsEnricher = claimsEnricher;
            _tokenRepository = tokenRepository;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
            _logger = logger;
        }

        [HttpGet]
        public Task<IActionResult> Get(CancellationToken token) => Common(null, token);

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken token)
        {
            try
            {
                var jObjBody = Request.Form?.ToJsonObject();
                return await Common(jObjBody, token);
            }
            catch (InvalidOperationException)
            {
                var jObj = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ErrorCodes.INVALID_REQUEST,
                    [ErrorResponseParameters.ErrorDescription] = ErrorMessages.CONTENT_TYPE_NOT_SUPPORTED
                };
                return new ContentResult
                {
                    Content = jObj.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }

        private async Task<IActionResult> Common(JsonObject content, CancellationToken cancellationToken)
        {
            try
            {
                var accessToken = ExtractAccessToken(content);
                var jwsPayload = Extract(accessToken);
                if (jwsPayload == null) throw new OAuthException(ErrorCodes.INVALID_TOKEN, ErrorMessages.BAD_TOKEN);

                var subject = jwsPayload.Subject;
                var scopes = jwsPayload.Claims.Where(c => c.Type == OpenIdConnectParameterNames.Scope).Select(c => c.Value);
                var audiences = jwsPayload.Audiences;
                var claims = GetClaims(jwsPayload);
                DateTime? authTime = null;
                if (jwsPayload.TryGetClaim(JwtRegisteredClaimNames.AuthTime, out Claim claim) && double.TryParse(claim.Value, out double a))
                    authTime = a.ConvertFromUnixTimestamp();

                var user = await _userRepository.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Id == subject, cancellationToken);
                if (user == null) return new UnauthorizedResult();

                var filteredClients = await _clientRepository.Query().AsNoTracking().Where(c => audiences.Contains(c.ClientId)).ToListAsync(cancellationToken);
                if (!filteredClients.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.INVALID_AUDIENCE);

                Client oauthClient = filteredClients.First();
                if (!user.HasOpenIDConsent(oauthClient.ClientId, scopes, claims, AuthorizationRequestClaimTypes.UserInfo))
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.NO_CONSENT);

                var token = await _tokenRepository.Query().AsNoTracking().FirstOrDefaultAsync(t => t.Id == accessToken);
                if (token == null)
                {
                    _logger.LogError("Cannot get user information because access token has been rejected");
                    throw new OAuthException(ErrorCodes.INVALID_TOKEN, ErrorMessages.ACCESS_TOKEN_REJECTED);
                }

                var oauthScopes = await _scopeRepository.Query().AsNoTracking().Where(s => scopes.Contains(s.Name)).ToListAsync(cancellationToken);
                var payload = new Dictionary<string, object>();
                IdTokenBuilder.EnrichWithScopeParameter(payload, oauthScopes, user, subject);
                _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(payload, claims, user, authTime, AuthorizationRequestClaimTypes.UserInfo);
                await _claimsEnricher.Enrich(user, payload, oauthClient, cancellationToken);
                string contentType = "application/json";
                var result = JsonSerializer.Serialize(payload);
                if (!string.IsNullOrWhiteSpace(oauthClient.UserInfoSignedResponseAlg))
                {
                    var securityTokenDescriptor = new SecurityTokenDescriptor
                    {
                        Claims = payload
                    };
                    securityTokenDescriptor.Issuer = Request.GetAbsoluteUriWithVirtualPath();
                    securityTokenDescriptor.Audience = token.ClientId;
                    result = await _jwtBuilder.BuildClientToken(oauthClient, securityTokenDescriptor, oauthClient.UserInfoSignedResponseAlg, oauthClient.UserInfoEncryptedResponseAlg, oauthClient.UserInfoEncryptedResponseEnc, cancellationToken);
                    contentType = "application/jwt";
                }

                return new ContentResult
                {
                    Content = result,
                    ContentType = contentType
                };
            }
            catch (OAuthException ex)
            {
                var jObj = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ex.Code,
                    [ErrorResponseParameters.ErrorDescription] = ex.Message
                };
                return new ContentResult
                {
                    Content = jObj.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            IEnumerable<AuthorizationRequestClaimParameter> GetClaims(JsonWebToken jsonWebToken)
            {
                if (jsonWebToken.TryGetClaim(AuthorizationRequestParameters.Claims, out Claim claim))
                {
                    var json = claim.Value.ToString();
                    return JsonSerializer.Deserialize<IEnumerable<AuthorizationRequestClaimParameter>>(json);
                }

                return new AuthorizationRequestClaimParameter[0];
            }
        }

        private JsonWebToken Extract(string accessToken)
        {
            var result = _jwtBuilder.ReadSelfIssuedJsonWebToken(accessToken);
            if (result.Error != null) return null;
            return result.Jwt;
        }

        private string ExtractAccessToken(JsonObject jsonObject)
        {
            if (Request.Headers.ContainsKey(Constants.AuthorizationHeaderName))
            {
                foreach (var authorizationValue in Request.Headers[Constants.AuthorizationHeaderName])
                {
                    var at = authorizationValue.ExtractAuthorizationValue(new string[] { AutenticationSchemes.Bearer });
                    if (!string.IsNullOrWhiteSpace(at))
                    {
                        return at;
                    }
                }
            }

            if (jsonObject != null && jsonObject.ContainsKey(TokenResponseParameters.AccessToken))
            {
                var at = jsonObject.GetStr(TokenResponseParameters.AccessToken);
                if (at != null && !string.IsNullOrWhiteSpace(at.ToString()))
                    return at.ToString();
            }

            throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_TOKEN);
        }
    }
}
