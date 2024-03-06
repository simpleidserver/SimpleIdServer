// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Store;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Notification.Gotify;

public class GotifyUserNotificationService : IUserNotificationService
{
    private readonly IConfiguration _configuration;
    private readonly Infrastructures.IHttpClientFactory _httpClientFactory;
    private readonly IGotiySessionStore _gotifySessionStore;

    public GotifyUserNotificationService(
        IConfiguration configuration, 
        Infrastructures.IHttpClientFactory httpClientFactory,
        IGotiySessionStore gotifySessionStore)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _gotifySessionStore = gotifySessionStore;
    }

    public string Name => Constants.NotificationName;

    public async Task Send(string title, string body, Dictionary<string, string> data, User user)
    {
        if (user.Devices == null || !user.Devices.Any(d => d.PushType == Constants.NotificationName)) throw new OAuthException(ErrorCodes.UNEXPECTED_ERROR, Global.MissingRegisteredUserDevice);
        var userDevice = user.Devices.First(d => d.PushType == Constants.NotificationName);
        await Send(title, body, data, userDevice.PushToken);
    }

    public async Task Send(string title, string body, Dictionary<string, string> data, string destination)
    {
        var options = GetOptions();
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            var obj = new
            {
                message = body,
                title = title,
                extras = data,
                priority = 1
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{options.BaseUrl}/message"),
                Content = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };
            var  session = await _gotifySessionStore.GetByClientToken(destination, CancellationToken.None);
            requestMessage.Headers.Add("X-Gotify-Key", session.ApplicationToken);
            var httpResult = await httpClient.SendAsync(requestMessage);
            httpResult.EnsureSuccessStatusCode();
        }
    }

    private GotifyOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(GotifyOptions).Name);
        return section.Get<GotifyOptions>();
    }
}
