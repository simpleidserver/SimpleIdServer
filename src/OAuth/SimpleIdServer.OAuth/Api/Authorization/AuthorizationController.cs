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
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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

        public async Task Get(CancellationToken token)
        {
            var jObjBody = Request.Query.ToJObject();
            var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userSubject = claimName == null ? string.Empty : claimName.Value;
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, jObjBody, null, Request.Cookies), new HandlerContextResponse(Response.Cookies));
            try
            {
                string url;
                var authorizationResponse = await _authorizationRequestHandler.Handle(context, token);
                if (authorizationResponse.Type == AuthorizationResponseTypes.RedirectUrl)
                {
                    var redirectUrlAuthorizationResponse = authorizationResponse as RedirectURLAuthorizationResponse;
                    url = QueryHelpers.AddQueryString(redirectUrlAuthorizationResponse.RedirectUrl, redirectUrlAuthorizationResponse.QueryParameters);
                    _responseModeHandler.Handle(jObjBody, redirectUrlAuthorizationResponse, HttpContext);
                    HttpContext.Response.Redirect(url);
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

                FormatRedirectUrl(parameters);
                var queryCollection = new QueryBuilder(parameters);
                var issuer = Request.GetAbsoluteUriWithVirtualPath();
                var returnUrl = $"{issuer}/{Constants.EndPoints.Authorization}{queryCollection.ToQueryString()}";
                var uiLocales = context.Request.Data.GetUILocalesFromAuthorizationRequest();
                url = Url.Action(redirectActionAuthorizationResponse.Action, redirectActionAuthorizationResponse.ControllerName, new
                {
                    ReturnUrl = _dataProtector.Protect(returnUrl),
                    area = redirectActionAuthorizationResponse.Area,
                    ui_locales = string.Join(" ", uiLocales)
                });
                HttpContext.Response.Redirect(url);
            }
            catch(OAuthExceptionBadRequestURIException ex)
            {
                await BuildErrorResponse(context, ex, true);
            }
            catch(OAuthException ex)
            {
                await BuildErrorResponse(context, ex);
            }
        }

        private async Task BuildErrorResponse(HandlerContext context, OAuthException ex, bool returnsJSON = false)
        {
            var redirectUri = context.Request.Data.GetRedirectUriFromAuthorizationRequest();
            var state = context.Request.Data.GetStateFromAuthorizationRequest();
            var jObj = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
            if (!string.IsNullOrWhiteSpace(state))
            {
                jObj.Add(ErrorResponseParameters.State, state);
            }
            if ((string.IsNullOrWhiteSpace(redirectUri) || !Uri.TryCreate(redirectUri, UriKind.Absolute, out Uri r)) || returnsJSON)
            {
                var payload = Encoding.UTF8.GetBytes(jObj.ToString());
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await HttpContext.Response.Body.WriteAsync(payload, 0, payload.Length);
                return;
            }

            var dic = jObj.ToEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var redirectUrlAuthorizationResponse = new RedirectURLAuthorizationResponse(redirectUri, dic);
            _responseModeHandler.Handle(context.Request.Data, redirectUrlAuthorizationResponse, HttpContext);
        }

        private static void FormatRedirectUrl(List<KeyValuePair<string, string>> parameters)
        {
            var kvp = parameters.FirstOrDefault(k => k.Key == AuthorizationRequestParameters.RedirectUri);
            if (!kvp.Equals(default(KeyValuePair<string, string>)) && !string.IsNullOrWhiteSpace(kvp.Value))
            {
                parameters.Remove(kvp);
                parameters.Add(new KeyValuePair<string, string>(AuthorizationRequestParameters.RedirectUri, HttpUtility.UrlEncode(kvp.Value)));
            }
        }
    }
}