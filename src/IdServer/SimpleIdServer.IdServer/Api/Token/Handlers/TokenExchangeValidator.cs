// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
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
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IGrantHelper _grantHelper;
    private readonly IScopeRepository _scopeRepository;

    public TokenExchangeValidator(IEnumerable<ITokenTypeService> tokenTypeParsers, IApiResourceRepository apiResourceRepository, IGrantHelper grantHelper, IScopeRepository scopeRepository)
    {
        _tokenTypeParsers = tokenTypeParsers;
        _apiResourceRepository = apiResourceRepository;
        _grantHelper = grantHelper;
        _scopeRepository = scopeRepository;
    }

    public async Task<TokenExchangeValidationResult> Validate(string realm, HandlerContext context, CancellationToken cancellationToken)
    {
        if (!context.Client.IsTokenExchangeEnabled) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_TOKENEXCHANGE_NOT_ENABLED);
        var subjectToken = context.Request.RequestData.GetSubjectToken();
        if (string.IsNullOrWhiteSpace(subjectToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.SubjectToken));
        var subjectTokenType = context.Request.RequestData.GetSubjectTokenType();
        if (string.IsNullOrWhiteSpace(subjectTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.SubjectTokenType));
        var tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == subjectTokenType);
        if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_TOKENTYPE, subjectTokenType));
        var requestedTokenType = context.Request.RequestData.GetRequestedTokenType();
        if (!string.IsNullOrWhiteSpace(requestedTokenType) && !_tokenTypeParsers.Any(p => p.Name == requestedTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_REQUESTED_TOKEN_TYPE, requestedTokenType));
        var subject = tokenTypeParser.Parse(context.Realm, subjectToken);
        var actorToken = context.Request.RequestData.GetActorToken();
        var actorTokenType = context.Request.RequestData.GetActorTokenType();
        TokenResult actor = null;
        if(!string.IsNullOrWhiteSpace(actorTokenType))
        {
            tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == actorTokenType);
            if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_ACTORTYPE, actorTokenType));
            if (!string.IsNullOrWhiteSpace(actorToken)) tokenTypeParser.Parse(context.Realm, actorToken);
        }


        var scopes = context.Request.RequestData.GetScopes();
        var resources = context.Request.RequestData.GetResources();
        var audiences = context.Request.RequestData.GetAudiences();
        var grantRequest = await _grantHelper.Extract(realm, scopes, resources, audiences, new List<AuthorizationData>(), cancellationToken);
        return new TokenExchangeValidationResult(subject, actor, grantRequest, requestedTokenType ?? subjectTokenType);
    }
}
