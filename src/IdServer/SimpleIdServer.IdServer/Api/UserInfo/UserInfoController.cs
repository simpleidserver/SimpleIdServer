// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.ClaimsEnricher;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Extractors;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.UserInfo
{
    public class UserInfoController : BaseController
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IUserHelper _userHelper;
        private readonly IScopeRepository _scopeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IClaimsEnricher _claimsEnricher;
        private readonly IClaimsJwsPayloadEnricher _claimsJwsPayloadEnricher;
        private readonly IScopeClaimsExtractor _claimsExtractor;
        private readonly IdServerHostOptions _options;
        private readonly ILogger<UserInfoController> _logger;
        private readonly IBusControl _busControl;

        public UserInfoController(
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IUserHelper userHelper,
            IScopeRepository scopeRepository,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IClaimsEnricher claimsEnricher,
            IClaimsJwsPayloadEnricher claimsJwsPayloadEnricher,
            IScopeClaimsExtractor claimsExtractor,
            IOptions<IdServerHostOptions> options,
            ILogger<UserInfoController> logger,
            IBusControl busControl) : base(tokenRepository, jwtBuilder)
        {
            _jwtBuilder = jwtBuilder;
            _userHelper = userHelper;
            _scopeRepository = scopeRepository;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _claimsEnricher = claimsEnricher;
            _claimsJwsPayloadEnricher = claimsJwsPayloadEnricher;
            _claimsExtractor = claimsExtractor;
            _options = options.Value;
            _logger = logger;
            _busControl = busControl;
        }

        [HttpGet]
        public Task<IActionResult> Get([FromRoute] string prefix, CancellationToken token) => Common(prefix, null, token);

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] string prefix, CancellationToken token)
        {
            try
            {
                var jObjBody = Request.Form?.ToJsonObject();
                return await Common(prefix, jObjBody, token);
            }
            catch (InvalidOperationException)
            {
                var jObj = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ErrorCodes.INVALID_REQUEST,
                    [ErrorResponseParameters.ErrorDescription] = Global.ContentTypeNotSupported
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
            string clientId = null;
            IEnumerable<string> scopes = new string[0];
            Domains.User user = null;
            using (var activity = Tracing.IdserverActivitySource.StartActivity("GetUserInfo"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    activity?.SetTag(Tracing.CommonTagNames.Realm, prefix);
                    var accessToken = string.Empty;
                    if(content != null)
                    {
                        if (content.ContainsKey(TokenResponseParameters.AccessToken))
                        {
                            var at = content.GetStr(TokenResponseParameters.AccessToken);
                            if (at != null && !string.IsNullOrWhiteSpace(at.ToString())) accessToken = at.ToString();
                        }

                        if (string.IsNullOrWhiteSpace(accessToken)) throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, Global.MissingToken);
                    }

                    var kvp = await CheckAccessToken(prefix, accessToken: accessToken);
                    var jwsPayload = kvp.Item1;
                    var token = kvp.Item2;
                    var subject = jwsPayload.Subject;
                    scopes = jwsPayload.Claims.Where(c => c.Type == OpenIdConnectParameterNames.Scope).Select(c => c.Value);
                    var audiences = jwsPayload.Audiences;
                    clientId = jwsPayload.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                    var claims = GetClaims(jwsPayload);
                    DateTime? authTime = null;
                    if (jwsPayload.TryGetClaim(JwtRegisteredClaimNames.AuthTime, out Claim claim) && double.TryParse(claim.Value, out double a))
                        authTime = a.ConvertFromUnixTimestamp();

                    user = await _userRepository.GetBySubject(subject, prefix, cancellationToken);
                    if (user == null) return new UnauthorizedResult();
                    var oauthClient = await _clientRepository.GetByClientId(prefix, clientId, cancellationToken);
                    if (oauthClient == null)
                        throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(Global.UnknownClient, clientId));

                    if (!oauthClient.IsConsentDisabled && _userHelper.GetConsent(user, prefix, oauthClient.ClientId, scopes, claims, null, AuthorizationClaimTypes.UserInfo) == null)
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.NoConsent);

                    var oauthScopes = await _scopeRepository.GetByNames(prefix, scopes.ToList(), cancellationToken);
                    var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, null, null, null, (X509Certificate2)null, HttpContext.Request.Method), prefix ?? Constants.DefaultRealm, _options);
                    context.SetUser(user, null);
                    var payload = await _claimsExtractor.ExtractClaims(context, oauthScopes, ScopeProtocols.OPENID);
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
                        securityTokenDescriptor.Issuer = context.GetIssuer();
                        securityTokenDescriptor.Audience = token.ClientId;
                        result = await _jwtBuilder.BuildClientToken(prefix, oauthClient, securityTokenDescriptor, oauthClient.UserInfoSignedResponseAlg, oauthClient.UserInfoEncryptedResponseAlg, oauthClient.UserInfoEncryptedResponseEnc, cancellationToken);
                        contentType = "application/jwt";
                    }

                    await _busControl.Publish(new UserInfoSuccessEvent
                    {
                        ClientId = oauthClient.ClientId,
                        Realm = prefix,
                        Scopes = scopes,
                        UserName = user.Name
                    });
                    activity?.SetStatus(ActivityStatusCode.Ok, "User Info has been returned");
                    return new ContentResult
                    {
                        Content = result,
                        ContentType = contentType
                    };
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.Message);
                    await _busControl.Publish(new UserInfoFailureEvent
                    {
                        ClientId = clientId,
                        Realm = prefix,
                        Scopes = scopes,
                        ErrorMessage = ex.Message,
                        UserName = user?.Name
                    });
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
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
    }
}
