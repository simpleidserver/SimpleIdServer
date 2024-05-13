// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface ISessionHelper
{
    Task RevokeBackChannels(string subject, IEnumerable<SigningCredentials> signingCredentials, UserSession session, IEnumerable<Client> clients, string issuer, CancellationToken cancellationToken);
}

public class SessionHelper : ISessionHelper
{
    private readonly IdServer.Infrastructures.IHttpClientFactory _httpClientFactory;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly IdServerHostOptions _options;
    private readonly ILogger<SessionHelper> _logger;

    public SessionHelper(
        IdServer.Infrastructures.IHttpClientFactory httpClientFactory,
        IJwtBuilder jwtBuilder,
        IOptions<IdServerHostOptions> options,
        ILogger<SessionHelper> logger)
    {
        _httpClientFactory = httpClientFactory;
        _jwtBuilder = jwtBuilder;
        _options = options.Value;
        _logger = logger;
    }

    public async Task RevokeBackChannels(string subject, IEnumerable<SigningCredentials> signingCredentials, UserSession session, IEnumerable<Client> clients, string issuer, CancellationToken cancellationToken)
    {
        if (!clients.Any()) return;
        var currentDateTime = DateTime.UtcNow;
        var events = new Dictionary<string, string>
        {
            { "http://schemas.openid.net/event/backchannel-logout", "{}" }
        };
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            foreach(var client in clients)
            {
                var jwsPayload = new Dictionary<string, object>
                {
                    { JwtRegisteredClaimNames.Sub, subject },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
                    { Constants.UserClaims.Events, events }
                };
                if (client.BackChannelLogoutSessionRequired)
                {
                    jwsPayload.Add(JwtRegisteredClaimNames.Sid, session.SessionId);
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = issuer,
                    Audience = client.ClientId,
                    IssuedAt = currentDateTime,
                    Claims = jwsPayload
                };
                var signedResponseAlg = client.TokenSignedResponseAlg ?? _options.DefaultTokenSignedResponseAlg;
                var logoutToken = _jwtBuilder.Sign(signingCredentials, session.Realm, tokenDescriptor, signedResponseAlg);
                var body = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("logout_token", logoutToken)
                });
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = body,
                    RequestUri = new Uri(client.BackChannelLogoutUri)
                };
                try
                {
                    await httpClient.SendAsync(request);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}