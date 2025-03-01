// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Authenticate;

public interface IPkceVerifier
{
    Task<bool> Validate(HandlerContext context, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT);
}

public class PkceVerifier : IPkceVerifier
{
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly IEnumerable<ICodeChallengeMethodHandler> _codeChallengeMethodHandlers;
    private readonly IdServerHostOptions _options;

    public PkceVerifier(IGrantedTokenHelper grantedTokenHelper, IEnumerable<ICodeChallengeMethodHandler> codeChallengeMethodHandlers, IOptions<IdServerHostOptions> options)
    {
        _grantedTokenHelper = grantedTokenHelper;
        _codeChallengeMethodHandlers = codeChallengeMethodHandlers;
        _options = options.Value;
    }

    public async Task<bool> Validate(HandlerContext context, Client client, string expectedIssuer, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_CLIENT)
    {
        var codeVerifier = context.Request.RequestData.GetCodeVerifier();
        if (string.IsNullOrWhiteSpace(codeVerifier)) throw new OAuthException(errorCode, string.Format(Global.MissingParameter, TokenRequestParameters.CodeVerifier));

        var code = context.Request.RequestData.GetAuthorizationCode();
        if (code == null) return false;

        var authCode = await _grantedTokenHelper.GetAuthorizationCode(code, cancellationToken);
        var previousRequest = authCode?.OriginalRequest;
        if (previousRequest == null) return false;

        var codeChallenge = previousRequest.GetCodeChallengeFromAuthorizationRequest();
        var codeChallengeMethod = previousRequest.GetCodeChallengeMethodFromAuthorizationRequest();
        if (string.IsNullOrWhiteSpace(codeChallengeMethod)) codeChallengeMethod = _options.DefaultCodeChallengeMethod;

        var codeChallengeMethodHandler = _codeChallengeMethodHandlers.First(c => c.Name == codeChallengeMethod);
        var newCodeChallenge = codeChallengeMethodHandler.Calculate(codeVerifier);
        if (newCodeChallenge != codeChallenge) throw new OAuthException(errorCode, Global.BadCodeVerifier);
        return true;
    }
}