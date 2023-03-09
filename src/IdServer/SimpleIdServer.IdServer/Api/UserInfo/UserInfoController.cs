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
        private readonly IClaimsExtractor _claimsExtractor;
        private readonly ILogger<UserInfoController> _logger;

        public UserInfoController(
            IJwtBuilder jwtBuilder,
            IScopeRepository scopeRepository,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            ITokenRepository tokenRepository,
            IClaimsEnricher claimsEnricher,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            IClaimsExtractor claimsExtractor,
            ILogger<UserInfoController> logger)
        {
            _jwtBuilder = jwtBuilder;
            _scopeRepository = scopeRepository;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _claimsEnricher = claimsEnricher;
            _tokenRepository = tokenRepository;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
            _claimsExtractor = claimsExtractor;
            _logger = logger;
        }

        [HttpGet]
        public Task<IActionResult> Get([FromRoute] string prefix, CancellationToken token) => Common(prefix, null, token);

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] string prefix, CancellationToken token)
        {
            try
            {
                var jObjBody = Request.Form?.ToJsonObject();
                return await Common(prefix,jObjBody, token);
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

        private async Task<IActionResult> Common(string prefix, JsonObject content, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var accessToken = ExtractAccessToken(content);
                var jwsPayload = Extract(prefix, accessToken);
                if (jwsPayload == null) throw new OAuthException(ErrorCodes.INVALID_TOKEN, ErrorMessages.BAD_TOKEN);

                var subject = jwsPayload.Subject;
                var scopes = jwsPayload.Claims.Where(c => c.Type == OpenIdConnectParameterNames.Scope).Select(c => c.Value);
                var audiences = jwsPayload.Audiences;
                var clientId = jwsPayload.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                var claims = GetClaims(jwsPayload);
                DateTime? authTime = null;
                if (jwsPayload.TryGetClaim(JwtRegisteredClaimNames.AuthTime, out Claim claim) && double.TryParse(claim.Value, out double a))
                    authTime = a.ConvertFromUnixTimestamp();

                var user = await _userRepository.Query().Include(u => u.Consents).Include(u => u.OAuthUserClaims).AsNoTracking().FirstOrDefaultAsync(u => u.Name == subject, cancellationToken);
                if (user == null) return new UnauthorizedResult();

                var oauthClient = await _clientRepository.Query().Include(c => c.Realms).Include(c => c.SerializedJsonWebKeys).AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
                if (oauthClient == null)
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));

                if (!oauthClient.IsConsentDisabled && user.GetConsent(oauthClient.ClientId, scopes, claims, AuthorizationClaimTypes.UserInfo) == null)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.NO_CONSENT);

                var token = await _tokenRepository.Query().AsNoTracking().FirstOrDefaultAsync(t => t.Id == accessToken);
                if (token == null)
                {
                    _logger.LogError("Cannot get user information because access token has been rejected");
                    throw new OAuthException(ErrorCodes.INVALID_TOKEN, ErrorMessages.ACCESS_TOKEN_REJECTED);
                }

                var oauthScopes = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ClaimMappers).AsNoTracking().Where(s => scopes.Contains(s.Name) && s.Realms.Any(r => r.Name == prefix)).ToListAsync(cancellationToken);
                var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, null, null, null, null), prefix ?? Constants.DefaultRealm);
                context.SetUser(user);
                var payload = await _claimsExtractor.ExtractClaims(new ClaimsExtractionParameter
                {
                    Protocol = ScopeProtocols.OPENID,
                    Context = context,
                    Scopes = oauthScopes
                });
                _claimsJwsPayloadEnricher.EnrichWithClaimsParameter(payload, claims, user, authTime, AuthorizationClaimTypes.UserInfo);
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
                    result = await _jwtBuilder.BuildClientToken(prefix, oauthClient, securityTokenDescriptor, oauthClient.UserInfoSignedResponseAlg, oauthClient.UserInfoEncryptedResponseAlg, oauthClient.UserInfoEncryptedResponseEnc, cancellationToken);
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

            IEnumerable<AuthorizedClaim> GetClaims(JsonWebToken jsonWebToken)
            {
                if (jsonWebToken.TryGetClaim(AuthorizationRequestParameters.Claims, out Claim claim))
                {
                    var json = JsonObject.Parse(claim.Value.ToString()) as JsonObject;
                    return json.GetOpenIdClaims();
                }

                return new AuthorizedClaim[0];
            }
        }

        private JsonWebToken Extract(string realm, string accessToken)
        {
            var result = _jwtBuilder.ReadSelfIssuedJsonWebToken(realm, accessToken);
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
