// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseModes
{
    public class ResponseModeHandler : IResponseModeHandler
    {
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
                // https://openid.net/specs/openid-connect-core-1_0.html#rfc.section.3.1.2.6
                if (!responseTypes.Any() || (responseTypes.Count() == 1 && responseTypes.Contains(AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE)))
                {
                    responseMode = QueryResponseModeHandler.NAME;
                }
                else
                {
                    responseMode = FragmentResponseModeHandler.NAME;
                }
            }

            var oauthResponseModeHandler = _oauthResponseModeHandlers.FirstOrDefault(o => o.ResponseMode == responseMode);
            if (oauthResponseModeHandler == null)
            {
                oauthResponseModeHandler = _oauthResponseModeHandlers.First(o => o.ResponseMode == QueryResponseModeHandler.NAME);
            }

            oauthResponseModeHandler.Handle(authorizationResponse, httpContext);
        }
    }
}