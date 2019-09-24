// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseModes
{
    public class ResponseModeHandler : IResponseModeHandler
    {
        private static Dictionary<IEnumerable<string>, string> MAPPING_RESPONSETYPES_TO_RESPONSEMODE = new Dictionary<IEnumerable<string>, string>
        {
            { new [] { "code" }, "query" }
        };
        private readonly IEnumerable<IOAuthResponseModeHandler> _oauthResponseModeHandlers;

        public ResponseModeHandler(IEnumerable<IOAuthResponseModeHandler> oauthResponseModeHandlers)
        {
            _oauthResponseModeHandlers = oauthResponseModeHandlers;
        }

        public void Handle(JObject queryParams, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var responseTypes = queryParams.GetResponseTypesFromAuthorizationRequest();
            var responseMode = queryParams.GetResponseModeFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(responseMode))
            {
                var kvp = MAPPING_RESPONSETYPES_TO_RESPONSEMODE.FirstOrDefault(r => r.Key.All(k => responseTypes.Contains(k)));
                responseMode = kvp.Value;
            }

            _oauthResponseModeHandlers.First(o => o.ResponseMode == responseMode).Handle(authorizationResponse, httpContext);
        }
    }
}