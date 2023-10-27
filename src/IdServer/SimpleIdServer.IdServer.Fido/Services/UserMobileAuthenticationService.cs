// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Caching.Distributed;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.Apis;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using System.Security.Claims;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.Services;

public interface IMobileAuthenticationService : IUserAuthenticationService
{

}

public class UserMobileAuthenticationService : GenericAuthenticationService<AuthenticateMobileViewModel>, IMobileAuthenticationService
{
    private readonly IDistributedCache _distributedCache;

    public UserMobileAuthenticationService(IDistributedCache distributedCache, IAuthenticationHelper authenticationHelper, IUserRepository userRepository) : base(authenticationHelper, userRepository)
    {
        _distributedCache = distributedCache;
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, AuthenticateMobileViewModel viewModel, CancellationToken cancellationToken)
    {
        var authenticatedUser = await GetUser(authenticatedUserId, viewModel, realm, cancellationToken);
        if (authenticatedUser == null) return CredentialsValidationResult.Error(ValidationStatus.UNKNOWN_USER);
        return await Validate(realm, authenticatedUser, null, viewModel, cancellationToken);
    }

    protected override async Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, ICollection<Claim> claims, AuthenticateMobileViewModel viewModel, CancellationToken cancellationToken)
    {
        var session = await _distributedCache.GetStringAsync(viewModel.SessionId, cancellationToken);
        if (string.IsNullOrWhiteSpace(session))
        {
            return CredentialsValidationResult.Error("unknown_session", "unknown_session");
        }

        var sessionRecord = JsonSerializer.Deserialize<AuthenticationSessionRecord>(session);
        if (!sessionRecord.IsValidated)
        {
            return CredentialsValidationResult.Error("session_not_validated", "session_not_validated");
        }

        return CredentialsValidationResult.Ok(authenticatedUser);
    }

    private async Task<User> GetUser(string authenticatedUserId, AuthenticateMobileViewModel viewModel, string realm, CancellationToken cancellationToken)
    {
        User authenticatedUser = null;
        if (string.IsNullOrWhiteSpace(authenticatedUserId))
            authenticatedUser = await AuthenticateUser(viewModel.Login, realm, cancellationToken);
        else
            authenticatedUser = await FetchAuthenticatedUser(realm, authenticatedUserId, cancellationToken);

        return authenticatedUser;
    }
}
