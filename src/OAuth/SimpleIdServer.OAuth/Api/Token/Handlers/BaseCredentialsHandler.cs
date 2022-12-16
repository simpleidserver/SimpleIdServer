// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public abstract class BaseCredentialsHandler : IGrantTypeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly OAuthHostOptions _options;

        public BaseCredentialsHandler(IClientAuthenticationHelper clientAuthenticationHelper, IOptions<OAuthHostOptions> options)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _options = options.Value;
        }

        public abstract string GrantType { get; }
        public abstract Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);

        protected async Task<Client> AuthenticateClient(HandlerContext context, CancellationToken cancellationToken)
        {
            var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.Request.IssuerName, cancellationToken);
            if (oauthClient.GrantTypes == null || !oauthClient.GrantTypes.Contains(GrantType)) throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.BAD_CLIENT_GRANT_TYPE, GrantType));
            return oauthClient;
        }

        protected JsonObject BuildResult(HandlerContext context, IEnumerable<string> scopes)
        {
            return new JsonObject
            {
                [TokenResponseParameters.ExpiresIn] = context.Client.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds,
                [TokenResponseParameters.Scope] = string.Join(" ", scopes)
            };
        }

        public static IActionResult BuildError(HttpStatusCode httpStatusCode, string error, string errorMessage)
        {
            var jObj = new JsonObject
            {
                [ErrorResponseParameters.Error] = error,
                [ErrorResponseParameters.ErrorDescription] = errorMessage
            };
            return new ContentResult
            {
                ContentType = "application/json",
                Content = jObj.ToString(),
                StatusCode = (int)httpStatusCode
            };
        }
    }
}