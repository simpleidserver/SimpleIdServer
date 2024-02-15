// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.TokenTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

public interface ITokenExchangeValidator
{
    Task<TokenExchangeValidationResult> Validate(string realm, HandlerContext context, CancellationToken cancellationToken);
}

public class TokenExchangeValidationResult
{
    public TokenExchangeValidationResult(TokenResult subject, TokenResult actor, GrantRequest grantRequest, string tokenType)
    {
        Subject = subject;
        Actor = actor;
        GrantRequest = grantRequest;
        TokenType = tokenType;
    }

    public TokenResult Subject { get; set; }
    public TokenResult Actor { get; set; }
    public GrantRequest GrantRequest{ get; set; }
    public string TokenType { get; set; }
}

public class TokenExchangeValidator : ITokenExchangeValidator
{
    private readonly IEnumerable<ITokenTypeService> _tokenTypeParsers;
    private readonly IGrantHelper _grantHelper;

    public TokenExchangeValidator(IEnumerable<ITokenTypeService> tokenTypeParsers, IGrantHelper grantHelper)
    {
        _tokenTypeParsers = tokenTypeParsers;
        _grantHelper = grantHelper;
    }

    public async Task<TokenExchangeValidationResult> Validate(string realm, HandlerContext context, CancellationToken cancellationToken)
    {
        if (!context.Client.IsTokenExchangeEnabled) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.ClientTokenExchangedNotEnabled);
        var subjectToken = context.Request.RequestData.GetSubjectToken();
        if (string.IsNullOrWhiteSpace(subjectToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.SubjectToken));
        var subjectTokenType = context.Request.RequestData.GetSubjectTokenType();
        if (string.IsNullOrWhiteSpace(subjectTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.SubjectTokenType));
        var tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == subjectTokenType);
        if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnsupportedTokenType, subjectTokenType));
        var requestedTokenType = context.Request.RequestData.GetRequestedTokenType();
        if (!string.IsNullOrWhiteSpace(requestedTokenType) && !_tokenTypeParsers.Any(p => p.Name == requestedTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnsupportedRequestedTokenType, requestedTokenType));
        var tokenResult = tokenTypeParser.Parse(context.Realm, subjectToken);
        if (string.IsNullOrWhiteSpace(tokenResult.Subject)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.MissingSubSubjectToken);
        var actorToken = context.Request.RequestData.GetActorToken();
        var actorTokenType = context.Request.RequestData.GetActorTokenType();
        TokenResult actor = null;
        if(!string.IsNullOrWhiteSpace(actorTokenType))
        {
            tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == actorTokenType);
            if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnsupportedActorType, actorTokenType));
            if (!string.IsNullOrWhiteSpace(actorToken)) tokenTypeParser.Parse(context.Realm, actorToken);
        }

        var scopes = context.Request.RequestData.GetScopes();
        var resources = context.Request.RequestData.GetResources();
        var audiences = context.Request.RequestData.GetAudiences();
        var grantRequest = await _grantHelper.Extract(realm, scopes, resources, audiences, new List<AuthorizationData>(), cancellationToken);
        return new TokenExchangeValidationResult(tokenResult, actor, grantRequest, requestedTokenType ?? subjectTokenType);
    }
}
