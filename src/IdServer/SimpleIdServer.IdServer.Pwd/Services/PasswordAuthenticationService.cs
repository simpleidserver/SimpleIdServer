// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.Services;

public interface IPasswordAuthenticationService : IUserAuthenticationService
{

}

public class PasswordAuthenticationService : GenericAuthenticationService<AuthenticatePasswordViewModel>, IPasswordAuthenticationService
{
    private readonly IEnumerable<IIdProviderAuthService> _authServices;

    public PasswordAuthenticationService(IEnumerable<IIdProviderAuthService> authServices, IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(authenticationHelper, userRepository)
    {
        _authServices = authServices;
    }

    public override string Amr => Constants.Areas.Password;

    protected override async Task<User> GetUser(string authenticatedUserId, AuthenticatePasswordViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
            authenticatedUser = await AuthenticateUser(viewModel.Login, realm, cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        var authenticatedUser = await GetUser(authenticatedUserId, viewModel, realm, cancellationToken);
        if (authenticatedUser == null) return CredentialsValidationResult.Error(ValidationStatus.UNKNOWN_USER);
        return await Validate(realm, authenticatedUser, viewModel, cancellationToken);
    }

    protected override Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, AuthenticatePasswordViewModel viewModel, CancellationToken cancellationToken)
    {
        var authService = _authServices.SingleOrDefault(s => s.Name == authenticatedUser.Source);
        if (authService != null)
        {
            if (!authService.Authenticate(authenticatedUser, authenticatedUser.IdentityProvisioning, viewModel.Password)) return Task.FromResult(CredentialsValidationResult.Error(ValidationStatus.INVALIDCREDENTIALS));
        }
        else
        {
            var credential = authenticatedUser.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password);
            var hash = PasswordHelper.ComputeHash(viewModel.Password);
            if (credential == null || credential.Value != hash && credential.IsActive) return Task.FromResult(CredentialsValidationResult.Error(ValidationStatus.INVALIDCREDENTIALS));
        }

        return Task.FromResult(CredentialsValidationResult.Ok(authenticatedUser));
    }
}
