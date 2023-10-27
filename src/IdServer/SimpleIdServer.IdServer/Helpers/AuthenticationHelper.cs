// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IAuthenticationHelper
    {
        string GetLogin(User user);
        Task<User> GetUserByLogin(Func<IQueryable<User>, IQueryable<User>> callback, string login, string realm, CancellationToken cancellationToken = default);
        IQueryable<User> FilterUsersByLogin(Func<IQueryable<User>, IQueryable<User>> callback, string login, string realm);
        IQueryable<User> FilterUsersByNotLogin(Func<IQueryable<User>, IQueryable<User>> callback, string login, string realm);
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

        public Task<User> GetUserByLogin(Func<IQueryable<User>, IQueryable<User>> callback, string login, string realm, CancellationToken cancellationToken = default)
        {
            var users = callback(_userRepository.Query());
            if (_options.IsEmailUsedDuringAuthentication) return users.SingleOrDefaultAsync(u => u.Email == login && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return users.SingleOrDefaultAsync(u => u.Name == login, cancellationToken);
        }

        public IQueryable<User> FilterUsersByLogin(Func<IQueryable<User>, IQueryable<User>> callback, string login, string realm)
        {
            var users = callback(_userRepository.Query());
            if (_options.IsEmailUsedDuringAuthentication) return users.Where(u => u.Email == login && u.Realms.Any(r => r.RealmsName == realm));
            return users.Where(u => u.Name == login && u.Realms.Any(r => r.RealmsName == realm));
        }

        public IQueryable<User> FilterUsersByNotLogin(Func<IQueryable<User>, IQueryable<User>> callback, string login, string realm)
        {
            var users = callback(_userRepository.Query());
            if (_options.IsEmailUsedDuringAuthentication) return users.Where(u => u.Email != login && u.Realms.Any(r => r.RealmsName == realm));
            return users.Where(u => u.Name != login && u.Realms.Any(r => r.RealmsName == realm));
        }

        public string GetLogin(User user) => _options.IsEmailUsedDuringAuthentication ? user.Email : user.Name;
    }
}
