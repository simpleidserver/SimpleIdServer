// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Store;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Notification.Gotify.Apis;

public class GotifyConnectionsController
{
    private readonly IConfiguration _configuration;
    private readonly Infrastructures.IHttpClientFactory _httpClientFactory;
    private readonly IGotiySessionStore _gotiySessionStore;

    public GotifyConnectionsController(
        IConfiguration configuration,
        Infrastructures.IHttpClientFactory httpClientFactory,
        IGotiySessionStore gotiySessionStore)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _gotiySessionStore = gotiySessionStore;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromRoute] string prefix, 
        [FromBody] AddGotifyConnectionRequest request, 
        CancellationToken cancellationToken)
    {
        var newUserName = Guid.NewGuid().ToString();
        var newUserPassword = Guid.NewGuid().ToString();
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            // create the user.
            var options = GetOptions();
            var msg = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{options.BaseUrl}/user"),
                Content = new StringContent(JsonSerializer.Serialize(new { admin = false, name = newUserName, pass = newUserPassword }), Encoding.UTF8, "application/json")
            };
            msg.Headers.Add("Authorization", $"Basic {Basic(options.AdminLogin, options.AdminPassword)}");
            var httpResult = await httpClient.SendAsync(msg, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var basic = Basic(newUserName, newUserPassword);

            // create the simpleidserver application.
            msg = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{options.BaseUrl}/application"),
                Content = new StringContent(JsonSerializer.Serialize(new { defaultPriority = 1, name = "SimpleIdServer", description = "SimpleIdServer" }), Encoding.UTF8, "application/json")
            };
            msg.Headers.Add("Authorization", $"Basic {basic}");
            httpResult = await httpClient.SendAsync(msg, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var applicationToken = JsonObject.Parse(json)["token"].ToString();

            // create the mobile client.
            msg = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{options.BaseUrl}/client"),
                Content = new StringContent(JsonSerializer.Serialize(new { name = "MobileClient" }), Encoding.UTF8, "application/json")
            };
            msg.Headers.Add("Authorization", $"Basic {basic}");
            httpResult = await httpClient.SendAsync(msg, cancellationToken);
            httpResult.EnsureSuccessStatusCode();
            json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var jObj = JsonObject.Parse(json);
            var clientToken = jObj["token"].ToString();
            _gotiySessionStore.Add(new Domains.GotifySession
            {
                ClientToken = clientToken,
                ApplicationToken = applicationToken
            });
            await _gotiySessionStore.SaveChanges(cancellationToken);
            return new OkObjectResult(jObj);
        }
    }

    static string Basic(string login, string pwd)
        => Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(login + ":" + pwd));

    private GotifyOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(GotifyOptions).Name);
        return section.Get<GotifyOptions>();
    }
}
