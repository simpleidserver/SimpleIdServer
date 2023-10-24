// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Domains.Extensions;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.TokenTypes;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

/// <summary>
/// https://datatracker.ietf.org/doc/html/rfc8693#TokenTypeIdentifiers
/// </summary>
public class TokenExchangeHandler : BaseCredentialsHandler
{
    private readonly ITokenExchangeValidator _tokenExchangeValidator;
    private readonly IBusControl _busControl;
    private readonly IEnumerable<ITokenTypeService> _tokenTypes;

    public TokenExchangeHandler(ITokenExchangeValidator tokenExchangeValidator, IBusControl busControl, IEnumerable<ITokenTypeService> tokenTypes, IClientAuthenticationHelper clientAuthenticationHelper, IEnumerable<ITokenProfile> tokenProfiles, IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, tokenProfiles, options)
    {
        _tokenExchangeValidator = tokenExchangeValidator;
        _busControl = busControl;
        _tokenTypes = tokenTypes;
    }

    public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:token-exchange";
    public override string GrantType => GRANT_TYPE;

    public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<string> scopeLst = new string[0];
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
        {
            try
            {
                var realm = context.Realm ?? Constants.DefaultRealm;
                activity?.SetTag("grant_type", GRANT_TYPE);
                activity?.SetTag("realm", realm);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var validationResult = await _tokenExchangeValidator.Validate(realm, context, cancellationToken);
                scopeLst = validationResult.GrantRequest.Scopes;
                activity?.SetTag("scopes", string.Join(",", validationResult.GrantRequest.Scopes));
                var result = BuildResult(context, validationResult.GrantRequest.Scopes);
                var claims = BuildClaims(validationResult, context);
                var tokenType = _tokenTypes.Single(t => t.Name == validationResult.TokenType);
                var token = await tokenType.Build(realm, context.GetIssuer(), context.Client, claims, cancellationToken);
                result.Add(TokenResponseParameters.AccessToken, token);
                result.Add(TokenResponseParameters.IssuedTokenType, validationResult.TokenType);
                result.Add(TokenResponseParameters.TokenType, tokenType.TokenType);
                await _busControl.Publish(new TokenIssuedSuccessEvent
                {
                    GrantType = GRANT_TYPE,
                    ClientId = context.Client.ClientId,
                    Scopes = validationResult.GrantRequest.Scopes,
                    Realm = context.Realm
                });
                activity?.SetStatus(ActivityStatusCode.Ok, "Token has been issued");
                return new OkObjectResult(result);
            }
            catch (OAuthUnauthorizedException ex)
            {
                await _busControl.Publish(new TokenIssuedFailureEvent
                {
                    GrantType = GRANT_TYPE,
                    ClientId = context.Client?.ClientId,
                    Scopes = scopeLst,
                    Realm = context.Realm,
                    ErrorMessage = ex.Message
                });
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                await _busControl.Publish(new TokenIssuedFailureEvent
                {
                    GrantType = GRANT_TYPE,
                    ClientId = context.Client?.ClientId,
                    Scopes = scopeLst,
                    Realm = context.Realm,
                    ErrorMessage = ex.Message
                });
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }

    private Dictionary<string, object> BuildClaims(TokenExchangeValidationResult validationResult, HandlerContext context)
    {
        var claims = validationResult.Subject.Claims;
        var expiresIn = context.Client.TokenExpirationTimeInSeconds ?? Options.DefaultTokenExpirationTimeInSeconds;
        var scopes = validationResult.GrantRequest.Scopes;
        if (!claims.ContainsKey(TokenResponseParameters.ExpiresIn)) claims.Add(TokenResponseParameters.ExpiresIn, expiresIn);
        else claims[TokenResponseParameters.ExpiresIn] = expiresIn;
        if (scopes != null && scopes.Any() && !claims.ContainsKey(TokenResponseParameters.Scope)) claims.Add(TokenResponseParameters.Scope, string.Join(" ", scopes));
        else if (scopes != null && scopes.Any() && claims.ContainsKey(TokenResponseParameters.Scope)) claims[TokenResponseParameters.Scope] = string.Join(" ", scopes);
        else if ((scopes == null || !scopes.Any()) && claims.ContainsKey(TokenResponseParameters.Scope)) claims.Remove(TokenResponseParameters.Scope);
        if(context.Client.TokenExchangeType == Domains.TokenExchangeTypes.DELEGATION)
        {
            var subject = validationResult.Actor?.Subject ?? context.Client.ClientId;
            object oldAct = null;
            if (claims.ContainsKey(AdditionalJsonWebKeyParameterNames.Act)) oldAct = JsonNode.Parse(claims[AdditionalJsonWebKeyParameterNames.Act].ToString()).SerializeJson();
            var newAct = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, subject }
            };
            if (oldAct != null) newAct.Add(AdditionalJsonWebKeyParameterNames.Act, oldAct);
            if (claims.ContainsKey(AdditionalJsonWebKeyParameterNames.Act)) claims[AdditionalJsonWebKeyParameterNames.Act] = newAct;
            else claims.Add(AdditionalJsonWebKeyParameterNames.Act, newAct);
        }

        return claims;
    }
}
