// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Notification.Gotify.Apis;

public class GotifyConnectionsController
{
    private readonly IConfiguration _configuration;
    private readonly Infrastructures.IHttpClientFactory _httpClientFactory;

    public GotifyConnectionsController(
        IConfiguration configuration,
        Infrastructures.IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
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
            var options = GetOptions();
            var msg = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{options.BaseUrl}/user"),
                Content = new StringContent(JsonSerializer.Serialize(new { admin = false, name = newUserName, pass = newUserPassword }))
            };
            msg.Headers.Add("Authorization", $"Basic {Basic(options.AdminLogin, options.AdminPassword)}");
            var httpResult = await httpClient.SendAsync(msg);
            httpResult.EnsureSuccessStatusCode();
            msg = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{options.BaseUrl}/client"),
                Content = new StringContent(JsonSerializer.Serialize(new { name = "SimpleIdServer" }))
            };
            msg.Headers.Add("Authorization", $"Basic {Basic(newUserName, newUserPassword)}");
            httpResult = await httpClient.SendAsync(msg);
            httpResult.EnsureSuccessStatusCode();
            var content = await httpResult.Content.ReadAsStringAsync();
            return new OkObjectResult(JsonObject.Parse(content));
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
