// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs
{
    public class UserSessionJob : IJob
    {
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly ISessionHelper _sessionHelper;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IdServerHostOptions _options;

        public UserSessionJob(
            IUserSessionResitory userSessionRepository,
            ISessionHelper sessionHelper,
            IAuthenticationHelper authenticationHelper,
            IOptions<IdServerHostOptions> options)
        {
            _userSessionRepository = userSessionRepository;
            _sessionHelper = sessionHelper;
            _authenticationHelper = authenticationHelper;
            _options = options.Value;
        }

        public async Task Execute()
        {
            var inactiveSessions = await _userSessionRepository.GetInactiveAndNotNotified(CancellationToken.None);
            foreach(var inactiveSession  in inactiveSessions)
            {
                var sub = _authenticationHelper.GetLogin(inactiveSession.User);
                await _sessionHelper.Revoke(sub, inactiveSession, $"{_options.Authority}/{inactiveSession.Realm}", CancellationToken.None);
                inactiveSession.IsClientsNotified = true;
            }

            await _userSessionRepository.SaveChanges(CancellationToken.None);
        }
    }
}
