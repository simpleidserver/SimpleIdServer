// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.PKCECodeChallengeMethods;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseTypes
{
    public class AuthorizationCodeResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ICodeChallengeMethodHandler> _codeChallengeMethodHandlers;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IdServerHostOptions _options;

        public AuthorizationCodeResponseTypeHandler(IGrantedTokenHelper grantedTokenHelper, IEnumerable<ICodeChallengeMethodHandler> codeChallengeMethodHandlers, IEnumerable<ITokenBuilder> tokenBuilders, IOptions<IdServerHostOptions> options)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _codeChallengeMethodHandlers = codeChallengeMethodHandlers;
            _tokenBuilders = tokenBuilders;
            _options = options.Value;
        }

        public string GrantType => AuthorizationCodeHandler.GRANT_TYPE;
        public string ResponseType => RESPONSE_TYPE;
        public int Order => 1;
        public static string RESPONSE_TYPE = "code";

        public async Task Enrich(EnrichParameter parameter, HandlerContext context, CancellationToken cancellationToken)
        {
            var activeSession = context.Session;
            var dic = new JsonObject
            {
                [JwtRegisteredClaimNames.Sub] = context.User.Name
            };
            if (activeSession != null)
                dic.Add(JwtRegisteredClaimNames.AuthTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());

            foreach (var record in context.Request.RequestData)
            {
                if (record.Value is JsonValue)
                    dic.Add(record.Key, QueryCollectionExtensions.GetValue(record.Value.GetValue<object>().ToString()));
                else
                    dic.Add(record.Key, QueryCollectionExtensions.GetValue(record.Value.ToJsonString()));
            }

            CheckPKCEParameters(context);
            var dpopJkt = context.Request.RequestData.GetDPOPJktFromAuthorizationRequest();
            var authCode = await _grantedTokenHelper.AddAuthorizationCode(dic, parameter.GrantId, _options.AuthorizationCodeExpirationInSeconds, dpopJkt, context.Session?.SessionId, cancellationToken);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
        }

        protected virtual void CheckPKCEParameters(HandlerContext handlerContext)
        {
            var codeChallenge = handlerContext.Request.RequestData.GetCodeChallengeFromAuthorizationRequest();
            var codeChallengeMethod = handlerContext.Request.RequestData.GetCodeChallengeMethodFromAuthorizationRequest();
            if (handlerContext.Client.TokenEndPointAuthMethod == OAuthPKCEAuthenticationHandler.AUTH_METHOD && string.IsNullOrWhiteSpace(codeChallenge))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.CodeChallenge));

            if (string.IsNullOrWhiteSpace(codeChallengeMethod))
                codeChallengeMethod = _options.DefaultCodeChallengeMethod;

            if (!_codeChallengeMethodHandlers.Any(c => c.Name == codeChallengeMethod))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.BadCodeChallengeMethod, codeChallengeMethod));
        }
    }
}