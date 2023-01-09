// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services
{
    public interface IPasswordAuthService
    {
        Task<User> Authenticate(string login, string password, CancellationToken cancellationToken);
    }

    public class PasswordAuthService : IPasswordAuthService
    {
        private readonly IUserRepository _userRepository;

        public PasswordAuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public virtual async Task<User> Authenticate(string login, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Query().Include(u => u.Credentials).FirstOrDefaultAsync(u => u.Id == login, cancellationToken);
            if (user == null)
                throw new BaseUIException(ErrorCodes.UNKNOWN_USER);

            var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password);
            var hash = PasswordHelper.ComputeHash(password);
            if (credential == null || credential.Value != PasswordHelper.ComputeHash(password))
                throw new BaseUIException(ErrorCodes.INVALID_CREDENTIALS);

            return user;
        }
    }
}
