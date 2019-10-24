// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization;
using SimpleIdServer.OAuth.Extensions;
using System.Linq;

namespace SimpleIdServer.OpenID.Api.Authorization
{
    public class OpenIDAuthorizationRequestEnricher : IAuthorizationRequestEnricher
    {
        public void Enrich(HandlerContext context)
        {
            var uiLocales = context.Request.Data.GetUILocalesFromAuthorizationRequest();
            if (uiLocales.Any())
            {
                context.Response.Add(OAuth.DTOs.AuthorizationRequestParameters.UILocales, string.Join(" ", uiLocales));
            }
        }
    }
}
