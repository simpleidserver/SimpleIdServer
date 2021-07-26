// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Exceptions;
using SimpleIdServer.Common.Helpers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.LoginPassword.Services
{
    public class PasswordAuthService : IPasswordAuthService
    {
        private readonly IOAuthUserRepository _oauthUserRepository;

        public PasswordAuthService(IOAuthUserRepository oauthUserRepository)
        {
            _oauthUserRepository = oauthUserRepository;
        }

        public virtual async Task<OAuthUser> Authenticate(string login, string password, CancellationToken token)
        {
            var user = await _oauthUserRepository.FindOAuthUserByLogin(login, token);
            if (user == null)
            {
                throw new BaseUIException(Exceptions.ErrorCodes.UNKNOWN_USER);
            }

            var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.AMR);
            var hash = PasswordHelper.ComputeHash(password);
            if (credential == null || credential.Value != PasswordHelper.ComputeHash(password))
            {
                throw new BaseUIException(Exceptions.ErrorCodes.INVALID_CREDENTIALS);
            }

            return user;
        }
    }
}
