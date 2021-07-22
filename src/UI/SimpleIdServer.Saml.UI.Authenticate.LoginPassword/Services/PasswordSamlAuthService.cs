// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Common.Exceptions;
using SimpleIdServer.Common.Helpers;
using SimpleIdServer.Saml.Idp.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.UI.Authenticate.LoginPassword.Services
{
    public class PasswordSamlAuthService : IPasswordSamlAuthService
    {
        private readonly IUserRepository _userRepository;

        public PasswordSamlAuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Authenticate(string login, string password, CancellationToken token)
        {
            var user = await _userRepository.FindOAuthUserByLogin(login, token);
            if (user == null)
            {
                throw new BaseUIException("UnknownUser");
            }

            var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.AMR);
            var hash = PasswordHelper.ComputeHash(password);
            if (credential == null || credential.Value != PasswordHelper.ComputeHash(password))
            {
                throw new BaseUIException("InvalidCredentials");
            }

            return user;
        }
    }
}
