// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs
{
    public class UserSessionJob : IJob
    {
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly ISessionHelper _sessionHelper;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IKeyStore _keyStore;
        private readonly IdServerHostOptions _options;

        public UserSessionJob(
            IUserSessionResitory userSessionRepository,
            ISessionHelper sessionHelper,
            IAuthenticationHelper authenticationHelper,
            IClientRepository clientRepository,
            IKeyStore keyStore,
            IOptions<IdServerHostOptions> options)
        {
            _userSessionRepository = userSessionRepository;
            _sessionHelper = sessionHelper;
            _authenticationHelper = authenticationHelper;
            _clientRepository = clientRepository;
            _keyStore = keyStore;
            _options = options.Value;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task Execute()
        {
            var inactiveSessions = await _userSessionRepository.GetInactiveAndNotNotified(CancellationToken.None);
            if(inactiveSessions.Any())
            {
                var groupedSessions = inactiveSessions.GroupBy(s => s.Realm);
                foreach(var group in groupedSessions) 
                {
                    var clientIds = group
                        .SelectMany(s => s.ClientIds)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct();

                    var targetedClients = await _clientRepository.Query()
                        .AsNoTracking()
                        .Where(c => clientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == group.Key) && !string.IsNullOrWhiteSpace(c.BackChannelLogoutUri))
                        .ToListAsync();
                    var sigCredentials = _keyStore.GetAllSigningKeys(group.Key);
                    await Parallel.ForEachAsync(group.Select(_ => _), async (inactiveSession, c) =>
                    {
                        var sub = _authenticationHelper.GetLogin(inactiveSession.User);
                        var sessionClientIds = inactiveSession.ClientIds;
                        var sessionClients = targetedClients.Where(c => sessionClientIds.Contains(c.ClientId));
                        await _sessionHelper.Revoke(sub, sigCredentials, inactiveSession, sessionClients, $"{_options.Authority}/{inactiveSession.Realm}", CancellationToken.None);
                        inactiveSession.IsClientsNotified = true;
                    });
                }
            }

            await _userSessionRepository.SaveChanges(CancellationToken.None);
        }
    }
}
