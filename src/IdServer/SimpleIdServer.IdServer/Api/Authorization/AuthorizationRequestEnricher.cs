// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Authorization
{
    public interface IAuthorizationRequestEnricher
    {
        void Enrich(HandlerContext context);
    }

    public class AuthorizationRequestEnricher : IAuthorizationRequestEnricher
    {
        public void Enrich(HandlerContext context)
        {
            var uiLocales = context.Request.RequestData.GetUILocalesFromAuthorizationRequest();
            if (uiLocales.Any())
                context.Response.Add(AuthorizationRequestParameters.UILocales, string.Join(" ", uiLocales));
        }
    }
}
