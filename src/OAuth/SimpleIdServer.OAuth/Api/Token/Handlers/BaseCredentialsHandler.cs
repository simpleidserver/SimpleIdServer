// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public abstract class BaseCredentialsHandler : IGrantTypeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;

        public BaseCredentialsHandler(IClientAuthenticationHelper clientAuthenticationHelper)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
        }

        public abstract string GrantType { get; }
        public abstract Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);

        protected async Task<OAuthClient> AuthenticateClient(HandlerContext context, CancellationToken cancellationToken)
        {
            var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Request.HttpHeader, context.Request.Data, context.Request.IssuerName, cancellationToken);
            if (oauthClient.GrantTypes == null || !oauthClient.GrantTypes.Contains(GrantType))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.BAD_CLIENT_GRANT_TYPE, GrantType));
            }

            return oauthClient;
        }

        protected JObject BuildResult(HandlerContext context, IEnumerable<string> scopes)
        {
            return new JObject
            {
                { TokenResponseParameters.ExpiresIn, context.Client.TokenExpirationTimeInSeconds },
                { TokenResponseParameters.Scope, new JArray(scopes) }
            };
        }

        public static IActionResult BuildError(HttpStatusCode httpStatusCode, string error, string errorMessage)
        {
            var jObj = new JObject
            {
                { ErrorResponseParameters.Error, error },
                { ErrorResponseParameters.ErrorDescription, errorMessage}
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