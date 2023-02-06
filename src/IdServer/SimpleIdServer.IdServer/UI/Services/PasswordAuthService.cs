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
        private readonly IAuthenticationHelper _userHelper;

        public PasswordAuthService(IUserRepository userRepository, IAuthenticationHelper userHelper)
        {
            _userRepository = userRepository;
            _userHelper = userHelper;
        }

        public virtual async Task<User> Authenticate(string login, string password, CancellationToken cancellationToken)
        {
            var user = await _userHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials).Include(u => u.OAuthUserClaims), login, cancellationToken);
            if (user == null)
                throw new BaseUIException(ErrorCodes.UNKNOWN_USER);

            var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password);
            var hash = PasswordHelper.ComputeHash(password);
            if (credential == null || credential.Value != PasswordHelper.ComputeHash(password) && credential.IsActive)
                throw new BaseUIException(ErrorCodes.INVALID_CREDENTIALS);

            return user;
        }
    }
}
