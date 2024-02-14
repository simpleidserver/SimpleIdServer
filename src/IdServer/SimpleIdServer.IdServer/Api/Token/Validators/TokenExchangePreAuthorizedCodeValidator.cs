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

namespace SimpleIdServer.IdServer.Api.Token.Validators;

public interface ITokenExchangePreAuthorizedCodeValidator
{
    Task<User> Validate(HandlerContext context, CancellationToken cancellationToken);
}

public class TokenExchangePreAuthorizedCodeValidator : ITokenExchangePreAuthorizedCodeValidator
{
    private readonly IEnumerable<ITokenTypeService> _tokenTypeParsers;
    private readonly IAuthenticationHelper _authenticationHelper;

    public TokenExchangePreAuthorizedCodeValidator(
        IEnumerable<ITokenTypeService> tokenTypeParsers,
        IAuthenticationHelper authenticationHelper)
    {
        _tokenTypeParsers = tokenTypeParsers;
        _authenticationHelper = authenticationHelper;
    }

    public async Task<User> Validate(HandlerContext context, CancellationToken cancellationToken)
    {
        var subjectToken = context.Request.RequestData.GetSubjectToken();
        var subjectTokenType = context.Request.RequestData.GetSubjectTokenType();
        var scopes = context.Request.RequestData.GetScopes();
        if (string.IsNullOrWhiteSpace(subjectToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.SubjectToken));
        if (string.IsNullOrWhiteSpace(subjectTokenType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.SubjectTokenType));
        if (scopes == null || !scopes.Any()) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.Scope));
        var tokenTypeParser = _tokenTypeParsers.SingleOrDefault(t => t.Name == subjectTokenType);
        if (tokenTypeParser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_TOKENTYPE, subjectTokenType));
        var tokenResult = tokenTypeParser.Parse(context.Realm, subjectToken);
        if (!tokenResult.Claims.ContainsKey("sub")) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_SUBJECT_SUBJECTTOKEN);
        var existingScopes = context.Client.Scopes;
        var unknownScopes = scopes.Where(s => !existingScopes.Any(sc => sc.Name == s));
        if (unknownScopes.Any())
            throw new OAuthException(ErrorCodes.INVALID_SCOPE, string.Format(Global.UnknownScope, string.Join(",", unknownScopes)));
        var sub = tokenResult.Claims["sub"].ToString();
        var user = await _authenticationHelper.GetUserByLogin(sub, context.Realm, cancellationToken);
        if (context.Client.IsTransactionCodeRequired)
        {
            var otp = user.ActiveOTP;
            if (otp == null) throw new OAuthException(ErrorCodes.NO_ACTIVE_OTP, ErrorMessages.NO_ACTIVE_OTP);
        }

        return user;
    }
}