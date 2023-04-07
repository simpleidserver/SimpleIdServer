// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services
{
    public interface IPasswordAuthService
    {
        Task<User> Authenticate(string realm, string login, string password, CancellationToken cancellationToken);
    }

    public class PasswordAuthService : IPasswordAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationHelper _userHelper;
        private readonly IEnumerable<IIdProviderAuthService> _authServices;

        public PasswordAuthService(IUserRepository userRepository, IAuthenticationHelper userHelper, IEnumerable<IIdProviderAuthService> authServices)
        {
            _userRepository = userRepository;
            _userHelper = userHelper;
            _authServices = authServices;
        }

        public virtual async Task<User> Authenticate(string realm, string login, string password, CancellationToken cancellationToken)
        {
            var user = await _userHelper.GetUserByLogin(_userRepository.Query().Include(u => u.IdentityProvisioning).ThenInclude(i => i.Properties).Include(u => u.Credentials).Include(u => u.Realms).Include(u => u.OAuthUserClaims), login, realm, cancellationToken);
            if (user == null)
                throw new BaseUIException(ErrorCodes.UNKNOWN_USER);

            var authService = _authServices.SingleOrDefault(s => s.Name == user.Source);
            if(authService != null)
            {
                if(!authService.Authenticate(user, user.IdentityProvisioning, password))
                    throw new BaseUIException(ErrorCodes.INVALID_CREDENTIALS);
            }
            else
            {
                var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password);
                var hash = PasswordHelper.ComputeHash(password);
                if (credential == null || credential.Value != PasswordHelper.ComputeHash(password) && credential.IsActive)
                    throw new BaseUIException(ErrorCodes.INVALID_CREDENTIALS);
            }

            return user;
        }
    }
}
