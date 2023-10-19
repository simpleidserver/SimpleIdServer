// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
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
    Task<TokenExchangeValidationResult> Validate(HandlerContext context, CancellationToken cancellationToken);
}

public class TokenExchangeValidationResult
{
    public TokenExchangeValidationResult(Dictionary<string, string> claims, IEnumerable<string> scopes, IEnumerable<string> audiences, string tokenType)
    {
        Claims = claims;
        Scopes = scopes;
        Audiences = audiences;
        TokenType = tokenType;
    }

    public Dictionary<string, string> Claims { get; set; }
    public IEnumerable<string> Scopes { get; set; }
    public IEnumerable<string> Audiences { get; set; }
    public string TokenType { get; set; }
}

public class TokenExchangeValidator : ITokenExchangeValidator
{
    private readonly IEnumerable<ITokenTypeParser> _tokenTypeParsers;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IScopeRepository _scopeRepository;

    public TokenExchangeValidator(IEnumerable<ITokenTypeParser> tokenTypeParsers, IApiResourceRepository apiResourceRepository, IScopeRepository scopeRepository)
    {
        _tokenTypeParsers = tokenTypeParsers;
        _apiResourceRepository = apiResourceRepository;
        _scopeRepository = scopeRepository;
    }

    public async Task<TokenExchangeValidationResult> Validate(HandlerContext context, CancellationToken cancellationToken)
    {
        var realm = context.Realm;
        if (!context.Client.IsTokenExchangeEnabled) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_TOKENEXCHANGE_NOT_ENABLED);
        // https://datatracker.ietf.org/doc/html/rfc8693#TokenTypeIdentifiers
        var subjectToken = context.Request.RequestData.GetSubjectToken();
        if (string.IsNullOrWhiteSpace(subjectToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.SubjectToken));
        var subjectTokenType = context.Request.RequestData.GetSubjectTokenType();
        if (string.IsNullOrWhiteSpace(subjectTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.SubjectTokenType));
        var tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == subjectTokenType);
        if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_TOKENTYPE, subjectTokenType));
        var requestedTokenType = context.Request.RequestData.GetRequestedTokenType();
        if (!string.IsNullOrWhiteSpace(requestedTokenType) && !_tokenTypeParsers.Any(p => p.Name == requestedTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_REQUESTED_TOKEN_TYPE, requestedTokenType));
        var claims = tokenTypeParser.Parse(context.Realm, subjectToken);
        var actorToken = context.Request.RequestData.GetActorToken();
        var actorTokenType = context.Request.RequestData.GetActorTokenType();
        if(!string.IsNullOrWhiteSpace(actorTokenType))
        {
            tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == actorTokenType);
            if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_ACTORTYPE, actorTokenType));
            if (!string.IsNullOrWhiteSpace(actorToken)) tokenTypeParser.Parse(context.Realm, actorToken);
        }

        var scopes = context.Request.RequestData.GetScopes();
        var resources = context.Request.RequestData.GetResources();
        var audiences = context.Request.RequestData.GetAudiences();
        var allResources = await _apiResourceRepository.Query().Include(r => r.Realms).Include(r => r.Scopes).Where(r => resources.Contains(r.Name) || audiences.Contains(r.Audience)).ToListAsync(cancellationToken);
        var unknownResources = resources.Where(r => !allResources.Any(re => re.Name == r));
        var unknownAudiences = audiences.Where(a => !allResources.Any(re => re.Audience == a));
        if (unknownResources.Any()) throw new OAuthException(ErrorCodes.INVALID_TARGET, string.Format(ErrorMessages.UNKNOWN_RESOURCE, string.Format(",", unknownResources)));
        if (unknownAudiences.Any()) throw new OAuthException(ErrorCodes.INVALID_TARGET, string.Format(ErrorMessages.UNKNOWN_AUDIENCE, string.Format(",", unknownAudiences)));
        if (allResources.Any())
        {
            var unknownScopes = scopes.Where(s => allResources.SelectMany(r => r.Scopes).Any(sc => sc.Name == s));
            if (unknownScopes.Any()) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(string.Format(ErrorMessages.UNKNOWN_SCOPE_RESOURCE_OR_AUDIENCE, string.Join(",", unknownScopes))));
        }
        else if(scopes.Any())
        {
            var unknownScopes = await _scopeRepository.Query().Where(s => !scopes.Contains(s.Name)).ToListAsync(cancellationToken);
            if (unknownScopes.Any()) throw new OAuthException(ErrorCodes.INVALID_SCOPE, string.Format(ErrorMessages.UNKNOWN_SCOPE, string.Format(string.Join(",", unknownScopes))));
        }

        return new TokenExchangeValidationResult(claims, scopes, audiences, requestedTokenType ?? subjectTokenType);
    }
}
