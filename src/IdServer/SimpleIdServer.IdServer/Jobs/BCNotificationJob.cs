// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
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
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IdServerHostOptions _options;

        public BCNotificationJob(
            IBCAuthorizeRepository repository, 
            ILogger<BCNotificationJob> logger,
            Infrastructures.IHttpClientFactory httpClientFactory, 
            IEnumerable<ITokenBuilder> tokenBuilders, 
            IUserRepository userRepository,
            IClientRepository clientRepository,
            ITransactionBuilder transactionBuilder,
            IOptions<IdServerHostOptions> options)
        {
            _repository = repository;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenBuilders = tokenBuilders;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _transactionBuilder = transactionBuilder;
            _options = options.Value;
        }

        public async Task Execute()
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var notificationMethods = GetNotificationMethods();
                var allMethods = notificationMethods.Select(kvp => kvp.Key).ToList();
                var bcAuthorizeLst = await _repository.GetAllConfirmed(allMethods, CancellationToken.None);
                foreach (var grp in bcAuthorizeLst.GroupBy(b => b.Realm))
                {
                    var realmBcAuthorizeLst = grp.Select(g => g);
                    var userIds = realmBcAuthorizeLst.Select(a => a.UserId).Distinct();
                    var clientIds = realmBcAuthorizeLst.Select(a => a.ClientId).Distinct().ToList();
                    var users = await _userRepository.GetUsersById(userIds, grp.Key, CancellationToken.None);
                    var clients = await _clientRepository.GetByClientIds(grp.Key, clientIds, CancellationToken.None);
                    var parameter = new NotificationParameter { Clients = clients, Users = users.ToList() };
                    await Parallel.ForEachAsync(realmBcAuthorizeLst, async (bc, t) =>
                    {
                        var method = notificationMethods[bc.NotificationMode];
                        await method(bc, parameter);
                        _repository.Update(bc);
                    });
                }

                await transaction.Commit(CancellationToken.None);
            }
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
                var context = new HandlerContext(new HandlerContextRequest(null, null, new JsonObject(), null, null, (X509Certificate2)null, string.Empty), bcAuthorize.Realm, _options);
                context.SetUser(parameter.Users.First(u => u.Id == bcAuthorize.UserId), null);
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
