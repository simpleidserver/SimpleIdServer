// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Services;

public class CustomPasswordAuthenticationService : GenericAuthenticationService<AuthenticatePasswordViewModel>, IPasswordAuthenticationService
{
    private List<User> _users = new List<User>();

    public CustomPasswordAuthenticationService(IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(authenticationHelper, userRepository)
    {
        var user = UserBuilder.Create("provisionedUser", "password").Build();
        user.Realms.Clear();
        user.Realms.Add(new RealmUser
        {
            RealmsName = IdServer.Constants.DefaultRealm
        });
        _users.Add(user);
    }

    protected override Task<User> GetUser(string authenticatedUserId, AuthenticatePasswordViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        var result = _users.FirstOrDefault(u => u.Id == authenticatedUserId);
        if (result == null) result = _users.FirstOrDefault(u => u.Name == viewModel.Login);
        return Task.FromResult(result);
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        var authenticatedUser = await GetUser(authenticatedUserId, viewModel, realm, cancellationToken);
        if (authenticatedUser == null) return CredentialsValidationResult.Error(ValidationStatus.UNKNOWN_USER);
        return await Validate(realm, authenticatedUser, viewModel, cancellationToken);
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        var credential = authenticatedUser.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password);
        var hash = PasswordHelper.ComputeHash(viewModel.Password);
        if (credential == null || credential.Value != hash && credential.IsActive) return CredentialsValidationResult.Error(ValidationStatus.INVALIDCREDENTIALS);
        await Provision(authenticatedUser, cancellationToken);
        return CredentialsValidationResult.Ok(authenticatedUser);
    }
}
