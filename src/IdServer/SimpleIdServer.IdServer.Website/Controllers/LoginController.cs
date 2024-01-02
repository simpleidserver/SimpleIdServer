// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Website.ViewModels;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Controllers;

public class LoginController : Controller
{
    private readonly DefaultSecurityOptions _defaultSecurityOptions;
    private readonly ILogger<LoginController> _logger;

    public LoginController(
        DefaultSecurityOptions defaultSecurityOptions,
        ILogger<LoginController> logger)
    {
        _defaultSecurityOptions = defaultSecurityOptions;
        _logger = logger;
    }

    [Route("login")]
    public IActionResult Login(string acrValues)
    {
        var items = new Dictionary<string, string>
        {
            { "scheme", "oidc" },
        };
        var props = new AuthenticationProperties(items);
        props.SetParameter(OpenIdConnectParameterNames.Prompt, "login");
        props.SetParameter(OpenIdConnectParameterNames.AcrValues, acrValues);
        props.SetParameter(OpenIdConnectParameterNames.RedirectUri, GetRedirectUri());
        var result = Challenge(props, "oidc");
        return result;
    }

    [Route("callback")]
    public async Task<IActionResult> Callback()
    {
        _logger.LogInformation("Execute callback");
        var tokenEndpoint = $"{_defaultSecurityOptions.Issuer}/token";
        var userInfoEndpoint = $"{_defaultSecurityOptions.Issuer}/userinfo";
        var authorizationResponse = await ExtractAuthorizationResponse();
        var tokenResponse = await GetToken(authorizationResponse, tokenEndpoint);
        var userInfo = await GetUserInfo(tokenResponse, userInfoEndpoint);
        return View(new CallbackViewModel { Claims = userInfo });
    }

    private async Task<OpenIdConnectMessage> ExtractAuthorizationResponse()
    {
        OpenIdConnectMessage message = null;
        if (HttpMethods.IsGet(Request.Method))
        {
            var queries = Request.Query.Select(pair => new KeyValuePair<string, string[]>(pair.Key, pair.Value.ToArray()));
            message = new OpenIdConnectMessage(queries);
        }
        else if(HttpMethods.IsPost(Request.Method) && !string.IsNullOrEmpty(Request.ContentType)
          && Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)
          && Request.Body.CanRead)
        {
            var form = await Request.ReadFormAsync();
            var queries = form.Select(pair => new KeyValuePair<string, string[]>(pair.Key, pair.Value.ToArray()));
            message = new OpenIdConnectMessage(queries);
        }

        return message;
    }

    private async Task<OpenIdConnectMessage> GetToken(OpenIdConnectMessage authorizationResponse, string tokenEndpoint)
    {
        var redirectUri = GetRedirectUri();
        using (var httpClient = new HttpClient())
        {
            var tokenEndpointRequest = new OpenIdConnectMessage
            {
                ClientId = _defaultSecurityOptions.ClientId,
                Code = authorizationResponse.Code,
                GrantType = OpenIdConnectGrantTypes.AuthorizationCode,
                RedirectUri = redirectUri,
                ClientSecret = _defaultSecurityOptions.ClientSecret
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            requestMessage.Content = new FormUrlEncodedContent(tokenEndpointRequest.Parameters);
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            return new OpenIdConnectMessage(json);
        }
    }

    private async Task<Dictionary<string, string>> GetUserInfo(OpenIdConnectMessage token, string userInfoEndpoint)
    {
        using(var httpClient = new HttpClient())
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(userInfoEndpoint) };
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
            var httpResult = await httpClient.SendAsync(request);
            var json = await httpResult.Content.ReadAsStringAsync();
            var jObj = JsonObject.Parse(json).AsObject();
            var result = new Dictionary<string, string>();
            foreach(var record in jObj)
            {
                if (record.Value == null) continue;
                result.Add(record.Key, record.Value.ToString());
            }

            return result;
        }
    }

    private string GetRedirectUri()
    {
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        return $"{issuer}/callback";
    }
}
