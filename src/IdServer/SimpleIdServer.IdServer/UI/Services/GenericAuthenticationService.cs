// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services;

public abstract class GenericAuthenticationService<TViewModel> : IUserAuthenticationService where TViewModel : BaseAuthenticateViewModel
{
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly IUserRepository _userRepository;

    public GenericAuthenticationService(IAuthenticationHelper authenticationHelper, IUserRepository userRepository)
    {
        _authenticationHelper = authenticationHelper;
        _userRepository = userRepository;
    }

    protected IUserRepository UserRepository => _userRepository;

    public Task<User> GetUser(string authenticatedUserId, object viewModel, string realm, CancellationToken cancellationToken)
    {
        return GetUser(authenticatedUserId, (TViewModel)viewModel, realm, cancellationToken);
    }

    public Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, object viewModel, CancellationToken cancellationToken)
    {
        return Validate(realm, authenticatedUserId, (TViewModel)viewModel, cancellationToken);
    }

    public Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, object viewModel, CancellationToken cancellationToken)
    {
        return Validate(realm, authenticatedUser, (TViewModel)viewModel, cancellationToken);
    }

    protected async Task Provision(User user, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Name == user.Name);
        if (existingUser != null) return;
        _userRepository.Add(user);
        await _userRepository.SaveChanges(cancellationToken);
    }

    protected abstract Task<User> GetUser(string authenticatedUserId, TViewModel viewModel, string realm, CancellationToken cancellationToken);

    protected abstract Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, TViewModel viewModel, CancellationToken cancellationToken);

    protected abstract Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, TViewModel viewModel, CancellationToken cancellationToken);

    protected async Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken)
    {
        var user = await _authenticationHelper.GetUserByLogin(_userRepository.Query()
            .Include(u => u.Realms)
            .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
            .Include(u => u.Groups)
            .Include(c => c.OAuthUserClaims)
            .Include(u => u.Credentials), login, realm, cancellationToken);
        return user;
    }

    protected async Task<User> FetchAuthenticatedUser(string realm, string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;
        return await _userRepository.Query()
            .Include(u => u.Realms)
            .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
            .Include(u => u.Groups)
            .Include(c => c.OAuthUserClaims)
            .Include(u => u.Credentials)
            .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Id == userId, cancellationToken);
    }
}
