// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers;

public interface ISessionHelper
{
    Task Revoke(string subject, UserSession session, string issuer, CancellationToken cancellationToken);
}

public class SessionHelper : ISessionHelper
{
    private readonly IdServer.Infrastructures.IHttpClientFactory _httpClientFactory;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly IClientRepository _clientRepository;
    private readonly IUserSessionResitory _userSessionRepository;
    private readonly IdServerHostOptions _options;

    public SessionHelper(
        IdServer.Infrastructures.IHttpClientFactory httpClientFactory,
        IJwtBuilder jwtBuilder,
        IClientRepository clientRepository,
        IUserSessionResitory userSessionRepository,
        IOptions<IdServerHostOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _jwtBuilder = jwtBuilder;
        _clientRepository = clientRepository;
        _userSessionRepository = userSessionRepository;
        _options = options.Value;
    }

    public async Task Revoke(string subject, UserSession session, string issuer, CancellationToken cancellationToken)
    {
        if (session == null) return;
        var targetedClients = await _clientRepository.Query()
            .AsNoTracking()
            .Where(c => session.ClientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == session.Realm) && !string.IsNullOrWhiteSpace(c.BackChannelLogoutUri))
            .ToListAsync(cancellationToken);
        if (!targetedClients.Any()) return;
        var currentDateTime = DateTime.UtcNow;
        var events = new JsonObject
        {
            { "http://schemas.openid.net/event/backchannel-logout", new JsonObject() }
        };
        using (var httpClient = _httpClientFactory.GetHttpClient())
        {
            foreach(var client in targetedClients)
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
                var logoutToken = _jwtBuilder.Sign(session.Realm, tokenDescriptor, signedResponseAlg);
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
                await httpClient.SendAsync(request);
            }
        }
    }
}