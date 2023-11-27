// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Authorization.ResponseModes;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.IdServer.Api.Authorization;

public class AuthorizationController : Controller
{
    private readonly IAuthorizationRequestHandler _authorizationRequestHandler;
    private readonly IResponseModeHandler _responseModeHandler;
    private readonly IDataProtector _dataProtector;
    private readonly IBusControl _busControl;
    private readonly IdServerHostOptions _options;

    public AuthorizationController(
        IAuthorizationRequestHandler authorizationRequestHandler, 
        IResponseModeHandler responseModeHandler, 
        IDataProtectionProvider dataProtectionProvider, 
        IBusControl busControl,
        IOptions<IdServerHostOptions> options)
    {
        _authorizationRequestHandler = authorizationRequestHandler;
        _responseModeHandler = responseModeHandler;
        _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        _busControl = busControl;
        _options = options.Value;
    }

    [HttpGet]
    public async Task Get([FromRoute] string prefix, [FromQuery] AuthorizationRequest request, CancellationToken token)
    {
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Authorization"))
        {
            var jObjBody = Request.Query.ToJObject();
            var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userSubject = claimName == null ? string.Empty : claimName.Value;
            var referer = string.Empty;
            if (Request.Headers.Referer.Any()) referer = Request.Headers.Referer.First();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, jObjBody, null, Request.Cookies, referer), prefix ?? Constants.DefaultRealm, _options, new HandlerContextResponse(Response.Cookies));
            activity?.SetTag("realm", context.Realm);
            try
            {
                var authorizationResponse = await _authorizationRequestHandler.Handle(context, token);
                if (authorizationResponse.Type == AuthorizationResponseTypes.RedirectUrl)
                {
                    var redirectUrlAuthorizationResponse = authorizationResponse as RedirectURLAuthorizationResponse;
                    _responseModeHandler.Handle(context, redirectUrlAuthorizationResponse, HttpContext);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Authorization Success");
                    await _busControl.Publish(new AuthorizationSuccessEvent
                    {
                        ClientId = context.Client.ClientId,
                        Realm = context.Realm,
                        RequestJSON = jObjBody.ToString()
                    });
                    return;
                }

                var redirectActionAuthorizationResponse = authorizationResponse as RedirectActionAuthorizationResponse;
                if (redirectActionAuthorizationResponse.Disconnect)
                {
                    if (redirectActionAuthorizationResponse.CookiesToRemove != null)
                    {
                        foreach (var cookieName in redirectActionAuthorizationResponse.CookiesToRemove)
                        {
                            Response.Cookies.Delete(cookieName);
                        }
                    }

                    try
                    {
                        await HttpContext.SignOutAsync();
                    }
                    catch { }
                }

                var parameters = new List<KeyValuePair<string, string>>();
                foreach (var record in redirectActionAuthorizationResponse.QueryParameters)
                {
                    if (record.Value is JsonArray)
                    {
                        var jArr = record.Value.AsArray();
                        foreach (var rec in jArr)
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
                if (!string.IsNullOrWhiteSpace(prefix))
                    issuer = $"{issuer}/{prefix}";
                var returnUrl = $"{issuer}/{Constants.EndPoints.Authorization}{queryCollection.ToQueryString()}";
                var uiLocales = context.Request.RequestData.GetUILocalesFromAuthorizationRequest();
                var url = Url.Action(redirectActionAuthorizationResponse.Action, redirectActionAuthorizationResponse.ControllerName, new
                {
                    ReturnUrl = _dataProtector.Protect(returnUrl),
                    area = redirectActionAuthorizationResponse.Area,
                    ui_locales = string.Join(" ", uiLocales)
                });
                activity?.SetStatus(ActivityStatusCode.Ok, $"User agent will be redirect to '{url}'");
                await _busControl.Publish(new AuthorizationFailureEvent
                {
                    ClientId = context.Client.ClientId,
                    Realm = context.Realm,
                    RequestJSON = jObjBody.ToString(),
                    ErrorMessage = $"User agent will be redirect to '{url}'"
                });
                HttpContext.Response.Redirect(url);
            }
            catch (OAuthExceptionBadRequestURIException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new AuthorizationFailureEvent
                {
                    ClientId = context.Client.ClientId,
                    Realm = context.Realm,
                    RequestJSON = jObjBody.ToString(),
                    ErrorMessage = ex.Message
                });
                await BuildErrorResponse(context, ex, true);
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new AuthorizationFailureEvent
                {
                    ClientId = context.Client?.ClientId,
                    Realm = context.Realm,
                    RequestJSON = jObjBody.ToString(),
                    ErrorMessage = ex.Message
                });
                await BuildErrorResponse(context, ex);
            }
        }
    }

    private async Task BuildErrorResponse(HandlerContext context, OAuthException ex, bool returnsJSON = false)
    {
        var redirectUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
        var state = context.Request.RequestData.GetStateFromAuthorizationRequest();
        var jObj = new JsonObject
        {
            [ErrorResponseParameters.Error] = ex.Code,
            [ErrorResponseParameters.ErrorDescription] = ex.Message
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
        _responseModeHandler.Handle(context, redirectUrlAuthorizationResponse, HttpContext);
    }

    private static void FormatRedirectUrl(List<KeyValuePair<string, string>> parameters)
    {
        var kvp = parameters.FirstOrDefault(k => k.Key == AuthorizationRequestParameters.RedirectUri);
        if (!kvp.Equals(default(KeyValuePair<string, string>)) && !string.IsNullOrWhiteSpace(kvp.Value) && !IsHtmlEncoded(kvp.Value))
        {
            parameters.Remove(kvp);
            parameters.Add(new KeyValuePair<string, string>(AuthorizationRequestParameters.RedirectUri, HttpUtility.UrlEncode(kvp.Value)));
        }
    }

    private static bool IsHtmlEncoded(string url) => HttpUtility.UrlDecode(url) != url;
}

public class AuthorizationRequest
{
    [FromQuery(Name = AuthorizationRequestParameters.ResponseType)]
    public string ResponseType { get; set; }
    [FromQuery(Name = AuthorizationRequestParameters.ClientId)]
    public string ClientId { get; set; }
    [FromQuery(Name = AuthorizationRequestParameters.RedirectUri)]
    public string RedirectUri { get; set; }
    [FromQuery(Name = AuthorizationRequestParameters.Scope)]
    public string Scope { get; set; }
    [FromQuery(Name = AuthorizationRequestParameters.State)]
    public string State { get; set; }
}