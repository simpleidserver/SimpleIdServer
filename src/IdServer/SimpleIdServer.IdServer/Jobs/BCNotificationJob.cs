// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs
{
    public class BCNotificationJob : IJob
    {
        private readonly IBCAuthorizeRepository _repository;
        private readonly ILogger<BCNotificationJob> _logger;
        private readonly Infrastructures.IHttpClientFactory _httpClientFactory;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;

        public BCNotificationJob(IBCAuthorizeRepository repository, ILogger<BCNotificationJob> logger, Infrastructures.IHttpClientFactory httpClientFactory, IEnumerable<ITokenBuilder> tokenBuilders, IUserRepository userRepository, IClientRepository clientRepository)
        {
            _repository = repository;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenBuilders = tokenBuilders;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
        }

        public async Task Execute()
        {
            var notificationMethods = GetNotificationMethods();
            var allMethods = notificationMethods.Select(kvp => kvp.Key);
            var bcAuthorizeLst = await _repository.Query().Include(a => a.Histories).Where(a => a.LastStatus == Domains.BCAuthorizeStatus.Confirmed && allMethods.Contains(a.NotificationMode) && DateTime.UtcNow < a.ExpirationDateTime).ToListAsync(CancellationToken.None);
            foreach(var grp in bcAuthorizeLst.GroupBy(b => b.Realm))
            {
                var realmBcAuthorizeLst = grp.Select(g => g);
                var userIds = realmBcAuthorizeLst.Select(a => a.UserId).Distinct();
                var clientIds = realmBcAuthorizeLst.Select(a => a.ClientId).Distinct();
                var users = await _userRepository.GetAll(u => u.Include(u => u.Groups).Include(u => u.Realms).Include(u => u.OAuthUserClaims).AsNoTracking().Where(u => userIds.Contains(u.Id) && u.Realms.Any(r => r.RealmsName == grp.Key)).ToListAsync());
                var clients = await _clientRepository.Query()
                    .Include(c => c.SerializedJsonWebKeys)
                    .Include(c => c.Realms)
                    .Include(c => c.Scopes).AsNoTracking().Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == grp.Key)).ToListAsync();
                var parameter = new NotificationParameter { Clients = clients, Users = users.ToList() };
                await Parallel.ForEachAsync(realmBcAuthorizeLst, async (bc, t) =>
                {
                    var method = notificationMethods[bc.NotificationMode];
                    await method(bc, parameter);
                });
            }

            await _repository.SaveChanges(CancellationToken.None);
        }

        protected virtual async Task HandlePingNotification(BCAuthorize bcAuthorize, NotificationParameter parameter)
        {
            try
            {
                using (var httpClient = _httpClientFactory.GetHttpClient())
                {
                    var content = new JsonObject
                    {
                        { BCAuthenticationResponseParameters.AuthReqId, bcAuthorize.Id }
                    };
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri(bcAuthorize.NotificationEdp),
                        Content = new StringContent(content.ToJsonString(), Encoding.UTF8, "application/json")
                    };
                    httpRequestMessage.Headers.Add(Constants.AuthorizationHeaderName, $"{AutenticationSchemes.Bearer} {bcAuthorize.NotificationToken}");
                    var httpResult = await httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
                    httpResult.EnsureSuccessStatusCode();
                    bcAuthorize.Pong();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        protected virtual async Task HandlePushNotification(BCAuthorize bcAuthorize, NotificationParameter parameter)
        {
            var parameters = await BuildParameters();
            var content = new JsonObject();
            foreach (var kvp in parameters)
                content.Add(kvp.Key, kvp.Value);

            try
            {
                using (var httpClient = _httpClientFactory.GetHttpClient())
                {
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri(bcAuthorize.NotificationEdp),
                        Content = new StringContent(content.ToJsonString(), Encoding.UTF8, "application/json")
                    };
                    httpRequestMessage.Headers.Add(Constants.AuthorizationHeaderName, $"{AutenticationSchemes.Bearer} {bcAuthorize.NotificationToken}");
                    var httpResult = await httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
                    httpResult.EnsureSuccessStatusCode();
                    bcAuthorize.Send();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            async Task<Dictionary<string, string>> BuildParameters()
            {
                var context = new HandlerContext(new HandlerContextRequest(null, null, new JsonObject(), null, null, (X509Certificate2)null, string.Empty), bcAuthorize.Realm);
                context.SetUser(parameter.Users.First(u => u.Id == bcAuthorize.UserId));
                context.SetClient(parameter.Clients.First(c => c.ClientId == bcAuthorize.ClientId && c.Realms.Any(r => r.Name == bcAuthorize.Realm)));
                foreach (var tokenBuilder in _tokenBuilders)
                    await tokenBuilder.Build(new BuildTokenParameter { Scopes = bcAuthorize.Scopes, AuthorizationDetails = bcAuthorize.AuthorizationDetails }, context, CancellationToken.None);
                return context.Response.Parameters;
            }
        }

        protected virtual Dictionary<string, Func<BCAuthorize, NotificationParameter, Task>> GetNotificationMethods() => new Dictionary<string, Func<BCAuthorize, NotificationParameter, Task>>
        {
            { Constants.StandardNotificationModes.Ping, HandlePingNotification },
            { Constants.StandardNotificationModes.Push, HandlePushNotification }
        };

        protected class NotificationParameter
        {
            public List<Client> Clients { get; set; }
            public List<User> Users { get; set; }
        }
    }
}
