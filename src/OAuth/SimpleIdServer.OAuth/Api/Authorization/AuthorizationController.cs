// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Authorization.ResponseModes;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    [Route(Constants.EndPoints.Authorization)]
    public class AuthorizationController : Controller
    {
        private readonly IAuthorizationRequestHandler _authorizationRequestHandler;
        private readonly IResponseModeHandler _responseModeHandler;
        private readonly IDataProtector _dataProtector;

        public AuthorizationController(IAuthorizationRequestHandler authorizationRequestHandler, IResponseModeHandler responseModeHandler, IDataProtectionProvider dataProtectionProvider)
        {
            _authorizationRequestHandler = authorizationRequestHandler;
            _responseModeHandler = responseModeHandler;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        }

        public async Task Get()
        {
            var jObjBody = Request.Query.ToJObject();
            var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var claimAuthTime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationInstant);
            var userSubject = claimName == null ? string.Empty : claimName.Value;
            DateTime? authTime = null;
            DateTime auth;
            if (claimAuthTime != null && !string.IsNullOrWhiteSpace(claimAuthTime.Value) && DateTime.TryParse(claimAuthTime.Value, out auth))
            {
                authTime = auth;
            }

            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, authTime, jObjBody, null, Request.Cookies), new HandlerContextResponse(Response.Cookies));
            try
            {
                string url;
                var authorizationResponse = await _authorizationRequestHandler.Handle(context);
                if (authorizationResponse.Type == AuthorizationResponseTypes.RedirectUrl)
                {
                    var redirectUrlAuthorizationResponse = authorizationResponse as RedirectURLAuthorizationResponse;
                    url = QueryHelpers.AddQueryString(redirectUrlAuthorizationResponse.RedirectUrl, redirectUrlAuthorizationResponse.QueryParameters);
                    _responseModeHandler.Handle(jObjBody, redirectUrlAuthorizationResponse, HttpContext);
                    return;
                }

                var redirectActionAuthorizationResponse = authorizationResponse as RedirectActionAuthorizationResponse;
                var parameters = new List<KeyValuePair<string, string>>();
                foreach(var record in redirectActionAuthorizationResponse.QueryParameters)
                {
                    var jArr = record.Value as JArray;
                    if (jArr != null)
                    {
                        foreach(var rec in jArr)
                        {
                            parameters.Add(new KeyValuePair<string, string>(record.Key, rec.ToString()));
                        }
                    }
                    else
                    {
                        parameters.Add(new KeyValuePair<string, string>(record.Key, record.Value.ToString()));
                    }
                }

                var queryCollection = new QueryBuilder(parameters);
                var returnUrl = $"{Request.Path}{queryCollection.ToQueryString()}";
                url = Url.Action(redirectActionAuthorizationResponse.Action, redirectActionAuthorizationResponse.ControllerName, new { ReturnUrl = _dataProtector.Protect(returnUrl), area = redirectActionAuthorizationResponse.Area });
                HttpContext.Response.Redirect(url);
            }
            catch(OAuthException ex)
            {
                var jObj = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                var payload = Encoding.UTF8.GetBytes(jObj.ToString());
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                HttpContext.Response.Body.Write(payload, 0, payload.Length);
            }
        }
    }
}