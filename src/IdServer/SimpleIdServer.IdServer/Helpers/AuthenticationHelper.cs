// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IAuthenticationHelper
    {
        string GetLogin(User user);
        Task<User> GetUserByLogin(string login, string realm, CancellationToken cancellationToken = default);
        Task<bool> AtLeastOneUserWithSameEmail(string login, string email, string realm, CancellationToken cancellationToken = default);
        Task<bool> AtLeastOneUserWithSameClaim(string login, string name, string value, string realm, CancellationToken cancellationToken = default);
    }

    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly IdServerHostOptions _options;
        private readonly IUserRepository _userRepository;

        public AuthenticationHelper(IOptions<IdServerHostOptions> options, IUserRepository userRepository)
        {
            _options = options.Value;
            _userRepository = userRepository;
        }

        public string GetLogin(User user) => _options.IsEmailUsedDuringAuthentication ? user.Email : user.Name;

        public async Task<User> GetUserByLogin(string login, string realm, CancellationToken cancellationToken = default)
        {
            if (_options.IsEmailUsedDuringAuthentication) return await _userRepository.GetByEmail(login, realm, cancellationToken);
            return await _userRepository.GetBySubject(login, realm, cancellationToken);
        }

        public async Task<bool> AtLeastOneUserWithSameEmail(string login, string email, string realm, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetByEmail(email, realm, cancellationToken);
            if (existingUser == null) return false;
            if (_options.IsEmailUsedDuringAuthentication) return existingUser.Email != login;
            return existingUser.Name != login;
        }

        public async Task<bool> AtLeastOneUserWithSameClaim(string login, string name, string value, string realm, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetByClaim(name, value, realm, cancellationToken);
            if (existingUser == null) return false;
            if (_options.IsEmailUsedDuringAuthentication) return existingUser.Email != login;
            return existingUser.Name != login;
        }
    }
}
